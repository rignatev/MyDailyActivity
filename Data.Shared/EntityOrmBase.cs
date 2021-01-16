using System;

namespace Data.Shared
{
    public class EntityOrmBase<TEntityIdType> : IEntityOrm<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        /// <inheritdoc />
        public TEntityIdType Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
