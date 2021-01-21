using System;
using System.Collections.Generic;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

namespace Services.Contracts.EntityServices
{
    public interface IEntityService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        OperationResult<TEntityIdType> Create(TEntity entity);

        OperationResult Update(TEntity entity);

        OperationResult Delete(TEntityIdType id);

        OperationResult DeleteRange(IEnumerable<TEntityIdType> ids);

        OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated = false);

        OperationResult<IEnumerable<TEntity>> GetEntities(EntityServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters);
    }
}
