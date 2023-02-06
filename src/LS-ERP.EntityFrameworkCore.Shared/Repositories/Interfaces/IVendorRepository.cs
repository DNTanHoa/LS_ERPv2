using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Vendor Add(Vendor vendor);
        void Update(Vendor vendor);
        void Delete(Vendor vendor);
        IEnumerable<Vendor> GetVendors();
        Vendor GetVendor(string ID);
        bool IsExist(string ID);
        bool IsExist(string ID, out Vendor vendor);
    }
}
