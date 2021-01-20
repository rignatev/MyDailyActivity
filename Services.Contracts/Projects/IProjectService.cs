using Contracts.Shared.Models;

using Services.Contracts.EntityServices;

namespace Services.Contracts.Projects
{
    public interface IProjectService : IEntityService<ProjectModel, int>
    {
    }
}
