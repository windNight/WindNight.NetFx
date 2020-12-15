using System;
using System.Windows.Forms;
using WindNight.DataSourceTestTool.Forms;

namespace WindNight.DataSourceTestTool
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if NETCOREAPP
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DataSourceTestForm());

        }
    }

    public class StringUtility
    {
        public static string ToUpperInvariant(string s)
        {
            return s.ToUpperInvariant();
        }

        public static string ToLowerInvariant(string s)
        {
            return s.ToLowerInvariant();
        }
    }

}
