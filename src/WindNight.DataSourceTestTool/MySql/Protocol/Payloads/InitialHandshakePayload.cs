using System;
using System.Text;
using MySqlConnector.Protocol.Serialization;
using MySqlConnector.Utilities;

#nullable enable
namespace MySqlConnector.Protocol.Payloads
{
	internal sealed class InitialHandshakePayload
	{
		public ProtocolCapabilities ProtocolCapabilities { get; }
		public byte[] ServerVersion { get; }
		public int ConnectionId { get; }
		public byte[] AuthPluginData { get; }
		public string? AuthPluginName { get; }

		public static InitialHandshakePayload Create(ReadOnlySpan<byte> span)
		{
			var reader = new ByteArrayReader(span);
			reader.ReadByte(c_protocolVersion);
			var serverVersion = reader.ReadNullTerminatedByteString();
			var connectionId = reader.ReadInt32();
			byte[]? authPluginData = null;
			var authPluginData1 = reader.ReadByteString(8);
			string? authPluginName = null;
			reader.ReadByte(0);
			var protocolCapabilities = (ProtocolCapabilities) reader.ReadUInt16();
			if (reader.BytesRemaining > 0)
			{
				var charSet = (CharacterSet) reader.ReadByte(); // lgtm[cs/useless-assignment-to-local]
				var status = (ServerStatus) reader.ReadInt16(); // lgtm[cs/useless-assignment-to-local]
				var capabilityFlagsHigh = reader.ReadUInt16();
				protocolCapabilities |= (ProtocolCapabilities) ((ulong) capabilityFlagsHigh << 16);
				var authPluginDataLength = reader.ReadByte();
				reader.Offset += 6;

				long extendedCapabilites = reader.ReadInt32();
				if ((protocolCapabilities & ProtocolCapabilities.LongPassword) == 0)
				{
					// MariaDB clears the CLIENT_LONG_PASSWORD flag to indicate it's not a MySQL Server
					protocolCapabilities |= (ProtocolCapabilities) (extendedCapabilites << 32);
				}

				if ((protocolCapabilities & ProtocolCapabilities.SecureConnection) != 0)
				{
					var authPluginData2 = reader.ReadByteString(Math.Max(13, authPluginDataLength - 8));
					authPluginData = new byte[authPluginData1.Length + authPluginData2.Length];
					authPluginData1.CopyTo(authPluginData);
					authPluginData2.CopyTo(new Span<byte>(authPluginData).Slice(authPluginData1.Length));
				}
				if ((protocolCapabilities & ProtocolCapabilities.PluginAuth) != 0)
					authPluginName = Encoding.UTF8.GetString(reader.ReadNullOrEofTerminatedByteString());
			}
			authPluginData ??= authPluginData1.ToArray();

			if (reader.BytesRemaining != 0)
				throw new FormatException("Extra bytes at end of payload.");

			return new InitialHandshakePayload(protocolCapabilities, serverVersion.ToArray(), connectionId, authPluginData, authPluginName);
		}

		private InitialHandshakePayload(ProtocolCapabilities protocolCapabilities, byte[] serverVersion, int connectionId, byte[] authPluginData, string? authPluginName)
		{
			ProtocolCapabilities = protocolCapabilities;
			ServerVersion = serverVersion;
			ConnectionId = connectionId;
			AuthPluginData = authPluginData;
			AuthPluginName = authPluginName;
		}

		const byte c_protocolVersion = 0x0A;
	}
}
