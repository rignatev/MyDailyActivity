using Contracts.Shared.Models;

using Services.Contracts.EntityServices;

namespace Services.Contracts.Tasks
{
    public interface ITaskService : IEntityService<TaskModel, int>
    {
    }
}
