using System;

using Data.Contracts.EntityOrm;
using Data.Contracts.Projects;
using Data.Contracts.Tasks;

namespace Data.Contracts.Activities
{
    public class ActivityOrm : EntityOrmBase<int>
    {
        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public TimeSpan Duration { get; set; }

        public string Description { get; set; }

        public int? ProjectId { get; set; }

        public ProjectOrm Project { get; set; }

        public int? TaskId { get; set; }

        public TaskOrm Task { get; set; }
        
        public bool IsHidden { get; set; }
    }
}
