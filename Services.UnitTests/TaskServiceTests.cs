using System;
using System.IO;

using Contracts.Shared.Models;

using FluentAssertions;
using FluentAssertions.Execution;

using Infrastructure.Shared.OperationResult;

using Microsoft.Extensions.DependencyInjection;

using Services.Contracts.Tasks;

using Xunit;

namespace Services.UnitTests
{
    public class TaskServiceTests : IDisposable
    {
        private readonly IServiceScope _serviceScope;

        public TaskServiceTests()
        {
            const string sqliteFileName = "TaskServiceTests.sqlite";

            if (File.Exists(sqliteFileName))
            {
                File.Delete(sqliteFileName);
            }

            var connectionString = $"Data Source = {sqliteFileName}";
            var serviceCollection = new ServiceCollection();

            ServicesConfigurator.ConfigureServices(serviceCollection, connectionString);
            ServicesConfigurator.InitializeDb(connectionString);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceScope = serviceProvider.CreateScope();
        }

        [Fact]
        public void TaskServiceCreate_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            var task = new TaskModel
            {
                Name = "Test task create",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            OperationResult<int> taskCreateResult = taskService.Create(task);

            taskCreateResult.Success.Should().BeTrue();
        }

        [Fact]
        public void TaskServiceUpdate_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            var task = new TaskModel
            {
                Name = "Test task before update",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            OperationResult<int> taskCreateResult = taskService.Create(task);
            taskCreateResult.Success.Should().BeTrue();

            OperationResult<TaskModel> taskGetEntityResult = taskService.GetEntity(id: taskCreateResult.Value);
            taskGetEntityResult.Success.Should().BeTrue();
            taskGetEntityResult.Value.Should().NotBeNull();

            TaskModel modifiedTask = taskGetEntityResult.Value;
            modifiedTask.Name = "Test task after update";

            OperationResult taskUpdateResult = taskService.Update(modifiedTask);
            taskUpdateResult.Success.Should().BeTrue();

            taskGetEntityResult = taskService.GetEntity(id: modifiedTask.Id);
            taskGetEntityResult.Success.Should().BeTrue();
            taskGetEntityResult.Value.Should().NotBeNull();
            taskGetEntityResult.Value.Name.Should().BeEquivalentTo(modifiedTask.Name);
        }

        [Fact]
        public void TaskServiceDelete_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            var task = new TaskModel
            {
                Name = "Test task delete",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            OperationResult<int> taskCreateResult = taskService.Create(task);
            taskCreateResult.Success.Should().BeTrue();

            OperationResult taskDeleteResult = taskService.Delete(id: taskCreateResult.Value);
            taskDeleteResult.Success.Should().BeTrue();

            OperationResult<TaskModel> taskGetEntityResult = taskService.GetEntity(id: taskCreateResult.Value);
            taskGetEntityResult.Success.Should().BeTrue();
            taskGetEntityResult.Value.Should().BeNull();
        }

        [Fact]
        public void TaskServiceDeleteRange_Should_Success()
        {
            using var assertionScope = new AssertionScope();

            var taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            var task1 = new TaskModel
            {
                Name = "Test task delete 1",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            OperationResult<int> task1CreateResult = taskService.Create(task1);
            task1CreateResult.Success.Should().BeTrue();

            var task2 = new TaskModel
            {
                Name = "Test task delete 2",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            OperationResult<int> task2CreateResult = taskService.Create(task2);
            task2CreateResult.Success.Should().BeTrue();

            OperationResult taskDeleteResult = taskService.DeleteRange(new[] { task1CreateResult.Value, task2CreateResult.Value });
            taskDeleteResult.Success.Should().BeTrue();

            OperationResult<TaskModel> taskGetEntityResult = taskService.GetEntity(id: task1CreateResult.Value);
            taskGetEntityResult.Success.Should().BeTrue();
            taskGetEntityResult.Value.Should().BeNull();

            taskGetEntityResult = taskService.GetEntity(id: task2CreateResult.Value);
            taskGetEntityResult.Success.Should().BeTrue();
            taskGetEntityResult.Value.Should().BeNull();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
