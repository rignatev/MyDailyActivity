using System;

namespace Data.EF.Core
{
    public class EntityOrmBase<TIdType> : IEntityOrm<TIdType>
    {
        /// <inheritdoc />
        public TIdType Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
