using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.Kanban.Models;

public class DailyTargetModel
{
    public DailyTargetModel()
    {
        TotalQuantity = 0;
    }
    public int ID { get; set; }
    public string? Name { get; set; } 
    public string? CustomerID { get; set; }
    public string? CustomerName { get; set; }
    public string? StyleNO { get; set; }
    public string? WorkCenterID { get; set; }
    public string? WorkCenterName { get; set; }
    public decimal TotalTargetQuantity { get; set; }
    public decimal TotalQuantity { get; set; }
    public int NumberOfWorker { get; set; }
    public DateTime ProduceDate { get; set; }
    public DateTime InlineDate { get; set; }
    public string? Operation { get; set; }
    public virtual List<JobOutput>? JobOutput { get; set; }
}
