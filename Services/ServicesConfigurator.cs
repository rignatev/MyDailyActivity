using Data.EF.Core;

using Microsoft.Extensions.DependencyInjection;

using Services.Contracts.Tasks;
using Services.Tasks;

namespace Services
{
    static public class ServicesConfigurator
    {
        static public void ConfigureServices(IServiceCollection serviceCollection, string connectionString)
        {
            DataServicesConfigurator.ConfigureServices(serviceCollection, connectionString);

            serviceCollection.AddScoped<ITaskService,TaskService>();
        }
    }
}
