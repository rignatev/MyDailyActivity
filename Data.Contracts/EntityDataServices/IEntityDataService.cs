using System;
using System.Collections.Generic;

using Data.Shared;

using Infrastructure.Shared.OperationResult;

namespace Data.Contracts.EntityDataServices
{
    public interface IEntityDataService<TEntityOrm, TEntityOrmIdType>
        where TEntityOrm : class, IEntityOrm<TEntityOrmIdType>, new()
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
        OperationResult<TEntityOrmIdType> Create(TEntityOrm entity);

        OperationResult Update(TEntityOrm entity);

        OperationResult Delete(TEntityOrmIdType id);

        OperationResult DeleteRange(IEnumerable<TEntityOrmIdType> ids);

        OperationResult<TEntityOrm> GetEntity(TEntityOrmIdType id, bool includeRelated);

        OperationResult<IEnumerable<TEntityOrm>> GetEntities(
            EntityDataServiceGetEntitiesParameters<TEntityOrm, TEntityOrmIdType> parameters);
    }
}
