using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.Redis
{
    public class RedisOption
    {
        public string Host { get; set; }
        public int Port { get; set; } = 6379;
        public string Password { get; set; }

        public int DefaultDb { get; set; }

        public string PrefixKey { get; set; }

        public bool Ssl { get; set; } = false;

        public static RedisOption BuildRedisOption(string conn)
        {
            var op = new RedisOption();
            var vs = Regex.Split(conn, @"\,([\w \t\r\n]+)=", RegexOptions.Multiline);
            SetHost(op, vs[0]);
            for (var a = 1; a < vs.Length; a += 2)
            {
                var kv = new[] { vs[a].ToLower().Trim(), vs[a + 1] };
                switch (kv[0])
                {
                    case "password":
                        op.Password = kv.Length > 1 ? kv[1] : "";
                        break;
                    case "prefix":
                        op.PrefixKey = kv.Length > 1 ? kv[1] : "";
                        break;
                    case "defaultdatabase":
                        op.DefaultDb = int.TryParse(kv.Length > 1 ? kv[1].Trim() : "0", out var _database) ? _database : 0;
                        break;
                    case "ssl":
                        op.Ssl = kv.Length > 1 && kv[1].ToLower().Trim() == "true";
                        break;
                    default:
                        break;
                }
            }
            return op;
        }

        internal static void SetHost(RedisOption op, string host)
        {
            if (string.IsNullOrEmpty(host?.Trim()))
            {
                op.Host = "127.0.0.1";
                // op.Port = 6379;
                return;
            }

            host = host.Trim();
            var ipv6 = Regex.Match(host, @"^\[([^\]]+)\]\s*(:\s*(\d+))?$");
            if (ipv6.Success) //ipv6+port 格式： [fe80::b164:55b3:4b4f:7ce6%15]:6379
            {
                op.Host = ipv6.Groups[1].Value.Trim();
                op.Port = int.TryParse(ipv6.Groups[3].Value, out var tryint) && tryint > 0 ? tryint : 6379;
                return;
            }

            var spt = (host ?? "").Split(':');
            if (spt.Length == 1) //ipv4 or domain
            {
                op.Host = string.IsNullOrEmpty(spt[0].Trim()) == false ? spt[0].Trim() : "127.0.0.1";
                // op.Port = 6379;
                return;
            }

            if (spt.Length == 2) //ipv4:port or domain:port
            {
                if (int.TryParse(spt.Last().Trim(), out var testPort2))
                {
                    op.Host = string.IsNullOrEmpty(spt[0].Trim()) == false ? spt[0].Trim() : "127.0.0.1";
                    op.Port = testPort2;
                    return;
                }

                op.Host = host;
                //op.Port = 6379;
                return;
            }

            if (IPAddress.TryParse(host, out var tryip) &&
                tryip.AddressFamily == AddressFamily.InterNetworkV6) //test ipv6
            {
                op.Host = host;
                // op.Port = 6379;
                return;
            }

            if (int.TryParse(spt.Last().Trim(), out var testPort)) //test ipv6:port
            {
                var testHost = string.Join(":", spt.Where((a, b) => b < spt.Length - 1));
                if (IPAddress.TryParse(testHost, out tryip) && tryip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    op.Host = testHost;
                    // op.Port = 6379;
                    return;
                }
            }

            op.Host = host;
            //op.Port = 6379;

        }

        public override string ToString()
        {
            return $"Host={Host},Port={Port},Password={Password},DefaultDb={DefaultDb},PrefixKey={PrefixKey},Ssl={Ssl}";
        }

    }
}
