using System;

using Contracts.Shared.Models;

using Data.Contracts.Projects;
using Data.EF.Core.Utils;

using Microsoft.EntityFrameworkCore;

namespace Data.EF.Core.Projects
{
    public class ProjectDataService<TDbContext> : EntityDataServiceBase<ProjectModel, int, ProjectOrm, int, TDbContext>, IProjectDataService
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public ProjectDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override ProjectModel ConvertToEntity(ProjectOrm entityOrm) =>
            entityOrm?.ToProjectModel(ConvertToEntityId);

        /// <inheritdoc />
        protected override int ConvertToEntityId(int entityOrmIdType) =>
            entityOrmIdType;

        /// <inheritdoc />
        protected override ProjectOrm ConvertToEntityOrm(ProjectModel entity) =>
            entity?.ToProjectOrm(ConvertToEntityOrmId);

        /// <inheritdoc />
        protected override int ConvertToEntityOrmId(int entityIdType) =>
            entityIdType;
    }
}
