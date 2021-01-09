using System;

using FluentMigrator;

namespace Data.Migrations
{
    public abstract class NonReversibleMigration : Migration
    {
        /// <inheritdoc />
        public sealed override void Down()
        {
            throw new NotSupportedException("Downgrade migration is not supported.");
        }
    }
}
