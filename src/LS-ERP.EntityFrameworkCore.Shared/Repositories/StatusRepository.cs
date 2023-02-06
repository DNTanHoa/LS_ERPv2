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
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDbContext context;

        public StatusRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Status Add(Status status)
        {
            return context.Add(status).Entity;
        }

        public void Delete(Status status)
        {
            context.Status.Remove(status);
        }

        public IQueryable<Status> GetStatus()
        {
            return context.Status;
        }

        public Status GetStatus(string ID)
        {
            return context.Status.FirstOrDefault(x => x.ID == ID);
        }

        public void Update(Status status)
        {
            context.Entry(status).State = EntityState.Modified;
        }
    }
}
