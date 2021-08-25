using System;

using Contracts.Shared.Models;

using Data.Contracts.Projects;

namespace Data.EF.Core.Utils
{
    static public class ProjectOrmExtensions
    {
        static public ProjectModel ToProjectModel(this ProjectOrm projectOrm, Func<int, int> convertToProjectId) =>
            new()
            {
                Id = convertToProjectId(projectOrm.Id),
                RowVersion = projectOrm.RowVersion,
                CreatedDateTimeUtc = projectOrm.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = projectOrm.ModifiedDateTimeUtc,
                Name = projectOrm.Name,
                Description = projectOrm.Description,
                IsHidden = projectOrm.IsHidden
            };
    }
}
