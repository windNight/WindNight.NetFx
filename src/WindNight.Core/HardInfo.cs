using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Extension;
using Newtonsoft.Json.Extension;

namespace System
{
    /// <summary>硬件信息</summary>
    public static class HardInfo
    {

        public static string NodeCode { get; private set; }

        public static string NodeIpAddress { get; private set; }

        public static void InitHardInfo(string nodeCode = "", string ip = "")
        {
            if (string.IsNullOrEmpty(nodeCode))
                nodeCode = GuidHelper.GenerateOrderNumber();
            if (string.IsNullOrEmpty(ip))
            {
                ip = string.Join(",", GetLocalIps());
            }
            NodeCode = nodeCode;
            NodeIpAddress = ip;
        }

        private const string DefaultIp = "0.0.0.0";

        public static string GetLocalIp()
        {
            return GetLocalIps().FirstOrDefault();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalIps()
        {
            try
            {
                var validAddressFamilies = new List<AddressFamily> { AddressFamily.InterNetwork, AddressFamily.InterNetworkV6 };
                var unicastAddresses = NetworkInterface.GetAllNetworkInterfaces()?
                    .Where(m => m.OperationalStatus == OperationalStatus.Up)?
                    .Select(m => m.GetIPProperties().UnicastAddresses);

                var ipList = (from unicastAddress in unicastAddresses
                              from unicastIpAddress in unicastAddress
                              where unicastIpAddress != null
                              where unicastIpAddress.IsDnsEligible
                              where validAddressFamilies.Contains(unicastIpAddress.Address.AddressFamily)
                              select unicastIpAddress.Address.ToString()).ToList();

                if (!ipList.Any())
                {
                    ipList.Add(DefaultIp);
                }
                return ipList;
            }
            catch
            {
                return new[] { DefaultIp };
            }
        }

        public static string ToString()
        {
            return new { NodeCode = NodeCode, NodeIpAddress = NodeIpAddress }.ToJsonStr();
        }
    }

}