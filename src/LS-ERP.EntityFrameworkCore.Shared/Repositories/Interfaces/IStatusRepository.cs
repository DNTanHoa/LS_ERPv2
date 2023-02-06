using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IStatusRepository
    {
        Status Add(Status status);
        void Update(Status status);
        void Delete(Status status);
        IQueryable<Status> GetStatus();
        Status GetStatus(string ID);
    }
}
