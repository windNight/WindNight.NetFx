using System;
using System.Collections.Generic;
using System.Text;
using WindNight.Core.Extension;

namespace WindNight.Core.Abstractions
{

    public class DomainConfigs
    {
        public IEnumerable<DomainConfigDto> Items { get; set; } = new List<DomainConfigDto>();


        public string QueryDomainConfig(string domainName)
        {
            var config = QueryDomainInfoConfig(domainName);
            return config?.Domain ?? "";
        }

        public DomainConfigDto QueryDomainInfoConfig(string domainName)
        {
            var config = Items.FirstOrDefault(m => m.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase));
            return config;
        }

        public string GetDomainExtValue(string domainName, string key, string defaultValue = "", bool isThrow = false)
        {
            var config = QueryDomainInfoConfig(domainName);
            if (isThrow && config == null)
            {
                throw new ArgumentNullException(domainName, $"DomainConfig({domainName}) is Null");
            }

            try
            {
                var value = config.GetValueInExtension(key, defaultValue);

                return value;

            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }

        }

        public bool GetDomainExtValue(string domainName, string key, bool defaultValue = false, bool isThrow = false)
        {
            var config = QueryDomainInfoConfig(domainName);
            if (isThrow && config == null)
            {
                throw new ArgumentNullException(domainName, $"DomainConfig({domainName}) is Null");
            }

            try
            {
                var value = config.GetValueInExtension(key, defaultValue);

                return value;

            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }

        }

        public int GetDomainExtValue(string domainName, string key, int defaultValue = 0, bool isThrow = false)
        {
            var config = QueryDomainInfoConfig(domainName);
            if (isThrow && config == null)
            {
                throw new ArgumentNullException(domainName, $"DomainConfig({domainName}) is Null");
            }

            try
            {
                var value = config.GetValueInExtension(key, defaultValue);

                return value;

            }
            catch (Exception ex)
            {
                if (isThrow)
                {
                    throw;
                }

                return defaultValue;
            }

        }

    }

    public partial class DomainConfigDto
    {
        public string Name { get; set; } = "";

        public string Domain { get; set; } = "";

        public string Remark { get; set; } = "";

        public IReadOnlyDictionary<string, string> Extension { get; set; } = new Dictionary<string, string>();

    }


    public partial class DomainConfigDto
    {

        public string GetValueInExtension(string key, string defaultValue = "")
        {
            var extInfo = Extension.SafeGetValue(key, null);

            if (extInfo != null)
            {
                return extInfo;
            }

            return defaultValue;
        }

        public int GetValueInExtension(string key, int defaultValue = 0)
        {
            var configValue = GetValueInExtension(key, null);
            if (configValue == null)
            {
                return defaultValue;

            }

            return configValue.ToInt(defaultValue);
        }

        public bool GetValueInExtension(string key, bool defaultValue = false)
        {
            var configValue = GetValueInExtension(key, "");
            if (configValue.IsNullOrEmpty())
            {
                return defaultValue;
            }

            return configValue.ToBoolean(defaultValue);
        }

    }



}
