using System;
using System.Linq;

using Contracts.Shared.Models;

using Data.Contracts.Activities;
using Data.Contracts.OperationScopes;
using Data.EF.Core.OperationScopes;
using Data.EF.Core.Utils;

using Infrastructure.Shared.OperationResult;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.Activities
{
    public class ActivityDataService<TDbContext> : EntityDataServiceBase<ActivityModel, int, ActivityOrm, int, TDbContext>,
        IActivityDataService
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public ActivityDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override ActivityModel ConvertToEntity(ActivityOrm entityOrm) =>
            entityOrm?.ToActivityModel(ConvertToEntityId);

        /// <inheritdoc />
        protected override int ConvertToEntityId(int entityOrmIdType) =>
            entityOrmIdType;

        /// <inheritdoc />
        protected override ActivityOrm ConvertToEntityOrm(ActivityModel entity) =>
            entity?.ToActivityOrm(ConvertToEntityOrmId);

        /// <inheritdoc />
        protected override int ConvertToEntityOrmId(int entityIdType) =>
            entityIdType;

        /// <inheritdoc />
        public OperationResult<ActivityModel> GetLatestActivity(IDbScope dbScope)
        {
            OperationResult<ActivityModel> result;

            try
            {
                TDbContext readerContext = GetReaderContext(dbScope);
                DbSet<ActivityOrm> activityDbSet = GetEntityDbSet(readerContext);

                ActivityOrm activityOrm = activityDbSet
                    .Include(x => x.Project)
                    .Include(x => x.Task)
                    .OrderByDescending(x => x.EndDateTimeUtc)
                    .FirstOrDefault();

                if (activityOrm != null)
                {
                    ActivityModel activity = ConvertToEntity(activityOrm);
                    result = OperationResult<ActivityModel>.Ok(activity);
                }
                else
                {
                    var error = new OperationError("No available activities.");
                    result = OperationResult<ActivityModel>.Fail(error);
                }
            }
            catch (Exception exception)
            {
                var error = new OperationError(exception);
                result = OperationResult<ActivityModel>.Fail(error);
            }

            return result;
        }
    }
}
