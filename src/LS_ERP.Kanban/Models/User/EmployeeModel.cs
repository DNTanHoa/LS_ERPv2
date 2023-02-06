namespace LS_ERP.Kanban.Models
{
    public class EmployeeModel
    {
        public EmployeeModel()
        {
            PositionId = 3;
            isActive = true;
        }
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        //public CompanyModel? Company { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
       // public DepartmentModel? Department { get; set; }
        public int PositionId { get; set; }
        public string? PositionName { get; set; }
        //public PositionModel? Position { get; set; }
        public bool? isActive { get; set; }
    }
}
