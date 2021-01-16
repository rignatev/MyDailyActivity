using Infrastructure.Shared.Entities;

namespace Contracts.Shared.Models
{
    public class ProjectModel : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }
    }
}