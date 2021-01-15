using System;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public abstract class OperationScopeBase<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        static protected readonly ReaderWriterLockSlim ScopeLock = new();
        protected readonly IServiceProvider ServiceProvider;
        private bool _disposed;

        public TDbContext DbContext { get; protected init; }

        protected OperationScopeBase(IServiceProvider serviceProvider)
        {
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            ServiceProvider = serviceScope.ServiceProvider;
        }

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
