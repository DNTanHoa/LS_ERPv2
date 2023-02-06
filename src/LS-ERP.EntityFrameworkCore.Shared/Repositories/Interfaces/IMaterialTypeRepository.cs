using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IMaterialTypeRepository
    {
        MaterialType Add(MaterialType materialType);
        void Update(MaterialType materialType);
        void Delete(MaterialType materialType);
        IEnumerable<MaterialType> GetBrands();
        MaterialType GetBrand(string materialType);
        bool IsExist(string Code, out MaterialType materialType);
        bool IsExist(string Code);
    }
}
