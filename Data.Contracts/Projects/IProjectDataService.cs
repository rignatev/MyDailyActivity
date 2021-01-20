using Contracts.Shared.Models;

using Data.Contracts.EntityDataServices;

namespace Data.Contracts.Projects
{
    public interface IProjectDataService : IEntityDataService<ProjectModel, int>
    {
    }
}
