using System;

using Data.Contracts;

namespace Data.EF.Core
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
