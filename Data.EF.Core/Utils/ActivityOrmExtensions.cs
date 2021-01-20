using System;

using Contracts.Shared.Models;

using Data.Contracts.Activities;

namespace Data.EF.Core.Utils
{
    static public class ActivityOrmExtensions
    {
        static public ActivityModel ToActivityModel(this ActivityOrm activityOrm, Func<int, int> convertToActivityId) =>
            new()
            {
                Id = convertToActivityId(activityOrm.Id),
                CreatedDateTimeUtc = activityOrm.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = activityOrm.ModifiedDateTimeUtc,
                StartDateTimeUtc = activityOrm.StartDateTimeUtc,
                EndDateTimeUtc = activityOrm.EndDateTimeUtc,
                Duration = activityOrm.Duration,
                Description = activityOrm.Description,
                Project = activityOrm.Project?.ToProjectModel(x => x),
                Task = activityOrm.Task?.ToTaskModel(x => x),
                IsHidden = activityOrm.IsHidden
            };
    }
}
