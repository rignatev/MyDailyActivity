using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Data.Contracts.OperationScopes
{
    public interface IDbModificationScope : IDbScope
    {
        void Commit();

        Task CommitAsync();

        void SaveChangesAndCommit<TDbContext>(TDbContext dbContext) where TDbContext : DbContext;

        Task SaveChangesAndCommitAsync<TDbContext>(TDbContext dbContext, CancellationToken cancellationToken) where TDbContext : DbContext;
    }
}
