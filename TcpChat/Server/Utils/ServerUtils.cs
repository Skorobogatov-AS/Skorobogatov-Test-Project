using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Server.Utils
{
    /// <summary>
    /// Вспомогательные методы сервера.
    /// </summary>
    public static class ServerUtils
    {
        /// <summary>
        /// Получает Ipv4 адрес сетевого интерфейса.
        /// </summary>
        /// <param name="networkInterface"> Сетевой интерфейс. </param>
        /// <returns> IP-адрес. </returns>
        public static IPAddress GetIpv4Adress(NetworkInterface networkInterface)
        {
            var ipAddressInformation = networkInterface.GetIPProperties()
                .UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);
            var adress = ipAddressInformation?.Address;

            return adress;
        }
    }
}