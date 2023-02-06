namespace LS_ERP.Kanban.Models
{
    public class DailyTargetOverviewModel
    {
        public DailyTargetOverviewModel()
        {
            Efficiency = 0;
        }
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string StyleNO { get; set; } = string.Empty;
        public string WorkCenterID { get; set; } = string.Empty;
        public string WorkCenterName { get; set; } = string.Empty;
        public decimal TotalTargetQuantity { get; set; }
        public decimal TotalOutputQuantity { get; set; }
        public int NumberOfWorker { get; set; }
        public DateTime ProduceDate { get; set; }
        public string Problem { get; set; } = string.Empty;
        public DateTime InlineDate { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string EfficiencyStr { get; set; } = string.Empty;
        public decimal? Efficiency { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
