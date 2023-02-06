using System;

namespace LS_ERP.Mobile.Models
{
    public class EmployeesDataTransferObject
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }        
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool? isActive { get; set; }
    }
}