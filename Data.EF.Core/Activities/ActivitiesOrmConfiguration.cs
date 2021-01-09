using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EF.Core.Activities
{
    public class ActivitiesOrmConfiguration : EntityOrmConfigurationBase<Projects.ProjectOrm, int>
    {
        /// <inheritdoc />
        protected override string Table => "Projects";

        /// <inheritdoc />
        protected override void ConfigureCore(EntityTypeBuilder<Projects.ProjectOrm> builder)
        {
        }
    }
}
