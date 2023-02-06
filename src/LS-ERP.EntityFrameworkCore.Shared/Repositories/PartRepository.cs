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
    public class PartRepository
        : IPartRepository
    {
        private readonly AppDbContext context;

        public PartRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Part Add(Part part)
        {
            return context.Part.Add(part).Entity;
        }

        public void Delete(Part part)
        {
            context.Part.Remove(part);
        }

        public Part GetPart(string Number)
        {
            return context.Part.FirstOrDefault(x => x.Number == Number);
        }

        public IQueryable<Part> GetParts()
        {
            return context.Part;
        }

        public IQueryable<Part> GetParts(string CustomerID)
        {
            return context.Part.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(string Number)
        {
            var part = GetPart(Number);
            return part != null ? true : false;
        }

        public bool IsExist(string Number, out Part part)
        {
            part = null;
            part = GetPart(Number);
            return part != null ? true : false;
        }

        public void Update(Part part)
        {
            context.Entry(part).State = EntityState.Modified;
        }
    }
}
