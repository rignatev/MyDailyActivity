using FluentMigrator;

namespace Data.Migrations.Migrations
{
    [Migration(version: 0)]
    public class M0_Initial : NonReversibleMigration
    {
        /// <inheritdoc />
        public override void Up()
        {
            const string tasksTableName = "Tasks";
            const string projectsTableName = "Projects";
            const string activitiesTableName = "Activities";

            // @formatter:off

            this.Create.Table(tasksTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            this.Create.Table(projectsTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            this.Create.Table(activitiesTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("StartDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("EndDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("Duration").AsTime().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("ProjectId").AsInt32().Nullable().ForeignKey(projectsTableName, "Id")
                .WithColumn("TaskId").AsInt32().Nullable().ForeignKey(tasksTableName, "Id")
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            // @formatter:on
        }
    }
}
