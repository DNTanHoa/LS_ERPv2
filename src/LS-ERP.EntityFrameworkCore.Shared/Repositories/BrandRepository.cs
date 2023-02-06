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
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext context;

        public BrandRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Brand Add(Brand brand)
        {
            return context.Add(brand).Entity;
        }

        public void Delete(Brand brand)
        {
            context.Brand.Remove(brand);
        }

        public Brand GetBrand(string Code)
        {
            return context.Brand
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.Code == Code);
        }

        public IEnumerable<Brand> GetBrands()
        {
            return context.Brand.ToList();
        }

        public bool IsExist(string Code, out Brand brand)
        {
            brand = null;
            brand = context.Brand.FirstOrDefault(x => x.Code == Code);
            return brand != null ? true : false;
        }

        public bool IsExist(string Code)
        {
            var brand = context.Brand.FirstOrDefault(x => x.Code == Code);
            return brand != null ? true : false;
        }

        public void Update(Brand brand)
        {
            context.Entry(brand).State = EntityState.Modified;
        }
    }
}
