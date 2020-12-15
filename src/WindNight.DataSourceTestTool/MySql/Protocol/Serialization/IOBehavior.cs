namespace MySqlConnector.Protocol.Serialization
{
	/// <summary>
	/// Specifies whether to perform synchronous or asynchronous I/O.
	/// </summary>
	internal enum IOBehavior
	{
		/// <summary>
		/// Use synchronous I/O.
		/// </summary>
		Synchronous,

		/// <summary>
		/// Use asynchronous I/O.
		/// </summary>
		Asynchronous,
	}
}
