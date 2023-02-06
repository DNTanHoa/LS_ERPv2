using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPartMaterialRepository
    {
        PartMaterial Add(PartMaterial partMaterial);
        void Update(PartMaterial partMaterial);
        void Delete(PartMaterial partMaterial);
        IQueryable<PartMaterial> GetPartMaterials(long partRevisionID);
        IQueryable<PartMaterial> GetPartMaterials(List<string> partNumbers);
        PartMaterial GetPartMaterial(long ID);
    }
}
