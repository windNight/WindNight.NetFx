using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WindNight.Core.Extension
{
    public static class IPExtension
    {

        public static bool IsPrivateOrLoopback(this string ip)
        {
            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                return ipAddress.IsPrivateOrLoopback();
            }

            return false;
        }

        public static bool IsPrivateOrLoopback(this IPAddress ip)
        {
            // 判断是否是回环地址
            if (ip.IsLoopback())
            {
                return true;
            }

            var checkIp = ip;
            if (checkIp?.IsIPv4MappedToIPv6 ?? false)
            {
                checkIp = ip.MapToIPv4();
            }
            if (checkIp.AddressFamily == AddressFamily.InterNetwork)
            {
                return IsPrivateIPv4(checkIp);
            }

            if (checkIp.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return IsIPv6ULA(checkIp) || checkIp.IsIPv6LinkLocal;
            }

            //// 判断是否是内网地址
            //var bytes = checkIp.GetAddressBytes();
            //if (bytes[0] == 10)// 10.0.0.0/8
            //{
            //    return true;
            //}
            //if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) // 172.16.0.0/12
            //{
            //    return true;
            //}
            //if (bytes[0] == 192 && bytes[1] == 168)// 192.168.0.0/16
            //{
            //    return true;
            //}

            return false;
        }

        public static string IpV6ToIpV4(this string ip)
        {
            if (ip.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                var ipV4 = ipAddress.IpV6ToIpV4();
                return ipV4.ToString();
            }


            return ip;
        }

        public static IPAddress IpV6ToIpV4(this IPAddress ipAddress)
        {
            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            return ipAddress;
        }

        private static bool IsIpAllowed(IPAddress ipAddress)
        {

            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                return IsPrivateIPv4(ipAddress) || ipAddress.IsLoopback();
            }
            else if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return IsIPv6ULA(ipAddress) || ipAddress.IsIPv6LinkLocal || ipAddress.IsLoopback();
            }

            return false;
        }

        private static bool IsPrivateIPv4(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();
            return bytes[0] switch
            {
                0 => true,// 0.0.0.0
                10 => true,
                172 when bytes[1] >= 16 && bytes[1] <= 31 => true,
                192 when bytes[1] == 168 => true,
                _ => false,
            };
        }

        private static bool IsIPv6ULA(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();
            return bytes.Length > 0 && (bytes[0] == 0xFC || bytes[0] == 0xFD);
        }

        public static bool IsLoopback(this IPAddress? ip)
        {
            if (ip == null)
            {
                return false;
            }

            return IPAddress.IsLoopback(ip);
        }





    }
}
