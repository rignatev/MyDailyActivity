using System;
using System.Collections.Generic;

using Data.Contracts.EntityDataServices;
using Data.EF.Core.OperationScopes;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

using Microsoft.EntityFrameworkCore;

using Services.Contracts.EntityServices;

namespace Services
{
    public abstract class EntityServiceBase<TEntity, TEntityIdType, TEntityDataService> :
        IEntityService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
        where TEntityDataService : IEntityDataService<TEntity, TEntityIdType>
    {
        private readonly IServiceProvider _serviceProvider;

        protected TEntityDataService EntityDataService { get; }

        protected EntityServiceBase(IServiceProvider serviceProvider, TEntityDataService entityDataService)
        {
            _serviceProvider = serviceProvider;
            this.EntityDataService = entityDataService;
        }

        /// <inheritdoc />
        public OperationResult<TEntityIdType> Create(TEntity entity)
        {
            using DbModificationScope dbModificationScope = CreateModificationScope();

            OperationResult<TEntityIdType> dataServiceResult = this.EntityDataService.Create(entity, dbModificationScope);

            return dataServiceResult.Success
                ? OperationResult<TEntityIdType>.Ok(dataServiceResult.Value)
                : OperationResult<TEntityIdType>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult Update(TEntity entity)
        {
            using DbModificationScope dbModificationScope = CreateModificationScope();

            return this.EntityDataService.Update(entity, dbModificationScope);
        }

        /// <inheritdoc />
        public OperationResult Delete(TEntityIdType id)
        {
            using DbModificationScope dbModificationScope = CreateModificationScope();

            return this.EntityDataService.Delete(id, dbModificationScope);
        }

        /// <inheritdoc />
        public OperationResult DeleteRange(IEnumerable<TEntityIdType> ids)
        {
            using DbModificationScope dbModificationScope = CreateModificationScope();

            return this.EntityDataService.DeleteRange(ids, dbModificationScope);
        }

        /// <inheritdoc />
        public OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false)
        {
            using DbReaderScope dbReaderScope = CreateReaderScope();

            OperationResult<TEntity> dataServiceResult = this.EntityDataService.GetEntity(id, includeRelated, dbReaderScope);

            return dataServiceResult.Success
                ? OperationResult<TEntity>.Ok(dataServiceResult.Value)
                : OperationResult<TEntity>.Fail(dataServiceResult.Error);
        }

        /// <inheritdoc />
        public OperationResult<List<TEntity>> GetEntities(EntityServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters)
        {
            var dataServiceGetEntitiesParameters = new EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType>
            {
                Ids = parameters.Ids,
                OrderByDescending = parameters.OrderByDescending,
                OrderByProperty = parameters.OrderByProperty,
                Count = parameters.Count,
                IncludeRelated = parameters.IncludeRelated
            };

            using DbReaderScope dbReaderScope = CreateReaderScope();

            OperationResult<List<TEntity>> dataServiceResult = this.EntityDataService.GetEntities(
                dataServiceGetEntitiesParameters,
                dbReaderScope
            );

            return dataServiceResult.Success
                ? OperationResult<List<TEntity>>.Ok(dataServiceResult.Value)
                : OperationResult<List<TEntity>>.Fail(dataServiceResult.Error);
        }

        protected DbModificationScope CreateModificationScope() => 
            new DbModificationScope(_serviceProvider);

        protected DbReaderScope CreateReaderScope() => 
            new DbReaderScope(_serviceProvider);
    }
}
