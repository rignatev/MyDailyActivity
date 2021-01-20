using System;

using Infrastructure.Shared.Entities;

namespace Contracts.Shared.Models
{
    public class ActivityModel : EntityBase<int>
    {
        public DateTime StartDateTimeUtc { get; set; }

        public DateTime EndDateTimeUtc { get; set; }

        public TimeSpan Duration { get; set; }

        public string Description { get; set; }

        public ProjectModel Project { get; set; }

        public TaskModel Task { get; set; }

        public bool IsHidden { get; set; }
    }
}
