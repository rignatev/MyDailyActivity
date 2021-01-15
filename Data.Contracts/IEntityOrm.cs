using System;

using Shared.Infrastructure.Entities;

namespace Data.Contracts
{
    public interface IEntityOrm<TEntityIdType> : IEntity<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
    }
}
