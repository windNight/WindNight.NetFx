namespace WindNight.Core.Abstractions
{
    public enum OperatorSysEnum
    {
        Unknown = 0,
        Windows = 1,
        Unix = 2,
        MacOSX = 3,
        XBox = 4,
    }
}

namespace WindNight.Core.Abstractions.Ex
{
    public static class EnumEx
    {
        public static string ToName(this OperatorSysEnum m)
        {
            var name = m switch
            {
                OperatorSysEnum.Windows => "Windows",
                OperatorSysEnum.Unix => "Unix",
                OperatorSysEnum.MacOSX => "Mac",
                OperatorSysEnum.XBox => "XBox",
                OperatorSysEnum.Unknown => "未知",
                _ => "未知",
            };
            return name;
        }
    }
}
