using System;

using Contracts.Shared.Models;

using Data.Contracts.Activities;
using Data.EF.Core.Utils;

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
    }
}
