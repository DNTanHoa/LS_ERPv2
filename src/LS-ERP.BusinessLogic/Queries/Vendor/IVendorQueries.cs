using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IVendorQueries
    {
        IEnumerable<VendorDtos> GetVendors();
        IEnumerable<VendorDtos> GetByDescription(string description);
        IEnumerable<VendorSelectDtos> GetSelectVendor();
        VendorDtos GetVendor(string ID);
    }
}
