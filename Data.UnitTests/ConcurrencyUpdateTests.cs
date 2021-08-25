using System;
using System.IO;

using Contracts.Shared.Models;

using Data.Contracts.Tasks;
using Data.EF.Core;
using Data.EF.Core.OperationScopes;

using FluentAssertions;

using Infrastructure.Shared.OperationResult;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Data.UnitTests
{
    public class ConcurrencyUpdateTests : IDisposable
    {
        private readonly IServiceScope _serviceScope;

        public ConcurrencyUpdateTests()
        {
            const string sqliteFileName = "ConcurrencyUpdateTests.sqlite";

            if (File.Exists(sqliteFileName))
            {
                File.Delete(sqliteFileName);
            }

            var connectionString = $"Data Source = {sqliteFileName}";
            var serviceCollection = new ServiceCollection();

            DataServicesConfigurator.ConfigureServices(serviceCollection, connectionString);
            DataServicesConfigurator.InitializeDb(connectionString);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceScope = serviceProvider.CreateScope();
        }

        [Fact]
        public void ConcurrencyUpdateTask_Should_Fail()
        {
            var taskDataService = _serviceScope.ServiceProvider.GetRequiredService<ITaskDataService>();

            var task = new TaskModel
            {
                Name = "Test task",
                Description = "Some description",
                IsHidden = false,
                CreatedDateTimeUtc = DateTime.UtcNow
            };

            int taskId;
            using (var dbModificationScope = new DbModificationScope(_serviceScope.ServiceProvider))
            {
                OperationResult<int> taskCreateResult = taskDataService.Create(task, dbModificationScope);
                dbModificationScope.CommitIfSuccess(taskCreateResult.Success);
                taskCreateResult.Success.Should().BeTrue();

                taskId = taskCreateResult.Value;
            }

            TaskModel taskFromDb;
            using (var dbReaderScope = new DbReaderScope(_serviceScope.ServiceProvider))
            {
                OperationResult<TaskModel> taskGetEntityResult = taskDataService.GetEntity(taskId, includeRelated: false, dbReaderScope);
                taskGetEntityResult.Success.Should().BeTrue();
                taskGetEntityResult.Value.Should().NotBeNull();

                taskFromDb = taskGetEntityResult.Value;
            }

            TaskModel taskFromDbFirstCopy = taskFromDb.CopyModelForEdit();
            TaskModel taskFromDbSecondCopy = taskFromDb.CopyModelForEdit();

            taskFromDbFirstCopy.Name = "Test task first modification";
            taskFromDbSecondCopy.Name = "Test task second modification";

            using (var dbModificationScope = new DbModificationScope(_serviceScope.ServiceProvider))
            {
                OperationResult taskUpdateResult = taskDataService.Update(taskFromDbFirstCopy, dbModificationScope);
                dbModificationScope.CommitIfSuccess(taskUpdateResult.Success);
                taskUpdateResult.Success.Should().BeTrue();
            }

            using (var dbModificationScope = new DbModificationScope(_serviceScope.ServiceProvider))
            {
                OperationResult taskUpdateResult = taskDataService.Update(taskFromDbSecondCopy, dbModificationScope);
                dbModificationScope.CommitIfSuccess(taskUpdateResult.Success);
                taskUpdateResult.Success.Should().BeFalse();
            }

            using (var dbReaderScope = new DbReaderScope(_serviceScope.ServiceProvider))
            {
                OperationResult<TaskModel> taskGetEntityResult = taskDataService.GetEntity(taskId, includeRelated: false, dbReaderScope);
                taskGetEntityResult.Success.Should().BeTrue();
                taskGetEntityResult.Value.Should().NotBeNull();
                taskGetEntityResult.Value.Name.Should().BeEquivalentTo(taskFromDbFirstCopy.Name);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
