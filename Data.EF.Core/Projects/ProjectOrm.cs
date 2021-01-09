using Data.EF.Core.Tasks;

namespace Data.EF.Core.Projects
{
    public class ProjectOrm : EntityOrmBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }
    }
}
