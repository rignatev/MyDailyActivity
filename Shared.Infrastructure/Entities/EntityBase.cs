using System;

namespace Shared.Infrastructure.Entities
{
    public class EntityBase<TIdType> : IEntity<TIdType>
    {
        /// <inheritdoc />
        public TIdType Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
