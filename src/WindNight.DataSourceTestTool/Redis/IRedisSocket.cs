using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.Redis
{
    internal interface IRedisSocket : IDisposable
    {
        bool Connected { get; }
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }

        void Connect(EndPoint endpoint, int timeout);
        Task<bool> ConnectAsync(EndPoint endpoint);

        Stream GetStream();
    }
}
