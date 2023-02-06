using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.SalesOrder
{
    public class SalesOrderDetailDtos
    {
        public string ID { get; set; }
        public string Customer { get; set; }
        public string Brand { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string Division { get; set; }
        public string PaymentTerm { get; set; }
        public string PriceTerm { get; set; }
        public string Currency { get; set; }
        public int? Year { get; set; }
        public string SalesOrderStatus { get; set; }
        public string SalesOrderType { get; set; }
        public string SaveFilePath { get; set; }

        public List<ItemStyle> ItemStyles { get; set; }
        
    }
}
