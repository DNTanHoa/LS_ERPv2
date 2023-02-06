using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ISizeRepository
    {
        Size Add(Size Size);
        void Update(Size Size);
        void Delete(Size Size);
        IQueryable<Size> GetSizes();
        IQueryable<Size> GetSizes(string CustomerID);
        Size GetSize(long ID);
        bool IsExist(long ID, out Size Size);
        bool IsExist(long ID);
    }
}
