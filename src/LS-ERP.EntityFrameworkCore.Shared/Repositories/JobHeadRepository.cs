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
    public class JobHeadRepository : IJobHeadRepository
    {
        private readonly AppDbContext context;

        public JobHeadRepository(AppDbContext context)
        {
            this.context = context;
        }

        public JobHead Add(JobHead jobHead)
        {
            return context.JobHead.Add(jobHead).Entity;
        }

        public void Delete(JobHead jobHead)
        {
            context.JobHead.Remove(jobHead);
        }

        public IQueryable<JobHead> GetJobHeads()
        {
            return context.JobHead;
        }

        public JobHead GetJobHead(string number)
        {
            return context.JobHead.FirstOrDefault(x => x.Number == number);
        }

        public bool IsExist(string number, out JobHead jobHead)
        {
            jobHead = null;
            jobHead = context.JobHead.FirstOrDefault(x => x.Number == number);
            return jobHead != null ? true : false;
        }

        public bool IsExist(string number)
        {
            var jobHead = context.JobHead.FirstOrDefault(x => x.Number == number);
            return jobHead != null ? true : false;
        }

        public void Update(JobHead jobHead)
        {
            context.Entry(jobHead).State = EntityState.Modified;
        }

        public IQueryable<JobHead> GetJobHeads(string style)
        {
            IQueryable<JobHead> dataSource = context.JobHead.Where(x => x.LSStyle.Contains(style));
            return dataSource;
        }

        public IQueryable<JobHead> SearchJobHeads(string keywords)
        {
            return context.JobHead.Where(x => x.LSStyle.Contains(keywords) ||
                                              x.GarmentSize.Contains(keywords));
        }

        public IQueryable<JobHead> GetJobHeads(string customerID, string style,
            DateTime fromDate, DateTime toDate)
        {
            var reservationJobHeads = context.ReservationEntry
                .Include(x => x.OrderDetail)
                .ThenInclude(x => x.ItemStyle)
                .ThenInclude(x => x.SalesOrder)
                .Where(x => !string.IsNullOrEmpty(x.JobHeadNumber) &&
                            (x.OrderDetail.ItemStyle.LSStyle.Contains(style) ||
                            string.IsNullOrEmpty(style)) &&
                            (x.OrderDetail.ItemStyle.SalesOrder.CustomerID == customerID ||
                            string.IsNullOrEmpty(customerID))).Select(x => x.JobHeadNumber);

            return context.JobHead.Where(x => reservationJobHeads.Contains(x.Number) &&
                                              x.CreatedAt.Value.Date >= fromDate &&
                                              x.CreatedAt.Value.Date <= toDate);
        }

        public IQueryable<JobHead> GetJobHeads(List<string> customerStyles)
        {
            IQueryable<JobHead> dataSource = context.JobHead.Where(x => customerStyles.Contains(x.CustomerStyle));
            return dataSource;
        }
    }
}
