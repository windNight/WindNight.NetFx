using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector.Protocol.Serialization;
using MySqlConnector.Utilities;

#nullable  enable
namespace MySqlConnector
{
    /// <summary>
    /// <see cref="MySqlTransaction"/> represents an in-progress transaction on a MySQL Server.
    /// </summary>
    public sealed class MySqlTransaction : DbTransaction
    {
        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public override void Commit() => CommitAsync(IOBehavior.Synchronous, default).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously commits the database transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
        public override Task CommitAsync(CancellationToken cancellationToken = default) => CommitAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#else
		public override Task CommitAsync(CancellationToken cancellationToken = default) => CommitAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#endif

        private async Task CommitAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
            VerifyValid();

            using (var cmd = new MySqlCommand("commit", Connection, this))
                await cmd.ExecuteNonQueryAsync(ioBehavior, cancellationToken).ConfigureAwait(false);
            Connection!.CurrentTransaction = null;
            Connection = null;
        }

        /// <summary>
        /// Rolls back the database transaction.
        /// </summary>
        public override void Rollback() => RollbackAsync(IOBehavior.Synchronous, default).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously rolls back the database transaction.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
        public override Task RollbackAsync(CancellationToken cancellationToken = default) => RollbackAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#else
		public override Task RollbackAsync(CancellationToken cancellationToken = default) => RollbackAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#endif

        private async Task RollbackAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
            VerifyValid();

            using (var cmd = new MySqlCommand("rollback", Connection, this))
                await cmd.ExecuteNonQueryAsync(ioBehavior, cancellationToken).ConfigureAwait(false);
            Connection!.CurrentTransaction = null;
            Connection = null;
        }

        /// <summary>
        /// Removes the named transaction savepoint with the specified <paramref name="savepointName"/>. No commit or rollback occurs.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <remarks>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override void Release(string savepointName) => ExecuteSavepointAsync("release ", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#else
		public override void Release(string savepointName) => ExecuteSavepointAsync("release ", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Asynchronously removes the named transaction savepoint with the specified <paramref name="savepointName"/>. No commit or rollback occurs.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override Task ReleaseAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("release ", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#else
		public override Task ReleaseAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("release ", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#endif

        /// <summary>
        /// Rolls back the current transaction to the savepoint with the specified <paramref name="savepointName"/> without aborting the transaction.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <remarks><para>The name must have been created with <see cref="Save"/>, but not released by calling <see cref="Release"/>.</para>
        /// <para>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</para></remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override void Rollback(string savepointName) => ExecuteSavepointAsync("rollback to ", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#else
		public override void Rollback(string savepointName) => ExecuteSavepointAsync("rollback to ", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Asynchronously rolls back the current transaction to the savepoint with the specified <paramref name="savepointName"/> without aborting the transaction.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks><para>The name must have been created with <see cref="SaveAsync"/>, but not released by calling <see cref="ReleaseAsync"/>.</para>
        /// <para>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</para></remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override Task RollbackAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("rollback to ", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#else
		public override Task RollbackAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("rollback to ", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#endif

        /// <summary>
        /// Sets a named transaction savepoint with the specified <paramref name="savepointName"/>. If the current transaction
        /// already has a savepoint with the same name, the old savepoint is deleted and a new one is set.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <remarks>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override void Save(string savepointName) => ExecuteSavepointAsync("", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#else
		public override void Save(string savepointName) => ExecuteSavepointAsync("", savepointName, IOBehavior.Synchronous, default).GetAwaiter().GetResult();
#endif

        /// <summary>
        /// Asynchronously sets a named transaction savepoint with the specified <paramref name="savepointName"/>. If the current transaction
        /// already has a savepoint with the same name, the old savepoint is deleted and a new one is set.
        /// </summary>
        /// <param name="savepointName">The savepoint name.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>The proposed ADO.NET API that this is based on is not finalized; this API may change in the future.</remarks>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_1 || NETCOREAPP3_1 || NETCOREAPP || NET5_0
        public override Task SaveAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#else
		public override Task SaveAsync(string savepointName, CancellationToken cancellationToken = default) => ExecuteSavepointAsync("", savepointName, Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, cancellationToken);
#endif

        private async Task ExecuteSavepointAsync(string command, string savepointName, IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
            VerifyValid();

            if (savepointName is null)
                throw new ArgumentNullException(nameof(savepointName));
            if (savepointName.Length == 0)
                throw new ArgumentException("savepointName must not be empty", nameof(savepointName));

            using var cmd = new MySqlCommand(command + "savepoint " + QuoteIdentifier(savepointName), Connection, this);
            await cmd.ExecuteNonQueryAsync(ioBehavior, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="MySqlConnection"/> that this transaction is associated with.
        /// </summary>
        public new MySqlConnection? Connection { get; private set; }

        /// <summary>
        /// Gets the <see cref="MySqlConnection"/> that this transaction is associated with.
        /// </summary>
        protected override DbConnection? DbConnection => Connection;

        /// <summary>
        /// Gets the <see cref="IsolationLevel"/> of this transaction. This value is set from <see cref="MySqlConnection.BeginTransaction(IsolationLevel)"/>
        /// or any other overload that specifies an <see cref="IsolationLevel"/>.
        /// </summary>
        public override IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// Releases any resources associated with this transaction. If it was not committed, it will be rolled back.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this method is being called from <c>Dispose</c>; <c>false</c> if being called from a finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    DisposeAsync(IOBehavior.Synchronous, CancellationToken.None).GetAwaiter().GetResult();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Asynchronously releases any resources associated with this transaction. If it was not committed, it will be rolled back.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
        public new Task DisposeAsync() => DisposeAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, CancellationToken.None);
#else
		public override ValueTask DisposeAsync() => DisposeAsync(Connection?.AsyncIOBehavior ?? IOBehavior.Asynchronous, CancellationToken.None);
#endif

#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
        internal Task DisposeAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
#else
		internal ValueTask DisposeAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
#endif
        {
            m_isDisposed = true;
            if (Connection?.CurrentTransaction == this)
                return DoDisposeAsync(ioBehavior, cancellationToken);
            Connection = null;
#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
            return Utility.CompletedTask;
#else
			return default;
#endif
        }

#if NET45 || NET461 || NET471 || NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP2_1 || NETCOREAPP || NET5_0
        private async Task DoDisposeAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
#else
		private async ValueTask DoDisposeAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
#endif
        {
            if (Connection?.CurrentTransaction == this)
            {
                if (Connection.State == ConnectionState.Open && Connection.Session.IsConnected)
                {
                    using (var cmd = new MySqlCommand("rollback", Connection, this))
                        await cmd.ExecuteNonQueryAsync(ioBehavior, cancellationToken).ConfigureAwait(false);
                }
                Connection.CurrentTransaction = null;
            }
            Connection = null;
        }

        internal MySqlTransaction(MySqlConnection connection, IsolationLevel isolationLevel)
        {
            Connection = connection;
            IsolationLevel = isolationLevel;
        }

        private void VerifyValid()
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(nameof(MySqlTransaction));
            if (Connection is null)
                throw new InvalidOperationException("Already committed or rolled back.");
            if (Connection.CurrentTransaction is null)
                throw new InvalidOperationException("There is no active transaction.");
            if (!object.ReferenceEquals(Connection.CurrentTransaction, this))
                throw new InvalidOperationException("This is not the active transaction.");
        }

        private static string QuoteIdentifier(string identifier) => "`" + identifier.Replace("`", "``") + "`";

        bool m_isDisposed;
    }
}
