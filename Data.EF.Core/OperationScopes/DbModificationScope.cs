using System;
using System.Threading;
using System.Threading.Tasks;

using Data.Contracts.OperationScopes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public class DbModificationScope : DbScopeBase, IDbModificationScope
    {
        private IDbContextTransaction _transaction;
        private bool _disposed;

        /// <inheritdoc />
        public DbModificationScope(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        public override TDbContext GetDbContext<TDbContext>()
        {
            if (this.DbContext != null)
            {
                return (TDbContext)this.DbContext;
            }

            this.DbContext = this.ServiceScope.ServiceProvider.GetRequiredService<TDbContext>();
            _transaction = this.DbContext.Database.BeginTransaction();

            return (TDbContext)this.DbContext;
        }

        /// <inheritdoc />
        public void SaveChangesAndCommit<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            dbContext.SaveChanges();

            Commit();
        }

        /// <inheritdoc />
        public async Task SaveChangesAndCommitAsync<TDbContext>(
            TDbContext dbContext,
            CancellationToken cancellationToken = default(CancellationToken)) where TDbContext : DbContext
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            await CommitAsync();
        }

        /// <inheritdoc />
        public void Commit() => 
            _transaction.Commit();

        /// <inheritdoc />
        public async Task CommitAsync() => 
            await _transaction.CommitAsync();

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _transaction.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
