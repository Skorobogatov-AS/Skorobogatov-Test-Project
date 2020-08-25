using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommonUtils.Commands;
using TcpChat.Annotations;
using TcpChat.Models;

namespace TcpChat.ViewModels
{
    /// <summary>
    /// ViewModel клиента чата.
    /// </summary>
    public sealed class ChatClientVm : INotifyPropertyChanged
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
        /// Модель клиента чата.
        /// </summary>
        private readonly ChatClientModel _clientModel;

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
        /// Коллекция подключенных клиентов.
        /// </summary>
        public ObservableCollection<ServerClient> ChatUsers => _clientModel.ChatUsers;

        /// <summary>
        /// Отдельное представление списка пользователей для табов. Без текущего пользователя.
        /// </summary>
        public ObservableCollection<ServerClient> ChatTabs =>
            new ObservableCollection<ServerClient>(from item in ChatUsers where item.UserName != UserName select item);

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
        /// ViewModel клиента чата.
        /// </summary>
        public ChatClientVm()
        {
            Ip = "127.0.0.1";
            Port = 8080;
            UserName = "NewUser";
            _clientModel = new ChatClientModel();
            _clientModel.PropertyChanged += (sender, args) => { OnPropertyChanged(args.PropertyName); };
            ChatUsers.CollectionChanged += ChatUsers_CollectionChanged;
        }

        /// <summary>
        /// Отдельное представление списка пользователей для табов. Без текущего пользователя.
        /// Вместо него можно было бы сделать нормальную фильтрацию, но не в этом тестовом задании.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatUsers_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ChatTabs));
        }

        /// <summary>
        /// Делает активной вкладку чата выбранного пользователя.
        /// </summary>
        public AsyncCommand StartChatCommand
        {
            get
            {
                return new AsyncCommand(id =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        _clientModel.ActiveClient = ChatUsers.FirstOrDefault(x => x.Id == (int) id);
                    });
                });
            }
        }

        /// <summary>
        /// Команда подключения к серверу.
        /// </summary>
        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        _clientModel.Ip = Ip;
                        _clientModel.Port = Port;
                        _clientModel.UserName = UserName;
                        _clientModel.Connect();
                    });
                }, () => _clientModel.TcpClient == null || _clientModel.TcpClient?.Connected == false);
            }
        }

        /// <summary>
        /// Команда отключения от сервера.
        /// </summary>
        public AsyncCommand DisconnectCommand
        {
            get
            {
                return new AsyncCommand(() => { return Task.Factory.StartNew(() => { _clientModel.Disconnect(); }); });
            }
        }

        /// <summary>
        /// Команда отправки сообщения.
        /// </summary>
        public AsyncCommand SendMessageCommand
        {
            get
            {
                return new AsyncCommand(() =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                _clientModel.ActiveClient = ActiveClient;
                                _clientModel.Message = Message;
                                _clientModel.SendMessage();
                                Message = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        });
                    },
                    () =>
                        _clientModel.TcpClient?.Connected == true && !string.IsNullOrWhiteSpace(Message));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}