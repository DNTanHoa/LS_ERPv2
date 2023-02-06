using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.Customer
{
    public class CustomerDetailDtos
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Division { get; set; }
        public string PriceTerm { get; set; }
        public string PaymentTerm { get; set; }
        public string Currency { get; set; }

        public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
    }
}
