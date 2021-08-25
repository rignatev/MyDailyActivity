using System;

namespace Infrastructure.Shared.Entities
{
    public interface IEntity<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        /// <summary>
        /// Gets or sets identifier for entity of type <see cref="TEntityIdType"/>
        /// </summary>
        TEntityIdType Id { get; set; }

        /// <summary>
        /// Gets or sets entity version.
        /// </summary>
        public byte[] RowVersion{ get; set; }

        /// <summary>
        /// Gets or sets created date and time in Utc.
        /// </summary>
        DateTime CreatedDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets modified date and time in Utc.
        /// </summary>
        DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
