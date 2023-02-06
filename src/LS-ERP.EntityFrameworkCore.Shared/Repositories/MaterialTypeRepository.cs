using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class MaterialTypeRepository : IMaterialTypeRepository
    {
        private readonly AppDbContext context;

        public MaterialTypeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public MaterialType Add(MaterialType materialType)
        {
            throw new NotImplementedException();
        }

        public void Delete(MaterialType materialType)
        {
            throw new NotImplementedException();
        }

        public MaterialType GetBrand(string materialType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MaterialType> GetBrands()
        {
            return context.MaterialType;
        }

        public bool IsExist(string Code, out MaterialType materialType)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(string Code)
        {
            throw new NotImplementedException();
        }

        public void Update(MaterialType materialType)
        {
            throw new NotImplementedException();
        }
    }
}
