using Contracts.Shared.Models;

using Data.Contracts.Projects;

using Services.Contracts.Projects;

namespace Services.Projects
{
    public class ProjectService : EntityServiceBase<ProjectModel, int, IProjectDataService>, IProjectService
    {
        /// <inheritdoc />
        public ProjectService(IProjectDataService entityDataService) : base(entityDataService)
        {
        }
    }
}
