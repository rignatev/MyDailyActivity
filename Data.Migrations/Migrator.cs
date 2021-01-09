using System;
using System.Reflection;

using FluentMigrator.Runner;

using Microsoft.Extensions.DependencyInjection;

namespace Data.Migrations
{
    public class Migrator
    {
        private readonly IServiceProvider _serviceProvider;

        public Migrator(string connectionString, Assembly assemblyWithMigrations)
        {
            _serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    runnerBuilder => runnerBuilder
                        .AddSQLite()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(assemblyWithMigrations)
                        .For.Migrations()
                )
                .BuildServiceProvider();
        }

        public void UpdateDatabase()
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            migrationRunner.MigrateUp();
        }
    }
}
