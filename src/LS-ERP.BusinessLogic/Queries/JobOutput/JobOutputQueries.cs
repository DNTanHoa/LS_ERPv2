using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class JobOutputQueries : IJobOutputQueries
    {
        private readonly SqlServerAppDbContext context;

        public JobOutputQueries(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<JobOutput> GetJobOutputs()
        {
            return context.JobOuput;
        }
        public IEnumerable<JobOutput> GetJobOutputs(int id)
        {
            return context.JobOuput.Where(x => x.ID == id);
        }
        public async Task<JobOutput> GetOneAsync(int id)
        {
            return (JobOutput)context.JobOuput.Where(x => x.ID == id);
        }
        public IQueryable<JobOutput> GetJobOutputs(string customerID, string workCenterID)
        {
            return context.JobOuput.Where(x => x.CustomerID == customerID &&
                                               x.WorkCenterID == workCenterID);
        }

        public IQueryable<JobOutput> GetJobOutputsByDepartment(string departmentID, DateTime atTime)
        {
            return context.JobOuput.Where(x => x.WorkCenter.DepartmentID == departmentID
                                            && x.OutputAt.Value.Year.ToString() == atTime.Year.ToString()
                                            && x.OutputAt.Value.Month.ToString() == atTime.Month.ToString()
                                            && x.OutputAt.Value.Day.ToString() == atTime.Day.ToString()
                                            ).OrderBy(o=>o.WorkingTimeID);
        }
        public IEnumerable<JobOutput> GetJobOutputsByDate(DateTime date)
        {
            return context.JobOuput.Where(x=> x.OutputAt.Value.Year.ToString() == date.Year.ToString()
                                            && x.OutputAt.Value.Month.ToString() == date.Month.ToString()
                                            && x.OutputAt.Value.Day.ToString() == date.Day.ToString()
                                            );
            
        }
        public IEnumerable<JobOutputSummaryDtos> GetJobOutputsSummaryByDate(DateTime date)
        {
            return context.JobOuput.Where(x => x.OutputAt.Value.Year.ToString() == date.Year.ToString()
                                            && x.OutputAt.Value.Month.ToString() == date.Month.ToString()
                                            && x.OutputAt.Value.Day.ToString() == date.Day.ToString()
                                            )
                                            .Where(x=>x.TargetQuantity!=null && x.Efficiency != null)
                                            .AsEnumerable()
                                            .GroupBy(x => new
                                            {
                                                x.CustomerID,
                                                x.CustomerName,
                                                x.WorkCenterID,
                                                x.WorkCenterName,
                                                x.DepartmentID,
                                                x.StyleNO,
                                                x.ItemStyleDescription
                                            })
                                            .Select(y => new JobOutputSummaryDtos()
                                            {
                                                CustomerID = y.Key.CustomerID,
                                                CustomerName = y.Key.CustomerName,
                                                WorkCenterID = y.Key.WorkCenterID,
                                                WorkCenterName = y.Key.WorkCenterName,
                                                DepartmentID = y.Key.DepartmentID,
                                                StyleNO = y.Key.StyleNO,
                                                ItemStyleDescription = y.Key.ItemStyleDescription,

                                                TargetQuantity = y.Sum(t => t.TargetQuantity),
                                                Quantity = y.Sum(t => t.Quantity),
                                                Efficiency = y.Average(t=>t.Efficiency),
                                                EfficiencyStr = decimal.Round((decimal)y.Average(t=>t.Efficiency),2).ToString("F")+"%",
                                                //Efficiency = y.Sum(t => t.Quantity) / y.Sum(t => t.TargetQuantity),
                                                //EfficiencyStr = decimal.Round((decimal)(100 * y.Sum(t => t.Quantity) / y.Sum(t => t.TargetQuantity)), 2)
                                                //.ToString("F") + "%",
                                                Problem = string.Join(",", y.Select(x=>x.Problem).Distinct())
                                            });
            
        }

        public IQueryable<JobOutput> GetJobOutputs(string customerID, 
            DateTime fromDate, DateTime toDate)
        {
            return context.JobOuput.Where(x => x.CustomerID == customerID &&
                                               x.OutputAt >= fromDate &&
                                               x.OutputAt <= toDate);
        }

        public IQueryable<JobOutput> GetJobOutputs(string customerID, string workCenterID, 
            DateTime fromDate, DateTime toDate)
        {
            return context.JobOuput.Where(x => x.CustomerID == customerID &&
                                               x.WorkCenterID == workCenterID &&
                                               x.OutputAt >= fromDate &&
                                               x.OutputAt <= toDate);
        }
    }
}
