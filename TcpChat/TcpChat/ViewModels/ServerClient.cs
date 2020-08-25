using System.ComponentModel;
using System.Runtime.CompilerServices;
using TcpChat.Annotations;

namespace TcpChat.ViewModels
{
    /// <summary>
    /// Подключенный к серверу пользователь
    /// </summary>
    public class ServerClient : INotifyPropertyChanged
    {
        /// <summary>
        /// Сообщения.
        /// </summary>
        private string _chat;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Сообщения в чате.
        /// </summary>
        public string Chat
        {
            get => _chat;
            set
            {
                _chat = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Подключенный к серверу пользователь.
        /// </summary>
        /// <param name="id"> Идентификатор пользователя. </param>
        /// <param name="userName"> Имя пользователя. </param>
        public ServerClient(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}