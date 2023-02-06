using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ForecastDetail
    {
        public long ID { get; set; }
        public string ForecastOverallID { get; set; }
        public string GarmentSize { get; set; }
        public string ItemCode { get; set; }
        public int SizeSortIndex { get; set; }
        public int? PlannedQuantity { get; set; }
        public decimal? Price { get; set; }

        public virtual ForecastOverall ForecastOverall { get; set; }

        public virtual IList<ForecastMaterial> ForecastMaterials { get; set; }
    }
}
