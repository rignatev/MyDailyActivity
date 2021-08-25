using System;

using Contracts.Shared.Models;

using Data.Contracts.Activities;
using Data.EF.Core.OperationScopes;

using Infrastructure.Shared.OperationResult;
using Infrastructure.Shared.Utils;

using Services.Contracts.Activities;

namespace Services.Activities
{
    public class ActivityService : EntityServiceBase<ActivityModel, int, IActivityDataService>, IActivityService
    {
        /// <inheritdoc />
        public ActivityService(IServiceProvider serviceProvider, IActivityDataService entityDataService) 
            : base(serviceProvider, entityDataService)
        {
        }

        /// <inheritdoc />
        public OperationResult<ActivityModel> CreateInitialActivity()
        {
            using DbReaderScope dbReaderScope = CreateReaderScope();

            OperationResult<ActivityModel> getLatestActivityResult = this.EntityDataService.GetLatestActivity(dbReaderScope);

            var initialActivity = new ActivityModel();

            if (getLatestActivityResult.Success)
            {
                initialActivity.StartDateTimeUtc = getLatestActivityResult.Value.EndDateTimeUtc;
                initialActivity.Description = getLatestActivityResult.Value.Description;
                initialActivity.Project = getLatestActivityResult.Value.Project;
                initialActivity.Task = getLatestActivityResult.Value.Task;
            }
            else
            {
                initialActivity.StartDateTimeUtc = DateTime.UtcNow.TrimToSeconds();
            }

            return OperationResult<ActivityModel>.Ok(initialActivity);
        }
    }
}
