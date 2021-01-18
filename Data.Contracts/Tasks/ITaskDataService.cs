using Data.Contracts.EntityDataServices;

namespace Data.Contracts.Tasks
{
    public interface ITaskDataService : IEntityDataService<TaskOrm, int>
    {
    }
}
