namespace LS_ERP.Kanban.Models
{
    public class DailyTargetCompareOverviewModel
    {
        public string StyleNO { get; set; } = string.Empty;
        public List<DailyTargetOverviewModel>? DailyTargetOverviewModels { get; set; }
    }
}
