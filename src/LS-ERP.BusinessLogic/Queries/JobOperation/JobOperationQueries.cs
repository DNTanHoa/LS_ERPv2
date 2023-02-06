using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class JobOperationQueries : IJobOperationQueries
    {
        private readonly SqlServerAppDbContext context;

        public JobOperationQueries(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public Task<JobOperation> GetJobOperation(string ID)
        {
            var data = context.JobOperation.FirstOrDefaultAsync(x => x.ID == ID);
            return data;
        }

        public IQueryable<JobOperation> GetJobOperations()
        {
            return context.JobOperation;
        }

        public IQueryable<JobOperation> GetJobOperations(string customerID)
        {
            return context.JobOperation.Where(x => x.CustomerID == customerID);
        }

        public IQueryable<JobOperation> GetJobOperations(string customerID, string style)
        {
            return context.JobOperation
                .Include(x => x.JobHead)
                .Where(x => x.CustomerID == customerID &&
                            x.JobHead.LSStyle.Contains(style) || string.IsNullOrEmpty(style));
        }

        public IQueryable<JobOperation> GetJobOperations(string customerID, 
            DateTime fromDate, DateTime toDate)
        {
            return context.JobOperation
                .Include(x => x.JobHead)
                .Where(x => x.CustomerID == customerID &&
                            x.StartDate >= fromDate &&
                            x.StartDate <= toDate);
        }

        public IQueryable<JobOperation> GetJobOperations(string customerID, string style, 
            DateTime fromDate, DateTime toDate)
        {
            return context.JobOperation
                .Include(x => x.JobHead)
                .Where(x => x.CustomerID == customerID &&
                            x.JobHead.LSStyle.Contains(style) || string.IsNullOrEmpty(style) &&
                            x.StartDate >= fromDate &&
                            x.StartDate <= toDate);
        }
    }
}
