using Data.Contracts.Activities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EF.Core.Activities
{
    public class ActivityOrmConfiguration : EntityOrmConfigurationBase<ActivityOrm, int>
    {
        /// <inheritdoc />
        protected override string Table => "Activities";

        /// <inheritdoc />
        protected override void ConfigureCore(EntityTypeBuilder<ActivityOrm> builder)
        {
        }
    }
}
