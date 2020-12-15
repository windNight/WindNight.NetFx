using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindNight.DataSourceTestTool.RabbitMQ
{
    public class BasicPropertiesConfigInfo
    {
        private int _priority = -1;
        private string _contentType = string.Empty;
        private string _contentEncoding = string.Empty;
        private IDictionary<string, object> _headers;
        private bool _durable;
        private string _correlationID = string.Empty;
        private string _replyTo = string.Empty;
        private long _expiration = -1;
        private string _messageID = string.Empty;
        private string _type = string.Empty;
        private long _timestamp = -1;
        private string _userID = string.Empty;
        private string _appID = string.Empty;
        private string _clusterID = string.Empty;

        public string ContentType
        {
            get => this._contentType;
            set => this._contentType = value;
        }

        public string ContentEncoding
        {
            get => this._contentEncoding;
            set => this._contentEncoding = value;
        }

        public IDictionary<string, object> Headers
        {
            get => this._headers;
            set => this._headers = value;
        }

        public bool Durable
        {
            get => this._durable;
            set => this._durable = value;
        }

        public int Priority
        {
            get => this._priority;
            set => this._priority = value <= 9 && value >= 0 ? (int)Convert.ToByte(value) : throw new Exception("无效的消息权重，消息权重的设置范围只能为[0,9]");
        }

        public string CorrelationID
        {
            get => this._correlationID;
            set => this._correlationID = value;
        }

        public string ReplyTo
        {
            get => this._replyTo;
            set => this._replyTo = value;
        }

        public long Expiration
        {
            get => this._expiration;
            set => this._expiration = value;
        }

        public string MessageID
        {
            get => this._messageID;
            set => this._messageID = value;
        }

        public long Timestamp
        {
            get => this._timestamp;
            set => this._timestamp = value;
        }

        public string Type
        {
            get => this._type;
            set => this._type = value;
        }

        public string UserID
        {
            get => this._userID;
            set => this._userID = value;
        }

        public string AppID
        {
            get => this._appID;
            set => this._appID = value;
        }

        public string ClusterID
        {
            get => this._clusterID;
            set => this._clusterID = value;
        }
    }
}
