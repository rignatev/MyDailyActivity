using Data.Contracts.Activities;
using Data.Contracts.Projects;
using Data.Contracts.Tasks;
using Data.EF.Core.Activities;
using Data.EF.Core.Contexts;
using Data.EF.Core.Projects;
using Data.EF.Core.Tasks;
using Data.Migrations;
using Data.Migrations.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.EF.Core
{
    static public class DataServicesConfigurator
    {
        static public void ConfigureServices(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection
                .AddEntityFrameworkSqlite()
                .AddDbContext<AppDbContext, AppDbContext>(options => options.UseSqlite(connectionString));

            serviceCollection.AddScoped<ITaskDataService, TaskDataService<AppDbContext>>();
            serviceCollection.AddScoped<IProjectDataService, ProjectDataService<AppDbContext>>();
            serviceCollection.AddScoped<IActivityDataService, ActivityDataService<AppDbContext>>();
        }

        static public void InitializeDb()
        {
            new Migrator("Data Source=data.sqlite", typeof(M0_Initial).Assembly)
                .UpdateDatabase();
        }
    }
}
