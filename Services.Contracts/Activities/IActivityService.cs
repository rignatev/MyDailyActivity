using Contracts.Shared.Models;

using Infrastructure.Shared.OperationResult;

using Services.Contracts.EntityServices;

namespace Services.Contracts.Activities
{
    public interface IActivityService : IEntityService<ActivityModel, int>
    {
        OperationResult<ActivityModel> CreateInitialActivity();
    }
}
