using System;
using System.Collections.Generic;

using Data.Contracts.EntityDataServices;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

using Services.Contracts.EntityServices;

namespace Services
{
    public abstract class EntityServiceBase<TEntity, TEntityIdType, TEntityDataService> :
        IEntityService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
        where TEntityDataService : IEntityDataService<TEntity, TEntityIdType>
    {
        protected TEntityDataService EntityDataService { get; }

        protected EntityServiceBase(TEntityDataService entityDataService) =>
            this.EntityDataService = entityDataService;

        /// <inheritdoc />
        public OperationResult<TEntityIdType> Create(TEntity entity)
        {
            OperationResult<TEntityIdType> dataServiceResult = this.EntityDataService.Create(entity);

            return dataServiceResult.Success
                ? OperationResult<TEntityIdType>.Ok(dataServiceResult.Value)
                : OperationResult<TEntityIdType>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult Update(TEntity entity) =>
            this.EntityDataService.Update(entity);

        /// <inheritdoc />
        public OperationResult Delete(TEntityIdType id) =>
            this.EntityDataService.Delete(id);

        /// <inheritdoc />
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids) =>
            this.EntityDataService.DeleteRange(ids);

        /// <inheritdoc />
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            OperationResult<TEntity> dataServiceResult = this.EntityDataService.GetEntity(id, includeRelated);

            return dataServiceResult.Success
                ? OperationResult<TEntity>.Ok(dataServiceResult.Value)
                : OperationResult<TEntity>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult<IEnumerable<TEntity>> GetEntities(EntityServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters)
        {
            var dataServiceGetEntitiesParameters = new EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType>
            {
                Ids = parameters.Ids,
                OrderByDescending = parameters.OrderByDescending,
                OrderByProperty = parameters.OrderByProperty,
                Count = parameters.Count,
                IncludeRelated = parameters.IncludeRelated
            };

            OperationResult<IEnumerable<TEntity>> dataServiceResult = this.EntityDataService.GetEntities(dataServiceGetEntitiesParameters);

            return dataServiceResult.Success
                ? OperationResult<IEnumerable<TEntity>>.Ok(dataServiceResult.Value)
                : OperationResult<IEnumerable<TEntity>>.Fail(dataServiceResult.Error);
        }
    }
}
