using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPartRevisionRepository
    {
        PartRevision Add(PartRevision partRevision);
        void Update(PartRevision partRevision);
        void Delete(PartRevision partRevision);
        IQueryable<PartRevision> GetPartRevisions();
        IQueryable<PartRevision> GetPartRevisions(List<string> partNumbers);
        PartRevision GetPartRevision(long ID);
        bool IsExist(long ID, out PartRevision partRevision);
        bool IsExist(string PartNumber, string RevisionNumber, string Season);
        bool IsExist(long ID);
        PartRevision GetLastestVersion(string partNumber);
        PartRevision GetLastestVersion(string season, string customerID);
        IQueryable<PartRevision> GetLastestPartRevisions(List<string> partNumbers);
    }
}
