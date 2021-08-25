using System;

using Infrastructure.Shared;
using Infrastructure.Shared.Entities;

namespace Contracts.Shared.Models
{
    public class ProjectModel : EntityBase<int>, ICopyModel<ProjectModel>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }

        /// <inheritdoc />
        public ProjectModel CopyModelForCreate(DateTime? createdDateTimeUtc = null) =>
            new ProjectModel
            {
                Id = default,
                RowVersion = null,
                CreatedDateTimeUtc = createdDateTimeUtc ?? DateTime.UtcNow,
                ModifiedDateTimeUtc = null,
                Name = this.Name,
                Description = this.Description,
                IsHidden = this.IsHidden
            };

        /// <inheritdoc />
        public ProjectModel CopyModelForEdit(DateTime? modifiedDateTimeUtc = null) =>
            new ProjectModel
            {
                Id = this.Id,
                RowVersion = this.RowVersion,
                CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = modifiedDateTimeUtc ?? DateTime.UtcNow,
                Name = this.Name,
                Description = this.Description,
                IsHidden = this.IsHidden
            };
    }
}
