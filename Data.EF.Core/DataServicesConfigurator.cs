using Data.Contracts.Tasks;
using Data.EF.Core.Contexts;
using Data.EF.Core.OperationScopes;
using Data.EF.Core.Tasks;

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

            serviceCollection.AddScoped<ReaderScope<AppDbContext>>();
            serviceCollection.AddScoped<ModificationScope<AppDbContext>>();
            serviceCollection.AddScoped<ITaskDataService, TaskDataService<AppDbContext>>();
        }
    }
}
