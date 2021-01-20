using Contracts.Shared.Models;

using Data.Contracts.EntityDataServices;

namespace Data.Contracts.Activities
{
    public interface IActivityDataService : IEntityDataService<ActivityModel, int>
    {
    }
}
