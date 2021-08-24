using System;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.OperationScopes
{
    public class ReaderScope<TDbContext> : OperationScopeBase<TDbContext>
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public ReaderScope(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
