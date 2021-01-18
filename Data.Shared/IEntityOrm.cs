using System;

using Infrastructure.Shared.Entities;

namespace Data.Shared
{
    public interface IEntityOrm<TEntityOrmIdType> : IEntity<TEntityOrmIdType>
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
    }
}
