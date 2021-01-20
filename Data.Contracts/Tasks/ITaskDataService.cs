using Contracts.Shared.Models;

using Data.Contracts.EntityDataServices;

namespace Data.Contracts.Tasks
{
    public interface ITaskDataService : IEntityDataService<TaskModel, int>
    {
    }
}
