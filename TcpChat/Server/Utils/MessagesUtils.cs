using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommonUtils;

namespace Server.Utils
{
    /// <summary>
    /// Методы работы с сообщениями сервера.
    /// </summary>
    public static class MessagesUtils
    {
        /// <summary>
        /// Отправляет сообщение клиенту.
        /// </summary>
        /// <param name="reply"> Сообщение. </param>
        /// <param name="client"> Клиент. </param>
        private static void SendReqly(string reply, ConnectedClient client)
        {
            var streamWriter = new StreamWriter(client.Client.GetStream()) {AutoFlush = true};
            streamWriter.WriteLine(reply);
        }

        /// <summary>
        /// Отправляет текстовое сообщение клиенту.
        /// </summary>
        /// <param name="userId"> Id отправителя. </param>
        /// <param name="message"> Сообщение. </param>
        /// <param name="client"> Клиент. </param>
        private static void SendTextMessage(int userId, string message, ConnectedClient client)
        {
            var formattedString = FormatMessageHeader(CommandNames.MESSAGE);
            formattedString.AppendLine(userId.ToString());
            formattedString.Append(message);

            SendReqly(formattedString.ToString(), client);
        }

        /// <summary>
        /// Отправляет список поключенных пользователей.
        /// </summary>
        /// <param name="connectedClients"> Подключенные пользователи. </param>
        /// <param name="receiver"> Клиент. </param>
        public static void SendUsersList(ObservableCollection<ConnectedClient> connectedClients,
            ConnectedClient receiver)
        {
            var formattedString = FormatMessageHeader(CommandNames.SEND_USER_LIST);
            formattedString.Append(connectedClients.Count.ToString());

            foreach (var user in connectedClients)
            {
                formattedString.AppendLine();
                formattedString.Append($"{user.Id,-ReservedParams.USERS_DIGIT_CAPACITY}{user.UserName}");
            }

            SendReqly(formattedString.ToString(), receiver);
        }

        /// <summary>
        /// Отправляет Id новому клиенту.
        /// </summary>
        /// <param name="client"> Клиент. </param>
        public static void SendUserId(ConnectedClient client)
        {
            var formattedString = FormatMessageHeader(CommandNames.CONNECT);
            formattedString.Append(client.Id);

            SendReqly(formattedString.ToString(), client);
        }

        /// <summary>
        /// Форматирует заголовок сообщения для отправки.
        /// </summary>
        /// <param name="messageHeader"> Имя команды. </param>
        /// <returns> Отформатированный заголовок. </returns>
        private static StringBuilder FormatMessageHeader(string messageHeader)
        {
            var formattedString = new StringBuilder(messageHeader);
            formattedString.AppendLine();

            return formattedString;
        }

        /// <summary>
        /// Сообщает всем клиентам о новом подключенном клиенте.
        /// </summary>
        /// <param name="connectedClients"> Подключенные пользователи. </param>
        /// <param name="newClient"> Новый клиент. </param>
        public static void SendNewUserConnectedMessage(ObservableCollection<ConnectedClient> connectedClients,
            ConnectedClient newClient)
        {
            NotifyAll(connectedClients, newClient, CommandNames.NEW_USER_CONNECTED);
        }

        /// <summary>
        /// Закрывает подключение пользователя.
        /// Если пользователь был зарегистрирован,
        /// удаляет его из списка активных пользователей и рассылает всем остальным клиентам сообщение об отключении пользователя.
        /// </summary>
        /// <param name="connectedClients"> Подключенные пользователи. </param>
        /// <param name="tcpClient"> Подключение удаляемого пользователя. </param>
        /// <param name="message"> Cообщение пользователю при отключении. </param>
        /// <param name="client"> Клиент удаляемого пользователя. </param>
        public static void DisconnectUser(ObservableCollection<ConnectedClient> connectedClients, TcpClient tcpClient,
            string message, ConnectedClient client = null)
        {
            var formattedString = FormatMessageHeader(CommandNames.DISCONNECT);
            formattedString.Append(message);

            if (tcpClient.Connected)
            {
                var streamWriter = new StreamWriter(tcpClient.GetStream()) {AutoFlush = true};
                streamWriter.WriteLine(formattedString);
                tcpClient.Client.Close();
            }

            var unregistredClient = client == null;

            if (unregistredClient)
                return;

            Application.Current.Dispatcher.Invoke(
                () => { connectedClients.Remove(client); });

            NotifyAll(connectedClients, client, CommandNames.USER_DISCONNECTED);
            Logger.GetInstance().LogMessage($"{CommandNames.USER_DISCONNECTED} {client.UserName}");
        }

        /// <summary>
        /// Оповещает всех подключенных пользователей о событии с клиентом.
        /// </summary>
        /// <param name="connectedClients"> Подключенные пользователи. </param>
        /// <param name="client"> Клиент, с которым произошло событие. </param>
        /// <param name="commandName"> Событие. </param>
        private static void NotifyAll(ObservableCollection<ConnectedClient> connectedClients, ConnectedClient client,
            string commandName)
        {
            var formattedString = FormatMessageHeader(commandName);
            formattedString.Append($"{client.Id,-ReservedParams.USERS_DIGIT_CAPACITY}{client.UserName}");

            foreach (var userName in connectedClients)
            {
                try
                {
                    if (userName.Client.Connected)
                        SendReqly(formattedString.ToString(), userName);
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().LogMessage($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Запускает процесс отправки сообщения.
        /// </summary>
        /// <param name="connectedClients"> Подключенные пользователи. </param>
        /// <param name="senderId"> Id отправителя. </param>
        /// <param name="receiverId"> Id получателя. </param>
        /// <param name="message"> Текст сообщения. </param>
        public static async void ProcessSendMessage(ObservableCollection<ConnectedClient> connectedClients,
            int senderId, int receiverId, string message)
        {
            await Task.Factory.StartNew(() =>
            {
                var senderName = connectedClients.FirstOrDefault(x => x.Id == senderId)?.UserName;
                var receiverName = connectedClients.FirstOrDefault(x => x.Id == receiverId)?.UserName ??
                                   "All users";

                Logger.GetInstance()
                    .LogMessage(
                        $"{CommandNames.MESSAGE}: from \"{senderName}\" to \"{receiverName}\" message text :\"{message}\"");

                if (receiverId == 0)
                    foreach (var client in connectedClients.Where(x => x.Id != senderId).ToList())
                        SendMessageToClient(0, message, client);
                else
                {
                    var receiverClient = connectedClients.FirstOrDefault(x => x.Id == receiverId);
                    SendMessageToClient(senderId, message, receiverClient);
                }
            });
        }

        /// <summary>
        /// Отправляет сообщение.
        /// </summary>
        /// <param name="senderId"> Id отправителя. </param>
        /// <param name="message"> Сообщение. </param>
        /// <param name="client"> Получатель сообщения. </param>
        private static void SendMessageToClient(int senderId, string message, ConnectedClient client)
        {
            try
            {
                if (client.Client.Connected)
                    SendTextMessage(senderId, message, client);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().LogMessage($"Error: {ex.Message}");
            }
        }
    }
}