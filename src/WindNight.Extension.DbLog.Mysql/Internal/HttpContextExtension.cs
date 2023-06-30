using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace WindNight.Extension.Logger.DbLog.Internal
{
    internal static class HttpContextExtension
    {
        private const string DefaultIP = "0.0.0.0";

        /// <summary>
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        public static bool IsDefaultIp(string ipStr)
        {
            return ipStr == "127.0.0.1" || ipStr == "0.0.0.0" || ipStr == "::1";
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalIPs()
        {
            try
            {
                var validAddressFamilies = new[] { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 };
                var ips = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(m => m.OperationalStatus == OperationalStatus.Up)
                    .Select(m => m.GetIPProperties().UnicastAddresses)
                    .FirstOrDefault()
                    .Where(m => validAddressFamilies.Contains(m.Address.AddressFamily))
                    .Select(m => m.Address.ToString());
                return ips;
            }
            catch
            {
                return new[] { "0.0.0.0" };
            }
        }
    }
}
