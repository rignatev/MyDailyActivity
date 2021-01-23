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
            using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

            return Create(entity, modificationScope);
        }

        /// <inheritdoc />
        public OperationResult Update(TEntity entity)
        {
            using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

            return Update(entity, modificationScope);
        }

        /// <inheritdoc />
        public OperationResult Delete(TEntityIdType id)
        {
            using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

            return Delete(id, modificationScope);
        }

        /// <inheritdoc />
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids)
        {
            using ModificationScope<TDbContext> modificationScope = CreateModificationScope();

            return DeleteRange(ids, modificationScope);
        }

        /// <inheritdoc />
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            using ReaderScope<TDbContext> readerScope = CreateReaderScope();

            return GetEntity(id, includeRelated, readerScope);
        }

        /// <inheritdoc />
        public OperationResult<List<TEntity>> GetEntities(EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters)
        {
            using ReaderScope<TDbContext> readerScope = CreateReaderScope();

            OperationResult<IEnumerable<TEntity>> result = GetEntities(parameters, readerScope);

            if (result.Success)
            {
                return OperationResult<List<TEntity>>.Ok(result.Value.ToList());
            }

            return OperationResult<List<TEntity>>.Fail(result.Error);
        }

        protected OperationResult<TEntityIdType> Create(TEntity entity, ModificationScope<TDbContext> modificationScope)
        {
            OperationResult<TEntityIdType> result;

            try
            {
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);

                EntityEntry<TEntityOrm> updateResult = entityDbSet.Update(ConvertToEntityOrm(entity));

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

        protected OperationResult Update(TEntity entity, ModificationScope<TDbContext> modificationScope)
        {
            OperationResult result;

            try
            {
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);

                entityDbSet.Update(ConvertToEntityOrm(entity));

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

        protected OperationResult Delete(TEntityIdType id, ModificationScope<TDbContext> modificationScope)
        {
            OperationResult result;

            try
            {
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);

                entityDbSet.Remove(new TEntityOrm { Id = ConvertToEntityOrmId(id) });

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

        protected OperationResult DeleteRange(IEnumerable<TEntityIdType> ids, ModificationScope<TDbContext> modificationScope)
        {
            OperationResult result;

            try
            {
                if (ids == null)
                {
                    throw new ArgumentNullException($"Argument {nameof(ids)} is null.");
                }

                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationScope);

                IEnumerable<TEntityOrm> entities = ids.Select(id => new TEntityOrm { Id = ConvertToEntityOrmId(id) });

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

        protected OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated, ReaderScope<TDbContext> readerScope)
        {
            OperationResult<TEntity> result;

            try
            {
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

        protected OperationResult<IEnumerable<TEntity>> GetEntities(
            EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters,
            ReaderScope<TDbContext> readerScope)
        {
            OperationResult<IEnumerable<TEntity>> result;

            try
            {
                IQueryable<TEntityOrm> query = GetEntityDbSet(readerScope).AsQueryable();

                if (parameters.Ids != null)
                {
                    query = query.Where(x => parameters.Ids.Select(ConvertToEntityOrmId).Contains(x.Id));
                }

                if (parameters.OrderByProperty != null)
                {
                    if (parameters.OrderByDescending == null)
                    {
                        throw new ArgumentNullException($"Argument {nameof(parameters.OrderByDescending)} is null.");
                    }

                    query = (bool)parameters.OrderByDescending
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

        static private Expression<Func<TEntityOrm, T>> ConvertToEntityOrmProperty<T>(Expression<Func<TEntity, T>> entityProperty)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntityOrm), "x");
            Expression<Func<TEntityOrm, T>> lambdaExpression;

            if (entityProperty.Body is UnaryExpression unaryExpression)
            {
                var memberExpression = (MemberExpression)unaryExpression.Operand;
                string entityPropertyName = memberExpression.Member.Name;
                MemberExpression property = Expression.Property(parameter, entityPropertyName);
                UnaryExpression lambdaBody = Expression.Convert(property, typeof(object));
                lambdaExpression = Expression.Lambda<Func<TEntityOrm, T>>(lambdaBody, parameter);
            }
            else if(entityProperty.Body is MemberExpression memberExpression)
            {
                string entityPropertyName = memberExpression.Member.Name;
                MemberExpression lambdaBody = Expression.Property(parameter, entityPropertyName);
                lambdaExpression = Expression.Lambda<Func<TEntityOrm, T>>(lambdaBody, parameter);
            }
            else
            {
                throw new Exception("Not handled expression body type.");
            }

            return lambdaExpression;
        }
    }
}
