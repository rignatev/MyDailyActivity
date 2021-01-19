using System;
using System.Threading;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.OperationScopes
{
    public abstract class OperationScopeBase<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        static protected readonly ReaderWriterLockSlim ScopeLock = new();
        private bool _disposed;

        public TDbContext DbContext { get; }

        protected OperationScopeBase(TDbContext dbContext) =>
            this.DbContext = dbContext;

        /// <inheritdoc />
        public void Dispose() =>
            Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                this.DbContext?.Dispose();
            }

            _disposed = true;
        }
    }
}
