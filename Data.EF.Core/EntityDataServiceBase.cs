using System;
using System.Collections.Generic;
using System.Linq;

using Data.Contracts.EntityServices;
using Data.EF.Core.OperationScopes;
using Data.EF.Core.Utils;
using Data.Shared;

using Infrastructure.Shared.OperationResult;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.EF.Core
{
    public abstract class EntityDataServiceBase<TEntity, TEntityIdType, TDbContext> : IEntityDataService<TEntity, TEntityIdType>
        where TEntity : class, IEntityOrm<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        protected EntityDataServiceBase(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        /// <inheritdoc />
        public OperationResult<TEntityIdType> Create(TEntity entity)
        {
            OperationResult<TEntityIdType> result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(modificationScope);
                EntityEntry<TEntity> updateResult = entityDbSet.Update(entity);

                modificationScope.SaveChangesAndCommit();

                result = OperationResult<TEntityIdType>.Ok(updateResult.Entity.Id);
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<TEntityIdType>.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult Update(TEntity entity)
        {
            OperationResult result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(modificationScope);
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
        public OperationResult Delete(TEntityIdType id)
        {
            OperationResult result;

            try
            {
                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.Remove(new TEntity { Id = id });

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
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids)
        {
            OperationResult result;

            try
            {
                if (ids == null)
                {
                    throw new ArgumentNullException($"Argument {nameof(ids)} is null.");
                }

                IEnumerable<TEntity> entities = ids.Select(id => new TEntity { Id = id });

                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(modificationScope);
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
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            OperationResult<TEntity> result;

            try
            {
                using ReaderScope<TDbContext> readerScope = CreateReaderScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(readerScope);

                TEntity entity = includeRelated
                    ? entityDbSet.IncludeAll().FirstOrDefault(x => x.Id.Equals(id))
                    : entityDbSet.Find(id);

                result = OperationResult<TEntity>.Ok(entity);
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<TEntity>.Fail(error);
            }

            return result;
        }

        /// <inheritdoc />
        public OperationResult<IEnumerable<TEntity>> GetEntities(EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters)
        {
            OperationResult<IEnumerable<TEntity>> result;

            try
            {
                using ReaderScope<TDbContext> readerScope = CreateReaderScope();

                DbSet<TEntity> entityDbSet = GetEntityDbSet(readerScope);
                IQueryable<TEntity> query = entityDbSet.AsQueryable();

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

                result = OperationResult<IEnumerable<TEntity>>.Ok(query.AsEnumerable());
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<IEnumerable<TEntity>>.Fail(error);
            }

            return result;
        }

        static private DbSet<TEntity> GetEntityDbSet(OperationScopeBase<TDbContext> scope) =>
            scope.DbContext.Set<TEntity>();

        private ReaderScope<TDbContext> CreateReaderScope() =>
            new(_serviceProvider);

        private ModificationScope<TDbContext> CreateModificationScope() =>
            new(_serviceProvider);
    }
}
