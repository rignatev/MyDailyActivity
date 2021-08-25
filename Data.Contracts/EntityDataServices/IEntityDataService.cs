using System;
using System.Collections.Generic;

using Data.Contracts.OperationScopes;

using Infrastructure.Shared.Entities;
using Infrastructure.Shared.OperationResult;

namespace Data.Contracts.EntityDataServices
{
    public interface IEntityDataService<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        OperationResult<TEntityIdType> Create(TEntity entity, IDbModificationScope dbModificationScope);

        OperationResult Update(TEntity entity, IDbModificationScope dbModificationScope);

        OperationResult Delete(TEntityIdType id, IDbModificationScope dbModificationScope);

        OperationResult DeleteRange(IEnumerable<TEntityIdType> ids, IDbModificationScope dbModificationScope);

        OperationResult<TEntity> GetEntity(TEntityIdType id, bool includeRelated, IDbScope dbScope);

        OperationResult<List<TEntity>> GetEntities(
            EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType> parameters,
            IDbScope dbScope);
    }
}
