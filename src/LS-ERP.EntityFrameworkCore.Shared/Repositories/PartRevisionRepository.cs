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
    public class PartRevisionRepository : IPartRevisionRepository
    {
        private readonly AppDbContext context;

        public PartRevisionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PartRevision Add(PartRevision partRevision)
        {
            return context.PartRevision.Add(partRevision).Entity;
        }

        public void Delete(PartRevision partRevision)
        {
            context.PartRevision.Remove(partRevision);
        }

        public PartRevision GetLastestVersion(string partNumber)
        {
            return context.PartRevision
                  .OrderByDescending(x => x.EffectDate)
                  .Include(x => x.PartMaterials)
                  .FirstOrDefault(x => x.PartNumber == partNumber);
        }

        public PartRevision GetPartRevision(long ID)
        {
            var query = from partRevision in context.PartRevision
                        join partMaterial in context.PartMaterial
                            on partRevision.ID equals partMaterial.PartRevisionID into part
                        from partDetail in part.DefaultIfEmpty()
                        where partRevision.ID == ID
                        select new PartRevision()
                        {
                            ID = partRevision.ID,
                            PartNumber = partRevision.PartNumber,
                            RevisionNumber = partRevision.RevisionNumber,
                            EffectDate = partRevision.EffectDate,
                            IsConfirmed = partRevision.IsConfirmed,
                            Season = partRevision.Season,
                            CreatedAt = partRevision.CreatedAt,
                            LastUpdatedAt = partRevision.LastUpdatedAt,
                            CreatedBy = partRevision.CreatedBy,
                            LastUpdatedBy = partRevision.LastUpdatedBy,
                            PartMaterials = partRevision.PartMaterials
                        };

            return query.FirstOrDefault();
        }

        public IQueryable<PartRevision> GetPartRevisions()
        {
            return context.PartRevision
                          .Include(x => x.PartMaterials);
        }

        public IQueryable<PartRevision> GetLastestPartRevisions(List<string> partNumbers)
        {
            return context.PartRevision
                   .OrderByDescending(x => x.EffectDate)
                   .Include(x => x.PartMaterials)
                   .ThenInclude(y => y.PriceUnit)
                   .Include(x => x.PartMaterials)
                   .ThenInclude(y => y.PerUnit)
                   .Where(x => partNumbers.Contains(x.PartNumber.Trim()));
        }

        public bool IsExist(long ID, out PartRevision partRevision)
        {
            partRevision = null;
            partRevision = context.PartRevision.FirstOrDefault(x => x.ID == ID);
            return partRevision != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var partRevision = context.PartRevision.FirstOrDefault(x => x.ID == ID);
            return partRevision != null ? true : false;
        }

        public bool IsExist(string PartNumber, string RevisionNumber, string Season)
        {
            var partRevision = context.PartRevision.FirstOrDefault(x => x.PartNumber == PartNumber &&
                                                                        x.RevisionNumber == RevisionNumber &&
                                                                        x.Season == Season);
            return partRevision != null ? true : false;
        }

        public void Update(PartRevision partRevision)
        {
            context.Entry(partRevision).State = EntityState.Modified;
        }

        public PartRevision GetLastestVersion(string season, string customerID)
        {
            return context.PartRevision
                  .OrderByDescending(x => x.EffectDate)
                  //.Include(x => x.PartMaterials)
                  .FirstOrDefault(x => x.CustomerID == customerID && x.Season == season);
        }

        public IQueryable<PartRevision> GetPartRevisions(List<string> partNumbers)
        {
            return context.PartRevision.Where(x => partNumbers.Contains(x.PartNumber));
        }
    }
}
