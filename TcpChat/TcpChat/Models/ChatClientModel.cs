using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommonUtils;
using TcpChat.Annotations;
using TcpChat.ViewModels;

namespace TcpChat.Models
{
    /// <summary>
    /// Модель чата.
    /// </summary>
    public sealed class ChatClientModel : INotifyPropertyChanged
    {
        /// <summary>
        /// <see cref="Ip"/>
        /// </summary>
        private string _ip;

        /// <summary>
        /// <see cref="Port"/>
        /// </summary>
        private int _port;

        /// <summary>
        /// <see cref="UserName"/>
        /// </summary>
        private string _userName;

        /// <summary>
        /// <see cref="Message"/>
        /// </summary>
        private string _message;

        /// <summary>
        /// <see cref="ActiveClient"/>
        /// </summary>
        private ServerClient _activeClient;

        /// <summary>
        /// Клиент пользовательского подключения.
        /// </summary>
        public TcpClient TcpClient;

        /// <summary>
        /// Поток для чтения.
        /// </summary>
        private StreamReader _reader;

        /// <summary>
        /// Поток для записи.
        /// </summary>
        private StreamWriter _writer;

        /// <summary>
        /// Отправитель сигнала отмены.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Id пользователя.
        /// </summary>
        private int _userId;

        /// <summary>
        /// Ip адрес подключения.
        /// </summary>
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Порт подключения.
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
        /// Имя пользователя.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущее сообщение.
        /// </summary>
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Клиент, которому будет отсылаться сообщение.
        /// </summary>
        public ServerClient ActiveClient
        {
            get => _activeClient;
            set
            {
                _activeClient = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Клиент, которому будет отсылаться сообщение.
        /// </summary>
        public ObservableCollection<ServerClient> ChatUsers { get; }

        /// <summary>
        /// Модель клиента чата.
        /// </summary>
        public ChatClientModel()
        {
            var generalСhat = new ServerClient(0, "All");
            ChatUsers = new ObservableCollection<ServerClient> { generalСhat };
            ActiveClient = generalСhat;
        }
        
        /// <summary>
        /// Отправка сообщения.
        /// Формат сообщения: тип команды, отправитель, получатель, сообщение.
        /// </summary>
        public void SendMessage()
        {
            var builder = new StringBuilder(CommandNames.MESSAGE);
            builder.AppendLine();
            builder.AppendLine(_userId.ToString()); // Sender.
            builder.AppendLine(ActiveClient.Id.ToString()); // Receiver.
            var message = $"{DateTime.Now.ToLongTimeString()}: {UserName}: {Message}";
            builder.Append(message);

            _writer.WriteLine(builder.ToString());

            ActiveClient.Chat += $"{message}\n";
            Message = string.Empty;
        }
        
        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        public void Connect()
        {
            try
            {
                ProcessClient();
                TcpClient = new TcpClient();
                TcpClient.Connect(Ip, Port);
                _reader = new StreamReader(TcpClient.GetStream());
                _writer = new StreamWriter(TcpClient.GetStream())
                {
                    AutoFlush = true
                };

                var ds = new StringBuilder(CommandNames.LOGIN);
                ds.AppendLine();
                ds.Append(UserName);

                _writer.WriteLine(ds.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Отключение от сервера.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                var builder = new StringBuilder(CommandNames.USER_DISCONNECTED);
                _writer?.WriteLine(builder.ToString());
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Запускает чтение клиентом входящего потока.
        /// </summary>
        private void ProcessClient()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(() => { TcpClient.Close(); });

            Task.Factory.StartNew(ProcessReadMessages, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Читает сообщения из входящего потока.
        /// </summary>
        private void ProcessReadMessages()
        {
            while (true)
            {
                try
                {
                    if (TcpClient == null || !TcpClient.Connected || _reader == null)
                        continue;

                    var line = _reader.ReadLine();

                    if (line != null)
                    {
                        switch (line)
                        {
                            case CommandNames.CONNECT:
                                line = _reader.ReadLine();
                                if (int.TryParse(line, out var newId))
                                    _userId = newId;

                                break;

                            case CommandNames.MESSAGE:
                                FetchMessage();

                                break;

                            case CommandNames.SEND_USER_LIST:
                                line = _reader.ReadLine();
                                if (!int.TryParse(line, out var usersCount))
                                    continue;

                                for (var i = 0; i < usersCount; i++)
                                    ParseUserAndAddToChatUsers();

                                break;

                            case CommandNames.NEW_USER_CONNECTED:
                                ParseUserAndAddToChatUsers();

                                break;

                            case CommandNames.USER_DISCONNECTED:
                                RemoveDisconnectedUser();

                                break;

                            case CommandNames.DISCONNECT:
                                line = _reader.ReadLine();
                                ChatUsers.First().Chat += $"{line}\n";
                                break;
                        }
                    }

                    else
                    {
                        TcpClient.Close();
                        ChatUsers.First().Chat += "ConnectedError" + "\n";
                        _cancellationTokenSource.Cancel();
                    }

                    Task.Delay(100).Wait();
                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Получает Id пользователя из потока и удаляет пользователя с таким Id из списка. 
        /// </summary>
        private void RemoveDisconnectedUser()
        {
            var line = _reader.ReadLine();

            if (line == null)
                return;

            var substring = line.Substring(0, ReservedParams.USERS_DIGIT_CAPACITY);
            var id = int.Parse(substring);
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var disconnectedUser = ChatUsers.FirstOrDefault(x => x.Id == id);
                    ChatUsers.Remove(disconnectedUser);
                });
        }

        /// <summary>
        /// Получает сообщение.
        /// </summary>
        private void FetchMessage()
        {
            var line = _reader.ReadLine();
            if (!int.TryParse(line, out var senderId))
                return;

            var chatUser = ChatUsers.FirstOrDefault(x => x.Id == senderId);
            line = _reader.ReadLine();

            if (chatUser != null)
                chatUser.Chat += $"{line}\n";
        }

        /// <summary>
        /// Получает пользователя из потока и добавляет его в список пользователей.
        /// </summary>
        private void ParseUserAndAddToChatUsers()
        {
            var line = _reader.ReadLine();

            if (line == null)
                return;

            var connectedUser = ParseClient(line);
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    if (ChatUsers.All(x => x.UserName != connectedUser.UserName))
                        ChatUsers.Add(connectedUser);
                });
        }

        /// <summary>
        /// Получает из строки id и имя пользователя.
        /// </summary>
        /// <param name="line"> Строка. </param>
        /// <returns> Пользователь. </returns>
        private static ServerClient ParseClient(string line)
        {
            var substring = line.Substring(0, ReservedParams.USERS_DIGIT_CAPACITY);
            var id = int.Parse(substring);
            var userName = line.Substring(ReservedParams.USERS_DIGIT_CAPACITY);
            var newUser = new ServerClient(id, userName);

            return newUser;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}