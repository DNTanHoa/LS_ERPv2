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
    public class LabelPortRepository : ILabelPortRepository
    {
        private readonly AppDbContext context;

        public LabelPortRepository(AppDbContext context)
        {
            this.context = context;
        }

        public LabelPort Add(LabelPort labelPort)
        {
            return context.LabelPort.Add(labelPort).Entity;
        }

        public void Delete(LabelPort labelPort)
        {
            context.LabelPort.Remove(labelPort);
        }

        public LabelPort GetLabelPort(long ID)
        {
            return context.LabelPort.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<LabelPort> GetLabelPorts()
        {
            return context.LabelPort;
        }

        public IQueryable<LabelPort> GetLabelPorts(string customerID)
        {
            return context.LabelPort.Where(x => x.CustomerID == customerID);
        }

        public void Update(LabelPort labelPort)
        {
            context.Entry(labelPort).State = EntityState.Modified;
        }
    }
}
