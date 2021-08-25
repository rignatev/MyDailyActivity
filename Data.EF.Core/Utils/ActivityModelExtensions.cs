using System;

using Contracts.Shared.Models;

using Data.Contracts.Activities;

namespace Data.EF.Core.Utils
{
    static public class ActivityModelExtensions
    {
        static public ActivityOrm ToActivityOrm(this ActivityModel activityModel, Func<int, int> convertToActivityOrmId) =>
            new()
            {
                Id = convertToActivityOrmId(activityModel.Id),
                RowVersion = activityModel.RowVersion,
                CreatedDateTimeUtc = activityModel.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = activityModel.ModifiedDateTimeUtc,
                StartDateTimeUtc = activityModel.StartDateTimeUtc,
                EndDateTimeUtc = activityModel.EndDateTimeUtc,
                Duration = activityModel.Duration,
                Description = activityModel.Description,
                ProjectId = activityModel.Project?.Id,
                TaskId = activityModel.Task?.Id,
                // TODO: After row version was added, Project and Task row versions always updating with activity.
                // Project = activityModel.Project?.ToProjectOrm(x => x),
                // Task = activityModel.Task?.ToTaskOrm(x => x),
                IsHidden = activityModel.IsHidden
            };
    }
}
