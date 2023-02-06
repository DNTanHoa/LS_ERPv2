namespace LS_ERP.Kanban.Models;
public class CompanyModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string OtherName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int SortIndex { get; set; }
}
