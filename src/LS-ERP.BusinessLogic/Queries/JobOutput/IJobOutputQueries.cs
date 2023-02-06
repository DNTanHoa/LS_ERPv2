using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LS_ERP.BusinessLogic.Queries
{
    public interface IJobOutputQueries
    {
        IQueryable<JobOutput> GetJobOutputs();
        IEnumerable<JobOutput> GetJobOutputs(int id);
        IQueryable<JobOutput> GetJobOutputs(string customerID, string workCenterID);
        IQueryable<JobOutput> GetJobOutputsByDepartment(string departmentID,DateTime atTime);
        IEnumerable<JobOutput> GetJobOutputsByDate(DateTime date);
        IEnumerable<JobOutputSummaryDtos> GetJobOutputsSummaryByDate(DateTime time);
        IQueryable<JobOutput> GetJobOutputs(string customerID, DateTime fromDate, DateTime toDate);
        IQueryable<JobOutput> GetJobOutputs(string customerID, string workCenterID ,DateTime fromDate, DateTime toDate);
        Task<JobOutput> GetOneAsync(int id);
    }
}
