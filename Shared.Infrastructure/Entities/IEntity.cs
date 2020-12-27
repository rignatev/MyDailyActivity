using System;

namespace Shared.Infrastructure.Entities
{
    public interface IEntity
    {
        object Id { get; }

        DateTime CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }
    }

    public interface IEntity<TIdType> : IEntity
    {
        new TIdType Id { get; set; }
    }
}
