using System;

namespace WindNight.Core.Extension
{
    public static class VersionExtensions
    {
        //private const string MAX_VERSION = "99.99.9999999999";
        //public const long MAX_VERSION_NO = 99999999999999;
        //private const long VERSION_HX = 1000000000000;
        //private const string MaxVersionString = "*";

        /// <summary>
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static VersionStruct Convert2Struct(this string version)
        {
            try
            {
                return new VersionStruct { Version = version };
            }
            catch// (Exception ex)
            {
                return VersionStruct.NullVersionStruct;
            }
        }

        public static bool IsMax(this string version)
        {
            return string.Equals(version, VersionStruct.AllVersion, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(version, VersionStruct.MaxVersion, StringComparison.OrdinalIgnoreCase);
        }

        public static long Convert2Long(this string? version, long defaultVersion = 0L, bool isThrow = false)
        {
            return version.Convert2Long(out var firstNumber, defaultVersion, isThrow);
        }

        public static long Convert2Long(this string? version, out int firstNumber, long defaultVersion = 0L,
            bool isThrow = true)
        {
            firstNumber = 0;
            try
            {
                if (string.IsNullOrEmpty(version) || version == "0") return defaultVersion;
                if (version.Trim() == "*") return VersionStruct.MaxVersionLong;
                long.TryParse(version.Replace(".", ""), out var result);
                if (result == 0L) return defaultVersion;
                var strArray = version.Split('.');
                firstNumber = strArray[0].ToInt();
                if (strArray.Length == 3)
                    return long.Parse(strArray[0] + strArray[1].PadLeft(4, '0') + strArray[2].PadLeft(8, '0'));
                if (strArray.Length > 3)
                    return long.Parse(strArray[0] + strArray[1].PadLeft(4, '0') + strArray[2].PadLeft(2, '0') +
                                      strArray[3].PadLeft(6, '0'));
                if (strArray.Length == 2)
                    return long.Parse(strArray[0] + strArray[1].PadRight(12, '0'));
                return long.Parse(strArray[0].PadRight(14, '0'));
            }
            catch (Exception ex)
            {
                if (isThrow)
                    throw new ArgumentException($"{version} 版本转换为数字错误", ex);
                return defaultVersion;
            }
        }
    }

    public struct VersionStruct
    {
        public const string AllVersion = "*";
        public const string MaxVersion = "99.99.9999999999";
        public const long MaxVersionLong = 99999999999999L;
        public const string MinVersion = "1.0.0";
        public const long MinVersionLong = 1000000000000;


        public string? Version { get; set; }
        public long VersionLong => Version.Convert2Long();

        public static VersionStruct NullVersionStruct => new VersionStruct { Version = "0" };

        public static bool operator ==(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null || left.VersionLong == 0L) return false;
            return left.VersionLong == right.VersionLong;
        }

        public static bool operator !=(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null) return false;
            return left.VersionLong != right.VersionLong;
        }

        public static bool operator >(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null) return false;
            return left.VersionLong > right.VersionLong;
        }

        public static bool operator <(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null) return false;
            return left.VersionLong < right.VersionLong;
        }

        public static bool operator >=(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null) return false;
            return left.VersionLong >= right.VersionLong;
        }

        public static bool operator <=(VersionStruct left, VersionStruct right)
        {
            if (left == null || right == null) return false;
            return left.VersionLong <= right.VersionLong;
        }


        public override bool Equals(object obj)
        {
            if (obj is VersionStruct objVersionStruct)
            {
                return this == objVersionStruct;

            }
            return false;
        }

        public override string ToString()
        {
            return $"{Version}";
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
        

    }
}