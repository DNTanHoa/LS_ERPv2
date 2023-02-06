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
    public class PadRepository : IPadRepository
    {
        private readonly AppDbContext context;

        public PadRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Pad Add(Pad pad)
        {
            return context.Add(pad).Entity;
        }

        public void Delete(Pad pad)
        {
            context.Pad.Remove(pad);
        }

        public Pad GetPad(string Code)
        {
            return context.Pad.FirstOrDefault(x => x.Code == Code);
        }

        public IQueryable<Pad> GetPads()
        {
            return context.Pad;
        }

        public void Update(Pad pads)
        {
            context.Entry(pads).State = EntityState.Modified;
        }
    }
}
