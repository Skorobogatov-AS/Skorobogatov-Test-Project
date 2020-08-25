using System;
using System.Net.NetworkInformation;
using System.Windows.Data;
using Server.Utils;

namespace Server.Converters
{
    /// <summary>
    /// Конвертирует NetworkInterface в формат "Описание интерфенйса + ip-адрес". 
    /// </summary>
    public class NetworkInterfaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is NetworkInterface networkInterface))
                return null;

            var adress = ServerUtils.GetIpv4Adress(networkInterface);

            return $"{networkInterface.Description} ({adress})";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}