using System;

using Infrastructure.Shared.Entities;

namespace Data.Shared
{
    public interface IEntityOrm<TEntityIdType> : IEntity<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
    }
}
