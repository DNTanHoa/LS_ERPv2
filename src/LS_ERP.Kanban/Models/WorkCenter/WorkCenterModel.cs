namespace LS_ERP.Kanban.Models
{
    public class WorkCenterModel
    {
        public WorkCenterModel()
        {
            AllowView = false;
            Timer = 60000;
        }
        public string ID { get; set; } = string.Empty;
        public string WorkCenterTypeID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DepartmentID { get; set; } = string.Empty;
        public int SortIndex { get; set; }
        public bool AllowView { get; set; } = false;
        public int Timer { get; set; }
    }
}
