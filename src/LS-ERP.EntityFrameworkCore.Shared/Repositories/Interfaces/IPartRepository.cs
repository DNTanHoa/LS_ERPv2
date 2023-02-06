using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPartRepository
    {
        Part Add(Part part);
        void Update(Part part);
        void Delete(Part part);
        IQueryable<Part> GetParts();
        IQueryable<Part> GetParts(string CustomerID);
        Part GetPart(string Number);
        bool IsExist(string Number);
        bool IsExist(string Number, out Part part);
    }
}
