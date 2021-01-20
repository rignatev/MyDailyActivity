using Data.Contracts.EntityOrm;

namespace Data.Contracts.Projects
{
    public class ProjectOrm : EntityOrmBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }
    }
}
