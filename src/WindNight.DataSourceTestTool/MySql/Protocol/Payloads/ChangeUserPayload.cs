using System;
using MySqlConnector.Protocol.Serialization;

#nullable enable
namespace MySqlConnector.Protocol.Payloads
{
	internal static class ChangeUserPayload
	{
		public static PayloadData Create(string user, ReadOnlySpan<byte> authResponse, string? schemaName, CharacterSet characterSet, byte[]? connectionAttributes)
		{
			var writer = new ByteBufferWriter();

			writer.Write((byte) CommandKind.ChangeUser);
			writer.WriteNullTerminatedString(user);
			writer.Write(checked((byte) authResponse.Length));
			writer.Write(authResponse);
			writer.WriteNullTerminatedString(schemaName ?? "");
			writer.Write((byte) characterSet);
			writer.Write((byte) 0);
			writer.WriteNullTerminatedString("mysql_native_password");
			if (connectionAttributes is not null)
				writer.Write(connectionAttributes);

			return writer.ToPayloadData();
		}
	}
}
