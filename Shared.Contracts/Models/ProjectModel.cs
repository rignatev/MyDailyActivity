using Shared.Infrastructure.Entities;

namespace Shared.Contracts.Models
{
    public class ProjectModel : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }

        public TaskModel Task { get; set; }
    }
}
