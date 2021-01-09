using System;

namespace Shared.Infrastructure.Entities
{
    public interface IEntity<TIdType>
    {
        TIdType Id { get; set; }

        DateTime CreatedDateTimeUtc { get; set; }

        DateTime? ModifiedDateTimeUtc { get; set; }
    }
}
