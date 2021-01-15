using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public class ReaderScope<TDbContext> : OperationScopeBase<TDbContext>
        where TDbContext : DbContext
    {
        private bool _disposed;

        /// <inheritdoc />
        public ReaderScope(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ScopeLock.EnterReadLock();
            this.DbContext = ServiceProvider.GetRequiredService<TDbContext>();
            this.DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ScopeLock.ExitReadLock();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
