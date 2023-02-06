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
    public class PartMaterialRepository : IPartMaterialRepository
    {
        private readonly AppDbContext context;

        public PartMaterialRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PartMaterial Add(PartMaterial partMaterial)
        {
            return context.PartMaterial.Add(partMaterial).Entity;
        }

        public void Delete(PartMaterial partMaterial)
        {
            context.PartMaterial.Remove(partMaterial);
        }

        public PartMaterial GetPartMaterial(long ID)
        {
            return context.PartMaterial.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<PartMaterial> GetPartMaterials(long partRevisionID)
        {
            return context.PartMaterial.Where(x => x.PartRevisionID == partRevisionID);
        }

        public IQueryable<PartMaterial> GetPartMaterials(List<string> partNumbers)
        {
            return context.PartMaterial.Where(x => partNumbers.Contains(x.PartNumber));
        }

        public void Update(PartMaterial partMaterial)
        {
            context.Entry(partMaterial).State = EntityState.Modified;
        }
    }
}
