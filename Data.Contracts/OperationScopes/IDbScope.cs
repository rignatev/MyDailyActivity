using Microsoft.EntityFrameworkCore;

namespace Data.Contracts.OperationScopes
{
    public interface IDbScope
    {
        TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext;
    }
}
