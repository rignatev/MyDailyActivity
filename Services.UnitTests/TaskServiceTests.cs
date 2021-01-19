using System;

using Contracts.Shared.Models;

using Data.Migrations;
using Data.Migrations.Migrations;

using FluentAssertions;

using Infrastructure.Shared.OperationResult;

using Microsoft.Extensions.DependencyInjection;

using Services.Contracts.Tasks;

using Xunit;

namespace Services.UnitTests
{
    public class TaskServiceTests
    {
        [Fact]
        public void Test1()
        {
            const string connectionString = "Data Source = testData.sqlite";

            var migrator = new Migrator(connectionString, typeof(M0_Initial).Assembly);
            migrator.UpdateDatabase();

            var serviceCollection = new ServiceCollection();

            ServicesConfigurator.ConfigureServices(serviceCollection, connectionString);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            using (IServiceScope serviceScope = serviceProvider.CreateScope())
            {
                var taskService = serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

                var entity = new TaskModel
                {
                    Name = "Test task",
                    Description = "Some description",
                    IsHidden = false,
                    CreatedDateTimeUtc = DateTime.UtcNow
                };

                OperationResult<int> result = taskService.Create(entity);

                result.Success.Should().BeTrue();
            }
        }
    }
}
