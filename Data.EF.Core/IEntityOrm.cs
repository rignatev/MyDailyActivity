using Shared.Infrastructure.Entities;

namespace Data.EF.Core
{
    public interface IEntityOrm<TIdType> : IEntity<TIdType>
    {
    }
}
