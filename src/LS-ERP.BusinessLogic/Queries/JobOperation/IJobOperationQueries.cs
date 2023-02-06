using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IJobOperationQueries
    {
        IQueryable<JobOperation> GetJobOperations();
        IQueryable<JobOperation> GetJobOperations(string customerID);
        IQueryable<JobOperation> GetJobOperations(string customerID,string style);
        IQueryable<JobOperation> GetJobOperations(string customerID,
            DateTime fromDate, DateTime toDate);
        IQueryable<JobOperation> GetJobOperations(string customerID, string style,
            DateTime fromDate, DateTime toDate);
        Task<JobOperation> GetJobOperation(string ID);
    }
}
