using System;

using Data.Contracts.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.Tasks
{
    public class TaskDataService<TDbContext> : EntityDataServiceBase<TaskOrm, int, TDbContext>, ITaskDataService
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public TaskDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
