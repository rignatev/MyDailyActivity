using System;

namespace Infrastructure.Shared.Entities
{
    public interface IEntity<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        TEntityIdType Id { get; set; }

        DateTime CreatedDateTimeUtc { get; set; }

        DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
