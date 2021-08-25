using System;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;

namespace Data.EF.Core.Utils
{
    static public class TaskOrmExtensions
    {
        static public TaskModel ToTaskModel(this TaskOrm taskOrm, Func<int, int> convertToTaskId) =>
            new()
            {
                Id = convertToTaskId(taskOrm.Id),
                RowVersion = taskOrm.RowVersion,
                CreatedDateTimeUtc = taskOrm.CreatedDateTimeUtc,
                ModifiedDateTimeUtc = taskOrm.ModifiedDateTimeUtc,
                Name = taskOrm.Name,
                Description = taskOrm.Description,
                IsHidden = taskOrm.IsHidden
            };
    }
}
