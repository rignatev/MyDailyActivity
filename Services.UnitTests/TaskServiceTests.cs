using System;
using System.Collections.Generic;
using System.Linq;

using Contracts.Shared.Models;

using Data.Migrations;
using Data.Migrations.Migrations;

using FluentAssertions;

using Infrastructure.Shared.OperationResult;

using Microsoft.Extensions.DependencyInjection;

using Services.Contracts.Activities;
using Services.Contracts.EntityServices;

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
                var random = new Random();
                int rndValue = random.Next(minValue: 1, maxValue: 100);
                DateTime nowUtc = DateTime.UtcNow.AddHours(rndValue);

                var task = new TaskModel
                {
                    Name = "Test task",
                    Description = "Some description",
                    IsHidden = false,
                    CreatedDateTimeUtc = nowUtc
                };

                var project = new ProjectModel
                {
                    Name = "Test project",
                    Description = "Some project description",
                    IsHidden = false,
                    CreatedDateTimeUtc = nowUtc
                };

                DateTime startDateTimeUtc = nowUtc.AddMinutes(value: -30);

                var activity = new ActivityModel
                {
                    Description = $"Test activity {rndValue}",
                    CreatedDateTimeUtc = nowUtc,
                    Task = task,
                    Project = project,
                    StartDateTimeUtc = startDateTimeUtc,
                    EndDateTimeUtc = nowUtc,
                    Duration = nowUtc - startDateTimeUtc,
                    IsHidden = false
                };

                var activityService = serviceScope.ServiceProvider.GetRequiredService<IActivityService>();
                
                OperationResult<int> activityCreateResult = activityService.Create(activity);

                activityCreateResult.Success.Should().BeTrue();

                OperationResult<ActivityModel> activityGetResult = activityService.GetEntity(
                    activityCreateResult.Value,
                    includeRelated: true
                );

                activityGetResult.Success.Should().BeTrue();

                ActivityModel activityFromDb = activityGetResult.Value;
                activityFromDb.Project.Name = "Changed name";
                activityFromDb.IsHidden = true;
                activityFromDb.Task.IsHidden = true;

                OperationResult activityUpdateResult = activityService.Update(activityFromDb);
                
                activityUpdateResult.Success.Should().BeTrue();

                activityGetResult = activityService.GetEntity(
                    activityFromDb.Id,
                    includeRelated: true
                );
                
                activityGetResult.Success.Should().BeTrue();

                var parameters = new EntityServiceGetEntitiesParameters<ActivityModel, int>
                {
                    IncludeRelated = true,
                    OrderByProperty = x => x.Task,
                    OrderByDescending = false
                };

                OperationResult<List<ActivityModel>> activityGetEntitiesResult = activityService.GetEntities(parameters);

                activityGetEntitiesResult.Success.Should().BeTrue();
            }
        }
    }
}
