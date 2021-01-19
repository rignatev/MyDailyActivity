using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.EF.Core.OperationScopes
{
    public class ModificationScope<TDbContext> : OperationScopeBase<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        /// <inheritdoc />
        public ModificationScope(TDbContext dbContext) : base(dbContext)
        {
            ScopeLock.EnterWriteLock();
            _transaction = this.DbContext.Database.BeginTransaction();
        }

        public int SaveChanges() =>
            this.DbContext.SaveChanges();

        public void Commit() =>
            _transaction.Commit();

        public int SaveChangesAndCommit()
        {
            int result = SaveChanges();
            Commit();

            return result;
        }

        public void SaveChangesIfSucceeded(bool success)
        {
            if (success)
            {
                SaveChanges();
            }
        }

        public void CommitIfSucceeded(bool success)
        {
            if (success)
            {
                Commit();
            }
        }

        public void SaveChangesAndCommitIfSucceeded(bool success)
        {
            if (success)
            {
                SaveChangesAndCommit();
            }
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
                _transaction.Dispose();
                ScopeLock.ExitWriteLock();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}
