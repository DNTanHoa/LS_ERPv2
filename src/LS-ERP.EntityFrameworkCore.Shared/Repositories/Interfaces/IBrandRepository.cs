using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Brand Add(Brand brand);
        void Update(Brand brand);
        void Delete(Brand brand);
        IEnumerable<Brand> GetBrands();
        Brand GetBrand(string Code);
        bool IsExist(string Code, out Brand brand);
        bool IsExist(string Code);
    }
}
