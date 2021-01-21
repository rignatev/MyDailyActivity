using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Data.Contracts.EntityDataServices;
using Data.Contracts.EntityOrm;
using Data.EF.Core.OperationScopes;
using Data.EF.Core.Utils;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Data.EF.Core
{
    public abstract class EntityDataServiceBase<TEntity, TEntityIdType, TEntityOrm, TEntityOrmIdType, TDbContext> :
        IEntityDataService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
        where TEntityOrm : class, IEntityOrm<TEntityOrmIdType>, new()
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
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
                TEntityOrm entityOrm = ConvertToEntityOrm(entity);

                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                EntityEntry<TEntityOrm> updateResult = entityDbSet.Update(entityOrm);

                modificationScope.SaveChangesAndCommit();

                TEntityIdType entityId = ConvertToEntityId(updateResult.Entity.Id);

                result = OperationResult<TEntityIdType>.Ok(entityId);
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
                TEntityOrm entityOrm = ConvertToEntityOrm(entity);

                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.Update(entityOrm);

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
                TEntityOrmIdType entityOrmId = ConvertToEntityOrmId(id);

                using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);
                entityDbSet.Remove(new TEntityOrm { Id = entityOrmId });

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

                IEnumerable<TEntityOrm> entities = ids.Select(id => new TEntityOrm { Id = ConvertToEntityOrmId(id) });

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
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            OperationResult<TEntity> result;

            try
            {
                using ReaderScope<TDbContext> readerScope = CreateReaderScope();

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(readerScope);

                TEntityOrm entityOrm = includeRelated
                    ? entityDbSet.IncludeAll().FirstOrDefault(x => x.Id.Equals(id))
                    : entityDbSet.Find(id);

                TEntity entity = ConvertToEntity(entityOrm);

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

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(readerScope);
                IQueryable<TEntityOrm> query = entityDbSet.AsQueryable();

                if (parameters.Ids != null)
                {
                    query = query.Where(x => parameters.Ids.Select(ConvertToEntityOrmId).Contains(x.Id));
                }

                if (parameters.OrderByDescending != null)
                {
                    if (parameters.OrderByProperty == null)
                    {
                        throw new ArgumentNullException($"Argument {nameof(parameters.OrderByProperty)} is null.");
                    }

                    query = parameters.OrderByDescending == true
                        ? query.OrderByDescending(ConvertToEntityOrmProperty(parameters.OrderByProperty))
                        : query.OrderBy(ConvertToEntityOrmProperty(parameters.OrderByProperty));
                }

                if (parameters.Count != null)
                {
                    query = query.Take((int)parameters.Count);
                }

                if (parameters.IncludeRelated)
                {
                    query = query.IncludeAll(readerScope.DbContext);
                }

                IEnumerable<TEntity> entities = query.AsEnumerable().Select(ConvertToEntity);

                result = OperationResult<IEnumerable<TEntity>>.Ok(entities);
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<IEnumerable<TEntity>>.Fail(error);
            }

            return result;
        }

        protected abstract TEntity ConvertToEntity(TEntityOrm entityOrm);

        protected abstract TEntityIdType ConvertToEntityId(TEntityOrmIdType entityOrmIdType);

        protected abstract TEntityOrm ConvertToEntityOrm(TEntity entity);

        protected abstract TEntityOrmIdType ConvertToEntityOrmId(TEntityIdType entityIdType);

        static protected DbSet<TEntityOrm> GetEntityDbSet(OperationScopeBase<TDbContext> scope) =>
            scope.DbContext.Set<TEntityOrm>();

        static protected DbSet<TCustomEntityOrm> GetEntityDbSet<TCustomEntityOrm>(OperationScopeBase<TDbContext> scope)
            where TCustomEntityOrm : class =>
            scope.DbContext.Set<TCustomEntityOrm>();

        protected ReaderScope<TDbContext> CreateReaderScope() =>
            new(_serviceProvider);

        protected ModificationScope<TDbContext> CreateModificationScope() =>
            new(_serviceProvider);

        static private Expression<Func<TEntityOrm, object>> ConvertToEntityOrmProperty(Expression<Func<TEntity, object>> entityProperty) =>
            // TODO: Implement convertion
            throw new NotImplementedException();
    }
}
