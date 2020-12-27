using System;

namespace Shared.Infrastructure.Entities
{
    public class EntityBase<TIdType> : IEntity<TIdType>
    {
        /// <inheritdoc />
        object IEntity.Id => this.Id;

        /// <inheritdoc />
        public TIdType Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDate { get; set; }

        /// <inheritdoc />
        public DateTime? ModifiedDate { get; set; }
    }
}
