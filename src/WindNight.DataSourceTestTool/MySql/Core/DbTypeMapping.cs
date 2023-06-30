using System;
using System.Data;

#nullable enable
namespace MySqlConnector.Core
{
	internal sealed class DbTypeMapping
	{
		public DbTypeMapping(Type clrType, DbType[] dbTypes, Func<object, object>? convert = null)
		{
			ClrType = clrType;
			DbTypes = dbTypes;
			m_convert = convert;
		}

		public Type ClrType { get; }
		public DbType[] DbTypes { get; }

		public object DoConversion(object obj)
		{
			if (obj.GetType() == ClrType)
				return obj;
			return m_convert is null ? Convert.ChangeType(obj, ClrType)! : m_convert(obj);
		}

		readonly Func<object, object>? m_convert;
	}
}
