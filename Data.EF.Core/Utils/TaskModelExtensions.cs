using System;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;

namespace Data.EF.Core.Utils
{
    static public class TaskModelExtensions
    {
        static public TaskOrm ToTaskOrm(this TaskModel taskModel, Func<int, int> convertToTaskOrmId) =>
            new()
            {
                Id = convertToTaskOrmId(taskModel.Id),
                RowVersion = taskModel.RowVersion,
                CreatedDateTimeUtc = taskModel.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = taskModel.ModifiedDateTimeUtc,
                Name = taskModel.Name,
                Description = taskModel.Description,
                IsHidden = taskModel.IsHidden
            };
    }
}
