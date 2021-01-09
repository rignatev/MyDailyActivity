namespace Data.EF.Core.Tasks
{
    public class TaskOrm : EntityOrmBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }
    }
}
