using System.IO;

using Data.Migrations;
using Data.Migrations.Migrations;

using FluentAssertions;

using Xunit;

namespace Data.UnitTests
{
    public class MigrationTests
    {
        [Fact]
        public void UpdateDatabase_Should_Success()
        {
            const string sqliteFileName = "data.sqlite";

            if (File.Exists(sqliteFileName))
            {
                File.Delete(sqliteFileName);
            }

            var migrator = new Migrator($"Data Source={sqliteFileName}", typeof(M0_Initial).Assembly);
            migrator.UpdateDatabase();

            File.Exists(sqliteFileName).Should().BeTrue();
        }
    }
}
