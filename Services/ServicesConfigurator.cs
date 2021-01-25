using Data.EF.Core;

using Microsoft.Extensions.DependencyInjection;

using Services.Activities;
using Services.Contracts.Activities;
using Services.Contracts.Projects;
using Services.Contracts.Tasks;
using Services.Projects;
using Services.Tasks;

namespace Services
{
    static public class ServicesConfigurator
    {
        static public void ConfigureServices(IServiceCollection serviceCollection, string connectionString)
        {
            DataServicesConfigurator.ConfigureServices(serviceCollection, connectionString);

            serviceCollection.AddScoped<ITaskService, TaskService>();
            serviceCollection.AddScoped<IProjectService, ProjectService>();
            serviceCollection.AddScoped<IActivityService, ActivityService>();
        }

        static public void InitializeDb()
        {
            DataServicesConfigurator.InitializeDb();
        }
    }
}
