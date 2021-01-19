using System;

namespace Data.Shared
{
    public abstract class EntityOrmBase<TEntityOrmIdType> : IEntityOrm<TEntityOrmIdType>
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
        /// <inheritdoc />
        public TEntityOrmIdType Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
