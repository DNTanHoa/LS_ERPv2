using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class CreateMultiItemPriceRequest
    {
        public List<ItemPrice> ItemPrices { get; set; }
    }
}
