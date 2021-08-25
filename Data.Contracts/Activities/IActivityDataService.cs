using Contracts.Shared.Models;

using Data.Contracts.EntityDataServices;
using Data.Contracts.OperationScopes;

using Infrastructure.Shared.OperationResult;

namespace Data.Contracts.Activities
{
    public interface IActivityDataService : IEntityDataService<ActivityModel, int>
    {
        OperationResult<ActivityModel> GetLatestActivity(IDbScope dbScope);
    }
}
