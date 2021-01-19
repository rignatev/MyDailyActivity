using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Data.Contracts.EntityDataServices;
using Data.Shared;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

using Services.Contracts.EntityServices;

namespace Services
{
    public abstract class EntityServiceBase<TEntity, TEntityIdType, TEntityDataService, TEntityOrm, TEntityOrmIdType> :
        IEntityService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
        where TEntityDataService : IEntityDataService<TEntityOrm, TEntityOrmIdType>
        where TEntityOrm : class, IEntityOrm<TEntityOrmIdType>, new()
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
        protected TEntityDataService EntityDataService { get; }

        protected EntityServiceBase(TEntityDataService entityDataService) =>
            this.EntityDataService = entityDataService;

        /// <inheritdoc />
        public OperationResult<TEntityIdType> Create(TEntity entity)
        {
            TEntityOrm entityOrm = ConvertToEntityOrm(entity);
            OperationResult<TEntityOrmIdType> dataServiceResult = this.EntityDataService.Create(entityOrm);

            return dataServiceResult.Success
                ? OperationResult<TEntityIdType>.Ok(ConvertToEntityId(dataServiceResult.Value))
                : OperationResult<TEntityIdType>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult Update(TEntity entity)
        {
            TEntityOrm entityOrm = ConvertToEntityOrm(entity);

            return this.EntityDataService.Update(entityOrm);
        }

        /// <inheritdoc />
        public OperationResult Delete(TEntityIdType id)
        {
            TEntityOrmIdType entityOrmId = ConvertToEntityOrmId(id);

            return this.EntityDataService.Delete(entityOrmId);
        }

        /// <inheritdoc />
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids)
        {
            IEnumerable<TEntityOrmIdType> entityOrmIds = ids.Select(ConvertToEntityOrmId);

            return this.EntityDataService.DeleteRange(entityOrmIds);
        }

        /// <inheritdoc />
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            TEntityOrmIdType entityOrmId = ConvertToEntityOrmId(id);

            OperationResult<TEntityOrm> dataServiceResult = this.EntityDataService.GetEntity(entityOrmId, includeRelated);

            return dataServiceResult.Success
                ? OperationResult<TEntity>.Ok(ConvertToEntity(dataServiceResult.Value))
                : OperationResult<TEntity>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult<IEnumerable<TEntity>> GetEntities(EntityServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters)
        {
            var dataServiceGetEntitiesParameters = new EntityDataServiceGetEntitiesParameters<TEntityOrm, TEntityOrmIdType>
            {
                Ids = parameters.Ids.Select(ConvertToEntityOrmId),
                OrderByDescending = parameters.OrderByDescending,
                OrderByProperty = ConvertToEntityOrmProperty(parameters.OrderByProperty),
                Count = parameters.Count,
                IncludeRelated = parameters.IncludeRelated
            };

            OperationResult<IEnumerable<TEntityOrm>> dataServiceResult =
                this.EntityDataService.GetEntities(dataServiceGetEntitiesParameters);

            return dataServiceResult.Success
                ? OperationResult<IEnumerable<TEntity>>.Ok(dataServiceResult.Value.Select(ConvertToEntity))
                : OperationResult<IEnumerable<TEntity>>.Fail(dataServiceResult.Error);
        }

        protected abstract TEntityIdType ConvertToEntityId(TEntityOrmIdType entityOrmIdType);

        protected abstract TEntity ConvertToEntity(TEntityOrm entityOrm);

        protected abstract TEntityOrmIdType ConvertToEntityOrmId(TEntityIdType entityIdType);

        protected abstract TEntityOrm ConvertToEntityOrm(TEntity entity);

        static private Expression<Func<TEntityOrm, object>> ConvertToEntityOrmProperty(Expression<Func<TEntity, object>> entityProperty) =>
            // TODO: Implement convertion
            throw new NotImplementedException();
    }
}
