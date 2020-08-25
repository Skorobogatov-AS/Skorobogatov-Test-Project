using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommonUtils;
using Server.Annotations;
using Server.Utils;

namespace Server.Models
{
    /// <summary>
    /// Модель сервера.
    /// </summary>
    public sealed class ServerModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Идетификатор последнего зарегистрированного пользователя.
        /// </summary>
        private static int _lastRegistredUserId;

        /// <summary>
        /// Порт.
        /// </summary>
        private int _port;

        /// <summary>
        /// Клиент серверного подключения.
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// <see cref="ServerMessages"/>
        /// </summary>
        private string _serverMessages;

        /// <summary>
        /// <see cref="IsRunning"/>
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// Список всех сообщений сервера.
        /// </summary>
        public string ServerMessages
        {
            get => _serverMessages;
            private set
            {
                _serverMessages = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Сервер запущен.
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Порт.
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный сетевой интерфейс.
        /// </summary>
        public NetworkInterface SelectedInterface { get; set; }

        /// <summary>
        /// Коллекция подключенных клиентов.
        /// </summary>
        public ObservableCollection<ConnectedClient> ConnectedClients { get; }

        /// <summary>
        /// Коллекция сетевых интерфейсов.
        /// </summary>
        public ObservableCollection<NetworkInterface> NetworkInterfaces { get; }

        public ServerModel()
        {
            IsRunning = false;
            ConnectedClients = new ObservableCollection<ConnectedClient>();
            NetworkInterfaces = new ObservableCollection<NetworkInterface>(
                NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up)
            );
        }

        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public void StartServer()
        {
            IsRunning = true;

            var ipAdress = ServerUtils.GetIpv4Adress(SelectedInterface);

            _listener = new TcpListener(ipAdress, Port);
            _listener.Start();
            Logger.GetInstance(logText => { ServerMessages += $"{logText}\n"; });
            Logger.GetInstance().LogMessage("Server started");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var addedClient = new ConnectedClient();
                var tokenSource = new CancellationTokenSource(ReservedParams.USER_TIMEOUT);

                Task.Factory.StartNew(() => { StartNewClientThread(client, addedClient, tokenSource); },
                    tokenSource.Token);
            }
        }

        /// <summary>
        /// Запускает обработчик нового клиента.
        /// </summary>
        /// <param name="tcpClient"> Tcp-клиент подключения нового клиента. </param>
        /// <param name="addedClient"> Новый клиент. </param>
        /// <param name="tokenSource"> Токен отмены обработчика клиента. </param>
        private void StartNewClientThread(TcpClient tcpClient, ConnectedClient addedClient,
            CancellationTokenSource tokenSource)
        {
            var reader = new StreamReader(tcpClient.GetStream());
            TryFetchNewClient(tcpClient, addedClient, tokenSource, reader);

            ProcessNewMessages(tcpClient, addedClient, tokenSource, reader);
        }

        /// <summary>
        /// Обработывает входящие сообщения.
        /// </summary>
        /// <param name="tcpClient"> Tcp-клиент подключения нового клиента. </param>
        /// <param name="addedClient"> Новый клиент. </param>
        /// <param name="tokenSource"> Токен отмены обработчика клиента. </param>
        /// <param name="reader"> Обработчик потока на чтение. </param>
        private void ProcessNewMessages(TcpClient tcpClient, ConnectedClient addedClient,
            CancellationTokenSource tokenSource, StreamReader reader)
        {
            while (tcpClient.Connected)
            {
                try
                {
                    var message = reader.ReadLine();
                    tokenSource.CancelAfter(ReservedParams.USER_TIMEOUT);

                    switch (message)
                    {
                        case CommandNames.MESSAGE:
                            message = reader.ReadLine();
                            // Формат сообщения: отправитель, получатель, текст сообщения.
                            if (!int.TryParse(message, out var senderId))
                            {
                                Logger.GetInstance().LogMessage(
                                    $"Error: Unable to recognize user Id. \r\n User id: {message}");

                                continue;
                            }

                            message = reader.ReadLine();
                            if (int.TryParse(message, out var receiverId))
                            {
                                message = reader.ReadLine();
                                MessagesUtils.ProcessSendMessage(ConnectedClients, senderId, receiverId,
                                    message);
                            }

                            break;

                        case CommandNames.USER_DISCONNECTED:
                            var errorString = $"Username \"{addedClient.UserName}\" disconnected";
                            MessagesUtils.DisconnectUser(ConnectedClients, tcpClient, errorString, addedClient);

                            break;

                        default:
                            Logger.GetInstance().LogMessage($"Error: Unknown message type! \r\n {message}");

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().LogMessage($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Читает из потока данные о новом клиенте и регистрирует его.
        /// В случае успешной регистрации добавляет callback на закрытие подключения по истечении сессии.
        /// </summary>
        /// <param name="tcpClient"> Tcp-клиент подключения нового клиента. </param>
        /// <param name="addedClient"> Новый клиент. </param>
        /// <param name="tokenSource"> Токен отмены обработчика клиента. </param>
        /// <param name="reader"> Обработчик потока на чтение. </param>
        private void TryFetchNewClient(TcpClient tcpClient, ConnectedClient addedClient,
            CancellationTokenSource tokenSource, StreamReader reader)
        {
            while (tcpClient.Connected)
            {
                var line = reader.ReadLine();

                if (line != CommandNames.LOGIN)
                    continue;

                var userName = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    var errorString = $"Username \"{userName}\" incorrect";
                    MessagesUtils.DisconnectUser(ConnectedClients, tcpClient, errorString);
                    break;
                }

                if (ConnectedClients.Any(s => s.UserName == userName))
                {
                    var errorString = $"Username \"{userName}\" already exists";
                    MessagesUtils.DisconnectUser(ConnectedClients, tcpClient, errorString);
                    break;
                }

                RegisterNewClient(addedClient, userName, tcpClient);

                tokenSource.Token.Register(callback: ()
                    =>
                {
                    if (tcpClient.Connected)
                        MessagesUtils.DisconnectUser(ConnectedClients, tcpClient,
                            $"{addedClient.UserName} disconnected by timeout)", addedClient);
                });

                break;
            }
        }

        /// <summary>
        /// Регистрирует нового клиента на сервере.
        /// </summary>
        /// <param name="addedClient"> Добавленный клиент. </param>
        /// <param name="userName"> Имя пользователя. </param>
        /// <param name="tcpClient"> TcpClient нового клиента. </param>
        private void RegisterNewClient(ConnectedClient addedClient, string userName, TcpClient tcpClient)
        {
            _lastRegistredUserId++;
            addedClient.Id = _lastRegistredUserId;
            addedClient.UserName = userName;
            addedClient.Client = tcpClient;

            ThreadSafeAddClient(addedClient);
            Logger.GetInstance().LogMessage($"New connection: {userName}");
            MessagesUtils.SendUserId(addedClient);
            MessagesUtils.SendUsersList(ConnectedClients, addedClient);
            MessagesUtils.SendNewUserConnectedMessage(ConnectedClients, addedClient);
        }

        /// <summary>
        /// Потокобезопасно добавляет клиента в список.
        /// </summary>
        private void ThreadSafeAddClient(ConnectedClient client)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    if (ConnectedClients.All(x => x.UserName != client.UserName))
                        ConnectedClients.Add(client);
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}