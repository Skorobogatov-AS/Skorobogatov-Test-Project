using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using CommonUtils.Commands;
using Server.Annotations;
using Server.Models;

namespace Server.ViewModels
{
    /// <summary>
    /// Вью модель сервера.
    /// </summary>
    public class ServerVm : INotifyPropertyChanged
    {

        private int _port;

        /// <summary>
        /// Модель сервера.
        /// </summary>
        private ServerModel _serverModel;

        public string ServerMessages => _serverModel.ServerMessages;

        public bool IsRunning { get; set; }

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
        public ObservableCollection<ConnectedClient> ConnectedClients => _serverModel.ConnectedClients;

        /// <summary>
        /// Коллекция сетевых интерфейсов.
        /// </summary>
        public ObservableCollection<NetworkInterface> NetworkInterfaces => _serverModel.NetworkInterfaces;

        /// <summary>
        /// <inheritdoc cref="ServerVm"/>
        /// </summary>
        public ServerVm()
        {
            Port = 8080;
            _serverModel = new ServerModel();
            _serverModel.PropertyChanged += (sender, args) => { OnPropertyChanged(args.PropertyName); };
        }

        /// <summary>
        /// Команда запуска сервера.
        /// </summary>
        public AsyncCommand StartServerCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        _serverModel.SelectedInterface = SelectedInterface;
                        _serverModel.Port = Port;
                        _serverModel.StartServer();
                    });
                }, () => !_serverModel.IsRunning && SelectedInterface != null);
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
