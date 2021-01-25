using System;

using Infrastructure.Shared;
using Infrastructure.Shared.Entities;

namespace Contracts.Shared.Models
{
    public class ActivityModel : EntityBase<int>, ICopyModel<ActivityModel>
    {
        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public TimeSpan Duration { get; set; }

        public string Description { get; set; }

        public ProjectModel Project { get; set; }

        public TaskModel Task { get; set; }

        public bool IsHidden { get; set; }

        /// <inheritdoc />
        public ActivityModel CopyModelForCreate(DateTime? createdDateTimeUtc = null)
        {
            createdDateTimeUtc ??= DateTime.UtcNow;

            return new ActivityModel()
            {
                Id = default,
                CreatedDateTimeUtc = (DateTime)createdDateTimeUtc,
                ModifiedDateTimeUtc = null,
                Description = this.Description,
                IsHidden = this.IsHidden,
                StartDateTimeUtc = this.StartDateTimeUtc,
                EndDateTimeUtc = this.EndDateTimeUtc,
                Duration = this.Duration,
                Project = this.Project.CopyModelForCreate((DateTime)createdDateTimeUtc),
                Task = this.Task.CopyModelForCreate((DateTime)createdDateTimeUtc)
            };
        }

        /// <inheritdoc />
        public ActivityModel CopyModelForEdit(DateTime? modifiedDateTimeUtc = null)
        {
            modifiedDateTimeUtc ??= DateTime.UtcNow;

            return new ActivityModel()
            {
                Id = this.Id,
                CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = (DateTime)modifiedDateTimeUtc,
                Description = this.Description,
                IsHidden = this.IsHidden,
                StartDateTimeUtc = this.StartDateTimeUtc,
                EndDateTimeUtc = this.EndDateTimeUtc,
                Duration = this.Duration,
                Project = this.Project.CopyModelForEdit((DateTime)modifiedDateTimeUtc),
                Task = this.Task.CopyModelForEdit((DateTime)modifiedDateTimeUtc)
            };
        }
    }
}
