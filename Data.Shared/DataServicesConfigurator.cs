using Data.Shared.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Shared
{
    static public class DataServicesConfigurator
    {
        static public void ConfigureServices(IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection
                .AddEntityFrameworkSqlite()
                .AddDbContext<AppDbContext, AppDbContext>(options => options.UseSqlite(connectionString));

        }
    }
}
