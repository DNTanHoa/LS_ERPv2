namespace LS_ERP.Kanban.Models;
public class DepartmentModel
{
    public string Id { get; set; } = string.Empty;
    public string? CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string OtherName { get; set; } = string.Empty;
    public int SortIndex { get; set; }
  
    public string DefaultLunchMenuId { get; set; } = string.Empty;
    public string DefaultDinnerMenuId { get; set; } = string.Empty;
}
