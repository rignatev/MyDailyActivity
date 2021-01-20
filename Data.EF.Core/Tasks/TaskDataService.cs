using System;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;
using Data.EF.Core.Utils;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.Tasks
{
    public class TaskDataService<TDbContext> : EntityDataServiceBase<TaskModel, int, TaskOrm, int, TDbContext>, ITaskDataService
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public TaskDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override TaskModel ConvertToEntity(TaskOrm entityOrm) =>
            entityOrm?.ToTaskModel(x => x);

        /// <inheritdoc />
        protected override int ConvertToEntityId(int entityOrmIdType) =>
            entityOrmIdType;

        /// <inheritdoc />
        protected override TaskOrm ConvertToEntityOrm(TaskModel entity) =>
            entity?.ToTaskOrm(x => x);

        /// <inheritdoc />
        protected override int ConvertToEntityOrmId(int entityIdType) =>
            entityIdType;
    }
}
