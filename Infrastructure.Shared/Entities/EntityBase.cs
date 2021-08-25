using System;

namespace Infrastructure.Shared.Entities
{
    public abstract class EntityBase<TEntityIdType> : IEntity<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        /// <inheritdoc />
        public TEntityIdType Id { get; set; }

        /// <inheritdoc />
        public byte[] RowVersion { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
