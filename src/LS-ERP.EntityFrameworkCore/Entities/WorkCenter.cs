using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class WorkCenter : Audit
    {
        public WorkCenter()
        {
            ID = string.Empty;
        }
        public string ID { get; set; }
        public string PlantCode { get; set; }
        public string WorkCenterTypeID { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public string Postion { get; set; }
        public string Image { get; set; }
        public string DepartmentID { get; set; }
        public int SortIndex { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual WorkCenterType WorkCenterType { get; set; }
    }
}
