using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly AppDbContext context;

        public VendorRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Vendor Add(Vendor vendor)
        {
            return context.Add(vendor).Entity;
        }

        public void Delete(Vendor vendor)
        {
            context.Vendor.Remove(vendor);
        }

        public Vendor GetVendor(string ID)
        {
            return context.Vendor
                .FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<Vendor> GetVendors()
        {
            return context.Vendor.ToList();
        }

        public bool IsExist(string ID, out Vendor vendor)
        {
            vendor = null;
            vendor = context.Vendor.FirstOrDefault(x => x.ID == ID);
            return vendor != null ? true : false;
        }

        public bool IsExist(string ID)
        {
            var Vendor = context.Vendor.FirstOrDefault(x => x.ID == ID);
            return Vendor != null ? true : false;
        }

        public void Update(Vendor Vendor)
        {
            context.Entry(Vendor).State = EntityState.Modified;
        }
    }
}
