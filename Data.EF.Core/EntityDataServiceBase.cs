using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Data.Contracts.EntityDataServices;
using Data.Contracts.EntityOrm;
using Data.Contracts.OperationScopes;
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
        public OperationResult<TEntityIdType> Create(TEntity entity, IDbModificationScope dbModificationScope)
        {
            OperationResult<TEntityIdType> result;

            try
            {
                TDbContext modificationContext = GetModificationContext(dbModificationScope);
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationContext);

                EntityEntry<TEntityOrm> updateResult = entityDbSet.Update(ConvertToEntityOrm(entity));
                modificationContext.SaveChanges();

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
        public OperationResult Update(TEntity entity, IDbModificationScope dbModificationScope)
        {
            OperationResult result;

            try
            {
                TDbContext modificationContext = GetModificationContext(dbModificationScope);
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationContext);

                entityDbSet.Update(ConvertToEntityOrm(entity));
                modificationContext.SaveChanges();

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
        public OperationResult Delete(TEntityIdType id, IDbModificationScope dbModificationScope)
        {
            OperationResult result;

            try
            {
                TDbContext modificationContext = GetModificationContext(dbModificationScope);
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationContext);

                TEntityOrm entityOrm = entityDbSet.Find(id);

                entityDbSet.Remove(entityOrm);
                modificationContext.SaveChanges();

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
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids, IDbModificationScope dbModificationScope)
        {
            OperationResult result;

            try
            {
                if (ids == null)
                {
                    throw new ArgumentNullException($"Argument {nameof(ids)} is null.");
                }

                TDbContext modificationContext = GetModificationContext(dbModificationScope);
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(modificationContext);

                IEnumerable<TEntityOrm> entities = entityDbSet.Where(x => ids.Select(ConvertToEntityOrmId).Contains(x.Id));

                entityDbSet.RemoveRange(entities);
                modificationContext.SaveChanges();

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
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated, IDbScope dbScope)
        {
            OperationResult<TEntity> result;

            try
            {
                TDbContext readerContext = GetReaderContext(dbScope);
                DbSet<TEntityOrm> entityDbSet = GetEntityDbSet(readerContext);

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
        public OperationResult<List<TEntity>> GetEntities(
            EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters,
            IDbScope dbScope)
        {
            OperationResult<List<TEntity>> result;

            try
            {
                TDbContext readerContext = GetReaderContext(dbScope);
                IQueryable<TEntityOrm> query = GetEntityDbSet(readerContext).AsQueryable();

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
                    query = query.IncludeAll(readerContext);
                }

                IEnumerable<TEntity> entities = query.AsEnumerable().Select(ConvertToEntity);

                result = OperationResult<List<TEntity>>.Ok(entities.ToList());
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<List<TEntity>>.Fail(error);
            }

            return result;
        }

        protected abstract TEntity ConvertToEntity(TEntityOrm entityOrm);

        protected abstract TEntityIdType ConvertToEntityId(TEntityOrmIdType entityOrmIdType);

        protected abstract TEntityOrm ConvertToEntityOrm(TEntity entity);

        protected abstract TEntityOrmIdType ConvertToEntityOrmId(TEntityIdType entityIdType);

        static protected DbSet<TEntityOrm> GetEntityDbSet(TDbContext dbContext) =>
            dbContext.Set<TEntityOrm>();

        static protected DbSet<TCustomEntityOrm> GetEntityDbSet<TCustomEntityOrm>(TDbContext dbContext)
            where TCustomEntityOrm : class =>
            dbContext.Set<TCustomEntityOrm>();

        protected TDbContext GetReaderContext(IDbScope dbScope) =>
            dbScope.GetDbContext<TDbContext>();

        protected TDbContext GetModificationContext(IDbModificationScope dbModificationScope) =>
            dbModificationScope.GetDbContext<TDbContext>();

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
