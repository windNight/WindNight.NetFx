using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WindNight.ConfigCenter.Extension
{
    internal enum DomainSwitchNodeType
    {
        Unknown = 0,
        Root = 1,
        SiteHost = 2,
        ServiceUrl = 3,
    }

    internal class DomainSwitch
    {
        protected DomainSwitch()
        {
        }


        private static string DomainSwitchConfigPath => GetMapPath(XmlPath);

        private static bool IsExistDomainSwitchConfig => File.Exists(DomainSwitchConfigPath);

        private static string XmlPath =>
            ConfigItems.DomainSwitchConfigPath.TrimStart('/');

        private static string ConfigSolution =>
            ConfigItems.DomainSwitchSolution;

        private static string SiteHostNodeKey =>
            $"/{nameof(DomainSwitch)}/{GetSolution()}/{DomainSwitchNodeType.SiteHost}";

        private static string ServiceUrlNodeKey =>
            $"/{nameof(DomainSwitch)}/{GetSolution()}/{DomainSwitchNodeType.ServiceUrl}";

        private static XmlNodeList? GetNodeChildren(string node)
        {
            return GetInstance().SelectSingleNode(node)?.ChildNodes ?? null;
        }

        private static XmlDocument GetInstance()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(DomainSwitchConfigPath);
            return xmlDocument;
        }

        private static string GetMapPath(string strPath)
        {
            return AppDomain.CurrentDomain.BaseDirectory + strPath.Replace("/", "\\");
        }

        private static string GetNodeValue(string node)
        {
            var xmlNode = GetInstance().SelectSingleNode(node);
            if (xmlNode != null)
            {
                return xmlNode.InnerText;
            }

            return string.Empty;
        }


        private static string GetSolution()
        {
            if (ConfigSolution != string.Empty)
            {
                return ConfigSolution;
            }

            return GetInstance().SelectSingleNode(nameof(DomainSwitch))?.ChildNodes[0]?.Name ?? "";
        }

        public static string RplDomain(string content)
        {
            var currDomain = GetCurrDomain();
            return new Regex("http://([\\w]+)(" + GetAllDomain() + ")",
                RegexOptions.IgnoreCase | RegexOptions.Singleline).Replace(content, "http://$1" + currDomain);
        }

        public static string GetCurrDomain()
        {
            return GetInstance().SelectSingleNode("DomainSwitch/" + GetSolution())?.Attributes?["Domain"]?.Value
                ?.ToLower() ?? "";
        }

        public static string GetSetDomain()
        {
            return GetInstance().SelectSingleNode("DomainSwitch/" + GetSolution())?.Attributes?["SetDomain"]?.Value
                ?.ToLower() ?? "";
        }

        public static string GetAllDomain()
        {
            if (!IsExistDomainSwitchConfig)
            {
                return "";
            }

            var stringBuilder = new StringBuilder();
            var domainSwitch = GetInstance().SelectSingleNode(nameof(DomainSwitch));
            if (domainSwitch == null)
            {
                return string.Empty;
            }

            foreach (XmlNode childNode in domainSwitch.ChildNodes)
            {
                if (childNode == null)
                {
                    continue;
                }

                if (stringBuilder.Length == 0)
                {
                    stringBuilder.Append(childNode.Attributes?["Domain"]?.Value?.ToLowerInvariant() ?? "");
                }
                else
                {
                    stringBuilder.Append("|");
                    stringBuilder.Append(childNode.Attributes?["Domain"]?.Value?.ToLowerInvariant() ?? "");
                }
            }

            return stringBuilder.ToString();
        }

        public static Dictionary<string, string> GetAllDomainDict()
        {
            var dict = new Dictionary<string, string>();
            if (!IsExistDomainSwitchConfig)
            {
                return dict;
            }

            var domainSwitch = GetInstance().SelectSingleNode(nameof(DomainSwitch));
            if (domainSwitch == null)
            {
                return dict;
            }

            foreach (XmlNode childNode in domainSwitch.ChildNodes)
            {
                var node = childNode?.Attributes?["Domain"];
                if (node == null)
                {
                    continue;
                }

                dict.Add(node.Name, node.Value.ToLowerInvariant());
            }

            dict = dict.Union(GetAllSiteHosts()).Union(GetAllServiceUrls()).ToDictionary(k => k.Key, v => v.Value);
            return dict;
        }


        public static string GetSiteHost(string siteName)
        {
            return GetNodeValue($"{SiteHostNodeKey}/{siteName}");
        }


        public static string GetServiceUrl(string serviceName)
        {
            return GetNodeValue($"{ServiceUrlNodeKey}/{serviceName}");
        }


        public static Dictionary<string, string> GetAllSiteHosts()
        {
            var dict = new Dictionary<string, string>();
            if (!IsExistDomainSwitchConfig)
            {
                return dict;
            }

            var nodeChildren = GetNodeChildren(SiteHostNodeKey);
            if (nodeChildren == null)
            {
                return dict;
            }

            foreach (XmlNode xmlNode in nodeChildren)
            {
                if (xmlNode.NodeType == XmlNodeType.Element)
                {
                    dict.Add($"{DomainSwitchNodeType.SiteHost}:{xmlNode.Name}", xmlNode.InnerText);
                }
            }

            return dict;
        }

        public static Dictionary<string, string> GetAllServiceUrls()
        {
            var dict = new Dictionary<string, string>();
            if (!IsExistDomainSwitchConfig)
            {
                return dict;
            }

            var nodeChildren = GetNodeChildren(ServiceUrlNodeKey);
            if (nodeChildren == null)
            {
                return dict;
            }

            foreach (XmlNode xmlNode in nodeChildren)
            {
                if (xmlNode.NodeType == XmlNodeType.Element)
                {
                    dict.Add($"{DomainSwitchNodeType.ServiceUrl}:{xmlNode.Name}", xmlNode.InnerText);
                }
            }

            return dict;
        }

        internal class ConfigItems : ConfigItemsBase
        {
            public static string DomainSwitchConfigPath
                => GetAppSetting(ConstKey.DomainSwitchConfigPathKey, "/Config/DomainSwitch.config");

            public static string DomainSwitchSolution
                => GetAppSetting(ConstKey.DomainSwitchSolutionKey, "Solution1");

            internal static class ConstKey
            {
                public const string DomainSwitchConfigPathKey = "DomainSwitchConfigPath";
                public const string DomainSwitchSolutionKey = "DomainSwitchSolution";
            }
        }
    }
}
