using System;

using Data.Contracts.OperationScopes;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public abstract class DbScopeBase : IDisposable, IDbScope
    {
        private bool _disposed;

        protected IServiceScope ServiceScope { get; }

        protected DbContext DbContext { get; set; }

        protected DbScopeBase(IServiceProvider serviceProvider) => this.ServiceScope = serviceProvider.CreateScope();

        /// <inheritdoc />
        public void Dispose() => 
            Dispose(disposing: true);

        /// <inheritdoc />
        public abstract TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                this.ServiceScope?.Dispose();
            }

            _disposed = true;
        }
    }
}
