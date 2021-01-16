using System;
using System.Collections.Generic;

using Data.Shared;

using Infrastructure.Shared.OperationResult;

namespace Data.Contracts.EntityServices
{
    public interface IEntityDataService<TEntity, TEntityIdType>
        where TEntity : class, IEntityOrm<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        OperationResult<TEntityIdType> Create(TEntity entity);

        OperationResult Update(TEntity entity);

        OperationResult Delete(TEntityIdType id);

        OperationResult DeleteRange(IEnumerable<TEntityIdType> ids);

        OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated);

        OperationResult<IEnumerable<TEntity>> GetEntities(EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters);
    }
}
