using Data.Contracts.Tasks;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EF.Core.Tasks
{
    public class TaskOrmConfiguration : EntityOrmConfigurationBase<TaskOrm, int>
    {
        /// <inheritdoc />
        protected override string Table => "Tasks";

        /// <inheritdoc />
        protected override void ConfigureCore(EntityTypeBuilder<TaskOrm> builder)
        {
        }
    }
}
