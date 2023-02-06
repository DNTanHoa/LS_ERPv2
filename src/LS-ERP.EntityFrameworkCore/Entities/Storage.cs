using System.Collections.Generic;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Storage : Audit
    {
        public Storage()
        {
            Code = string.Empty;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string PlantCode { get; set; }

        public virtual Plant Plant { get; set; }

        public virtual IList<StorageDetail> StorageDetails { get; set; }
    }
}