using System;

using Infrastructure.Shared.Entities;

namespace Data.Contracts.EntityOrm
{
    public interface IEntityOrm<TEntityOrmIdType> : IEntity<TEntityOrmIdType>
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
    }
}
