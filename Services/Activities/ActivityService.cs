using Contracts.Shared.Models;

using Data.Contracts.Activities;

using Services.Contracts.Activities;

namespace Services.Activities
{
    public class ActivityService : EntityServiceBase<ActivityModel, int, IActivityDataService>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IActivityDataService entityDataService) : base(entityDataService)
        {
        }
    }
}
