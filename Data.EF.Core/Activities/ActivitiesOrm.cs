using System;

using Data.EF.Core.Projects;
using Data.EF.Core.Tasks;

namespace Data.EF.Core.Activities
{
    public class ActivitiesOrm : EntityOrmBase<int>
    {
        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public TimeSpan Duration { get; set; }

        public string Description { get; set; }

        public int? ProjectId { get; set; }

        public ProjectOrm Project { get; set; }

        public int? TaskId { get; set; }

        public TaskOrm Task { get; set; }
    }
}
