using System;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public abstract class OperationScopeBase<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        private readonly IServiceScope _serviceScope;
        private bool _disposed;

        public TDbContext DbContext { get; }

        protected OperationScopeBase(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            this.DbContext = _serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
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
                _serviceScope?.Dispose();
            }

            _disposed = true;
        }
    }
}
