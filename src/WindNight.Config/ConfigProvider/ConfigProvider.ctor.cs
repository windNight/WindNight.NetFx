
namespace WindNight.ConfigCenter.Extension
{
    internal partial class ConfigProvider
    {
        private static readonly object LockInstance = new object();

        static ConfigProvider()
        {
            if (Instance == null)
                lock (LockInstance)
                {
                    if (Instance == null)
                        Instance = new ConfigProvider();
                }
        }


        private ConfigProvider()
        {
            _isStop = false;
            RegisterHeartRun();
        }

        ~ConfigProvider()
        {
            Stop();
        }

    }
}
