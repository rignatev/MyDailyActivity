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

            string rowVersionTriggerSql = @"
                CREATE TRIGGER Set{0}RowVersionOn{1}
                AFTER {1} ON {0}
                BEGIN
                    UPDATE {0}
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ";

            this.Create.Table(tasksTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("RowVersion").AsByte().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            Execute.Sql(string.Format(rowVersionTriggerSql, tasksTableName, "UPDATE"));
            Execute.Sql(string.Format(rowVersionTriggerSql, tasksTableName, "INSERT"));

            this.Create.Table(projectsTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("RowVersion").AsByte().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            Execute.Sql(string.Format(rowVersionTriggerSql, projectsTableName, "UPDATE"));
            Execute.Sql(string.Format(rowVersionTriggerSql, projectsTableName, "INSERT"));

            this.Create.Table(activitiesTableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CreatedDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("ModifiedDateTimeUtc").AsDateTime2().Nullable()
                .WithColumn("RowVersion").AsByte().Nullable()
                .WithColumn("StartDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("EndDateTimeUtc").AsDateTime2().NotNullable()
                .WithColumn("Duration").AsTime().NotNullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("ProjectId").AsInt32().Nullable().ForeignKey(projectsTableName, "Id")
                .WithColumn("TaskId").AsInt32().Nullable().ForeignKey(tasksTableName, "Id")
                .WithColumn("IsHidden").AsBoolean().NotNullable();

            Execute.Sql(string.Format(rowVersionTriggerSql, activitiesTableName, "UPDATE"));
            Execute.Sql(string.Format(rowVersionTriggerSql, activitiesTableName, "INSERT"));

            // @formatter:on
        }
    }
}
