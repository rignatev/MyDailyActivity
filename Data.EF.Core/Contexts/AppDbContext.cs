using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.Contexts
{
    public class AppDbContext : DbContext
    {
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
