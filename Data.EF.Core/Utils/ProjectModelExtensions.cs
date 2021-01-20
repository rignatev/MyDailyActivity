using System;

using Contracts.Shared.Models;

using Data.Contracts.Projects;

namespace Data.EF.Core.Utils
{
    static public class ProjectModelExtensions
    {
        static public ProjectOrm ToProjectOrm(this ProjectModel projectModel, Func<int, int> convertToProjectOrmId) =>
            new()
            {
                Id = convertToProjectOrmId(projectModel.Id),
                CreatedDateTimeUtc = projectModel.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = projectModel.ModifiedDateTimeUtc,
                Name = projectModel.Name,
                Description = projectModel.Description,
                IsHidden = projectModel.IsHidden
            };
    }
}
