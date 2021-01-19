using Contracts.Shared.Models;

using Data.Contracts.Tasks;

using Services.Contracts.Tasks;

namespace Services.Tasks
{
    public class TaskService : EntityServiceBase<TaskModel, int, ITaskDataService, TaskOrm, int>, ITaskService
    {
        /// <inheritdoc />
        public TaskService(ITaskDataService taskDataService) : base(taskDataService)
        {
        }

        /// <inheritdoc />
        protected override int ConvertToEntityId(int entityOrmIdType) =>
            entityOrmIdType;

        /// <inheritdoc />
        protected override TaskModel ConvertToEntity(TaskOrm entityOrm) =>
            new()
            {
                Id = ConvertToEntityId(entityOrm.Id),
                CreatedDateTimeUtc = entityOrm.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = entityOrm.ModifiedDateTimeUtc,
                Name = entityOrm.Name,
                Description = entityOrm.Description,
                IsHidden = entityOrm.IsHidden
            };

        /// <inheritdoc />
        protected override int ConvertToEntityOrmId(int entityIdType) =>
            entityIdType;

        /// <inheritdoc />
        protected override TaskOrm ConvertToEntityOrm(TaskModel entity) =>
            new()
            {
                Id = ConvertToEntityOrmId(entity.Id),
                CreatedDateTimeUtc = entity.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = entity.ModifiedDateTimeUtc,
                Name = entity.Name,
                Description = entity.Description,
                IsHidden = entity.IsHidden
            };
    }
}
