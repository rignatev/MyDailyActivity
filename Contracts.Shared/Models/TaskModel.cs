using System;

using Infrastructure.Shared;
using Infrastructure.Shared.Entities;

namespace Contracts.Shared.Models
{
    public class TaskModel : EntityBase<int>, ICopyModel<TaskModel>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }

        /// <inheritdoc />
        public TaskModel CopyModelForCreate(DateTime? createdDateTimeUtc = null) =>
            new TaskModel
            {
                Id = default,
                CreatedDateTimeUtc = createdDateTimeUtc ?? DateTime.UtcNow,
                ModifiedDateTimeUtc = null,
                Name = this.Name,
                Description = this.Description,
                IsHidden = this.IsHidden
            };

        /// <inheritdoc />
        public TaskModel CopyModelForEdit(DateTime? modifiedDateTimeUtc = null) =>
            new TaskModel
            {
                Id = this.Id,
                CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = modifiedDateTimeUtc ?? DateTime.UtcNow,
                Name = this.Name,
                Description = this.Description,
                IsHidden = this.IsHidden
            };
    }
}
