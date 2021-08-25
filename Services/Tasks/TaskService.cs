using System;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;

using Services.Contracts.Tasks;

namespace Services.Tasks
{
    public class TaskService : EntityServiceBase<TaskModel, int, ITaskDataService>, ITaskService
    {
        /// <inheritdoc />
        public TaskService(IServiceProvider serviceProvider, ITaskDataService taskDataService)
            : base(serviceProvider, taskDataService)
        {
        }
    }
}
