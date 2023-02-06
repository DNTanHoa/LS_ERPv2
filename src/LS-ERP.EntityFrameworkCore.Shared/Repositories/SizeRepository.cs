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
    public class SizeRepository : ISizeRepository
    {
        private readonly AppDbContext context;

        public SizeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Size Add(Size Size)
        {
            return context.Size.Add(Size).Entity;
        }

        public void Delete(Size Size)
        {
            context.Size.Remove(Size);
        }

        public Size GetSize(long ID)
        {
            return context.Size.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<Size> GetSizes()
        {
            return context.Size;
        }

        public IQueryable<Size> GetSizes(string CustomerID)
        {
            return context.Size.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(long ID, out Size Size)
        {
            Size = null;
            Size = GetSize(ID);
            return Size != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var Size = GetSize(ID);
            return Size != null ? true : false;
        }

        public void Update(Size Size)
        {
            context.Entry(Size).State = EntityState.Modified;
        }
    }
}
