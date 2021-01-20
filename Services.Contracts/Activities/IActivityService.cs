using Contracts.Shared.Models;

using Services.Contracts.EntityServices;

namespace Services.Contracts.Activities
{
    public interface IActivityService : IEntityService<ActivityModel, int>
    {
    }
}
