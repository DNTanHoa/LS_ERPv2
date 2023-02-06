using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class SalesContractDto
    {
        public string ID { get; set; }
        public string Number { get; set; }
        public string CustomerID { get; set; }
        public string FileName { get; set; }
        public string SaveFilePath { get; set; }
        public IList<SalesContractDetailDto> ContractDetails { get; set; } = new List<SalesContractDetailDto>();
    }
}
