using System;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;

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
        protected override TaskModel ConvertToEntity(TaskOrm entityOrm)
        {
            if (entityOrm == null)
            {
                return null;
            }

            return new TaskModel
            {
                Id = ConvertToEntityId(entityOrm.Id),
                CreatedDateTimeUtc = entityOrm.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = entityOrm.ModifiedDateTimeUtc,
                Name = entityOrm.Name,
                Description = entityOrm.Description,
                IsHidden = entityOrm.IsHidden
            };
        }

        /// <inheritdoc />
        protected override int ConvertToEntityId(int entityOrmIdType) =>
            entityOrmIdType;

        /// <inheritdoc />
        protected override TaskOrm ConvertToEntityOrm(TaskModel entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new TaskOrm
            {
                Id = ConvertToEntityOrmId(entity.Id),
                CreatedDateTimeUtc = entity.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = entity.ModifiedDateTimeUtc,
                Name = entity.Name,
                Description = entity.Description,
                IsHidden = entity.IsHidden
            };
        }

        /// <inheritdoc />
        protected override int ConvertToEntityOrmId(int entityIdType) =>
            entityIdType;
    }
}
