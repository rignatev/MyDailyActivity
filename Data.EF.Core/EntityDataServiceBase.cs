using System;
using System.Collections.Generic;
using System.Linq;

using Data.Contracts.EntityDataServices;
using Data.EF.Core.OperationScopes;
using Data.EF.Core.Utils;
using Data.Shared;

using Infrastructure.Shared.OperationResult;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.EF.Core
{
    public abstract class EntityDataServiceBase<TEntityOrm, TEntityOrmIdType, TDbContext> : IEntityDataService<TEntityOrm, TEntityOrmIdType>
        where TEntityOrm : class, IEntityOrm<TEntityOrmIdType>, new()
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        protected EntityDataServiceBase(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        /// <inheritdoc />
        public OperationResult<TEntityOrmIdType> Create(TEntityOrm entity)
        {
            OperationResult<TEntityOrmIdType> result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                EntityEntry<TEntityOrm> updateResult = entityDbSet.Update(entity);

                modificationScope.SaveChangesAndCommit();

                result = OperationResult<TEntityOrmIdType>.Ok(updateResult.Entity.Id);
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<TEntityOrmIdType>.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult Update(TEntityOrm entity)
        {
            OperationResult result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.Update(entity);

                modificationScope.SaveChangesAndCommit();

                result = OperationResult.Ok();
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult Delete(TEntityOrmIdType id)
        {
            OperationResult result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.Remove(new TEntityOrm { Id = id });

                modificationScope.SaveChangesAndCommit();

                result = OperationResult.Ok();
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult DeleteRange(IEnumerable<TEntityOrmIdType> ids)
        {
            OperationResult result;

            try
            {
                if (ids == null)
                {
                    throw new ArgumentNullException($"Argument {nameof(ids)} is null.");
                }

                IEnumerable<TEntityOrm> entities = ids.Select(id => new TEntityOrm { Id = id });

                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.RemoveRange(entities);

                modificationScope.SaveChangesAndCommit();

                result = OperationResult.Ok();
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult<TEntityOrm> GetEntity(TEntityOrmIdType id, bool includeRelated = false)
        {
            OperationResult<TEntityOrm> result;

            try
            {
                using ReaderScope<TDbContext> readerScope = CreateReaderScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(readerScope);

                TEntityOrm entity = includeRelated
                    ? entityDbSet.IncludeAll().FirstOrDefault(x => x.Id.Equals(id))
                    : entityDbSet.Find(id);

                result = OperationResult<TEntityOrm>.Ok(entity);
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<TEntityOrm>.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult<IEnumerable<TEntityOrm>> GetEntities(EntityDataServiceGetEntitiesParameters<TEntityOrm, TEntityOrmIdType> parameters)
        {
            OperationResult<IEnumerable<TEntityOrm>> result;

            try
            {
                using ReaderScope<TDbContext> readerScope = CreateReaderScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(readerScope);
                IQueryable<TEntityOrm> query = entityDbSet.AsQueryable();

                if (parameters.Ids != null)
                {
                    query = query.Where(x => parameters.Ids.Contains(x.Id));
                }

                if (parameters.OrderByDescending != null)
                {
                    if (parameters.OrderByProperty == null)
                    {
                        throw new ArgumentNullException($"Argument {nameof(parameters.OrderByProperty)} is null.");
                    }

                    query = parameters.OrderByDescending == true
                        ? query.OrderByDescending(parameters.OrderByProperty)
                        : query.OrderBy(parameters.OrderByProperty);
                }

                if (parameters.Count != null)
                {
                    query = query.Take((int)parameters.Count);
                }

                if (parameters.IncludeRelated)
                {
                    query = query.IncludeAll(readerScope.DbContext);
                }

                result = OperationResult<IEnumerable<TEntityOrm>>.Ok(query.AsEnumerable());
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<IEnumerable<TEntityOrm>>.Fail(error);
            }

            return result;
        }

        static private DbSet<TEntityOrm> GetEntityDbSet(OperationScopeBase<TDbContext> scope) =>
            scope.DbContext.Set<TEntityOrm>();

        private ReaderScope<TDbContext> CreateReaderScope() =>
            new(_serviceProvider);

        private ModificationScope<TDbContext> CreateModificationScope() =>
            new(_serviceProvider);
    }
}
