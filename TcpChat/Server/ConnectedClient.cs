using System.Net.Sockets;

namespace Server
{
    /// <summary>
    /// Клиент.
    /// </summary>
    public class ConnectedClient
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TcpClient Client { get; set; }
    }
}
