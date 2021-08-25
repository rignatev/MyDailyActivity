using System;

using Data.Contracts.OperationScopes;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core.OperationScopes
{
    public class DbReaderScope : DbScopeBase, IDbReaderScope
    {
        /// <inheritdoc />
        public DbReaderScope(IServiceProvider serviceProvider) : base(serviceProvider)
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
            this.DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return (TDbContext)this.DbContext;
        }
    }
}
