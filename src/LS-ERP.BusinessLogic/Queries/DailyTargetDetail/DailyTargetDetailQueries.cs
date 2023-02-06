using AutoMapper;
using Microsoft.Extensions.Configuration;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Extensions;
using Ultils.Helpers;
using LS_ERP.XAF.Module.Dtos;

namespace LS_ERP.BusinessLogic.Queries
{
    public class DailyTargetDetailQueries : IDailyTargetDetailQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public DailyTargetDetailQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<DailyTargetDetailDtos> GetAll()
        {
            var result = context.DailyTargetDetail;
            return result.Select(x => mapper.Map<DailyTargetDetailDtos>(x));
        }
        public IEnumerable<DailyTargetDetailDtos> GetByDate(DateTime date, List<string> departmentIds)
        {
            var listWorkCenterID = context.WorkCenter.Where(x => departmentIds.Contains(x.DepartmentID)).Select(x => x.ID).ToList();
            var result = context.DailyTargetDetail.Where(x => x.ProduceDate.Day == date.Day
                                                        && x.ProduceDate.Month == date.Month
                                                        && x.ProduceDate.Year == date.Year                                                        
                                                        && listWorkCenterID.Contains(x.WorkCenterID)
                                                       );
            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
        public IEnumerable<DailyTargetDetailDtos> GetByDate(DateTime date)
        {
            var result = context.DailyTargetDetail.Where(x => x.ProduceDate.Day == date.Day
                                                        && x.ProduceDate.Month == date.Month
                                                        && x.ProduceDate.Year == date.Year
                                                       );
               
            var res =  result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach(var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
        public IEnumerable<MonthDailyTargetDetailDtos> GetByMonth(string companyID,DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var currentDay = firstDayOfMonth;
            var response = new List<MonthDailyTargetDetailDtos>();
            var monthDailyTargetDetailDtos = new MonthDailyTargetDetailDtos();
            while(currentDay <= lastDayOfMonth)
            {                
                var res = context.DailyTargetDetail.Where(x => x.ProduceDate.Day == currentDay.Day
                                                        && x.ProduceDate.Month == currentDay.Month
                                                        && x.ProduceDate.Year == currentDay.Year
                                                        && x.WorkCenterID.Contains(companyID)
                                                       );                
                var totalDailyTargetDetails = res.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
                foreach(var item in totalDailyTargetDetails)
                {
                    var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                    var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                    item.PlantCode = planCode;
                    item.SortIndex = sortIndex;
                }
                totalDailyTargetDetails = totalDailyTargetDetails.OrderBy(o=>o.PlantCode).ThenBy(o=>o.SortIndex).ToList();
                var dateDailyTargetDetails = new DateDailyTargetDetailDtos();
                dateDailyTargetDetails.DateDailyTargetDetail = totalDailyTargetDetails;
                monthDailyTargetDetailDtos.TotalDateDailyTargetDetail.Add(dateDailyTargetDetails);
                currentDay = currentDay.AddDays(1);
            }
            response.Add(monthDailyTargetDetailDtos);
            return response;
        }
        public IEnumerable<DailyTargetDetailDtos> GetToDate(string companyID, DateTime fromDate,DateTime toDate)
        {
            
            var result = context.DailyTargetDetail.Where(x => x.ProduceDate.Date >= fromDate.Date
                                                               && x.ProduceDate.Date <= toDate.Date
                                                               && x.WorkCenterID.Contains(companyID)
                                                        //&& x.ProduceDate.Day <= date.Day
                                                       // && x.ProduceDate.Month <= date.Month
                                                       // && x.ProduceDate.Year <= date.Year
                                                       ).OrderBy(x=>x.ProduceDate);
            return result.Select(x => mapper.Map<DailyTargetDetailDtos>(x));
        }

        public IEnumerable<DailyTargetDetailDtos> GetById(int id)
        {
            var result = context.DailyTargetDetail.Where(x => x.ID == id);            
            return result.Select(x => mapper.Map<DailyTargetDetailDtos>(x));
        }

        public IEnumerable<DailyTargetDetailDtos> GetByWorkCenterId(string departmentId, DateTime produceDate)
        {
            var result = context.DailyTargetDetail
                                            .Where(x => x.WorkCenterID == departmentId
                                                && x.ProduceDate.Day == produceDate.Day
                                                && x.ProduceDate.Month == produceDate.Month
                                                && x.ProduceDate.Year == produceDate.Year
                                                    );
            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
        public IEnumerable<DailyTargetDetailDtos> GetByOperation(string companyId, DateTime produceDate, string operation)
        {
            var result = context.DailyTargetDetail
                                            .Where(x => x.ProduceDate.Day == produceDate.Day
                                                && x.ProduceDate.Month == produceDate.Month
                                                && x.ProduceDate.Year == produceDate.Year
                                                && x.WorkCenterID.Contains(companyId)
                                                && x.Operation == operation
                                                    );

            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
        public IEnumerable<DailyTargetDetailDtos> GetByOperation(string companyId, DateTime fromDate, DateTime toDate, string operation)
        {
            var result = context.DailyTargetDetail
                                            .Where(x => x.ProduceDate.Date >= fromDate.Date
                                                && x.ProduceDate.Date <= toDate.Date
                                                && x.WorkCenterID.Contains(companyId)
                                                && x.Operation == operation
                                                    );
            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
        public IEnumerable<AllocDailyOutputDtos> GetAllocBySize(string companyId, DateTime fromDate, DateTime toDate, string operation)
        {
            var result = (from a in context.AllocDailyOutput
                         from da in context.DailyTargetDetail
                         where da.LSStyle.Contains(a.LSStyle)
                         select new AllocDailyOutputDtos
                         {
                             LSStyle = a.LSStyle,
                             OrderQuantity = a.OrderQuantity,
                             Quantity = a.Quantity,
                             Size = a.Size,
                             IsFull = a.IsFull,
                             WorkCenterID = da.WorkCenterID,
                             WorkCenterName = da.WorkCenterName,
                             ProduceDate = da.ProduceDate,
                             Operation = da.Operation
                         })
                         .Where(x=>x.ProduceDate.Date>=fromDate.Date
                                && x.ProduceDate.Date<=toDate.Date
                                && x.Operation == operation
                                && x.WorkCenterID.Contains(companyId))
                         .ToList();
            return result; 
        }
        public IEnumerable<XAFDailyTargetDetailSummaryDtos> GetDailyTargetDetailSummary(string customerID, string style, DateTime fromDate, DateTime toDate)
        {

            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID",customerID ?? string.Empty),
                new SqlParameter("@Style",style ?? string.Empty),
                new SqlParameter("@FROMDATE", fromDate),
                new SqlParameter("@TODATE", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_DailyTargetDetailSummary", parameters);
            return table.AsListObject<XAFDailyTargetDetailSummaryDtos>();
        }

        public IEnumerable<DailyTargetDetailSummaryDtos> GetSummaryByMonth(string companyId, DateTime fromDate,DateTime toDate)
        {
           var result = context.DailyTargetDetail.Where(x => x.ProduceDate.Date >= fromDate.Date
                                                        && x.ProduceDate <= toDate.Date
                                                        && x.WorkCenterID.Contains(companyId)
                                                        && x.Quantity != null
                                                        && x.Efficiency != null)
                                                  .AsEnumerable()
                                                  .GroupBy(g => new
                                                  {
                                                      g.ProduceDate,
                                                      g.NumberOfWorker,
                                                      g.WorkCenterName
                                                  })
                                                  .Select(s => new DailyTargetDetailSummaryLineDtos
                                                  {
                                                      ProduceDate = s.Key.ProduceDate,
                                                      WorkCenterName = s.Key.WorkCenterName,
                                                      NumberOfWorker = s.Key.NumberOfWorker,
                                                      Quantity = (decimal)s.Sum(y => y.Quantity),
                                                      Efficiency = (decimal)s.Sum(y => y.Efficiency),
                                                      Lines = s.Select(t => t.ProduceDate).Distinct().Count()
                                                  })
                                                  .OrderBy(x=>x.ProduceDate)
                                                  .GroupBy(g => new
                                                  {
                                                      g.ProduceDate
                                                  })
                                                  .Select(s => new DailyTargetDetailSummaryDtos
                                                  {
                                                      ProduceDate = s.Key.ProduceDate.ToString("dd-MMM"),
                                                      NumberOfWorker = s.Sum(y => y.NumberOfWorker),
                                                      Quantity = s.Sum(y => y.Quantity),
                                                      AvgEfficiency = s.Average(y => y.Efficiency),
                                                      Lines = s.Sum(t => t.Lines)
                                                  })
                                                  ;
            return result;

        }
        public IEnumerable<LSStyleOrderQuantityDtos> GetOrderOutputQuantity(List<string> LSStyles)
        {
            var result = new List<LSStyleOrderQuantityDtos>();
            var itemStyles = context.ItemStyle.Where(x => LSStyles.Contains(x.LSStyle)).ToList();
            decimal totalOrder = 0;
            decimal totalOutput = 0;
            foreach(var item in itemStyles)
            {
                totalOrder += (decimal)item.TotalQuantity;
                var allocDailyOutputs = context.AllocDailyOutput.Where(x => x.LSStyle == item.LSStyle).ToList();
                foreach(var allocDailyOutput in allocDailyOutputs)
                {
                    totalOutput += allocDailyOutput.Quantity;
                }    
            }
            
            LSStyleOrderQuantityDtos lSStyleOrderQuantityDtos = new LSStyleOrderQuantityDtos();
            lSStyleOrderQuantityDtos.TotalOrderQuantity = totalOrder;
            lSStyleOrderQuantityDtos.TotalOutputQuantity = totalOutput;
            result.Add(lSStyleOrderQuantityDtos);

            return result;

        }

        public IEnumerable<DailyTargetDetailDtos> GetByOperationDate(string customerId, string operation, DateTime produceDate)
        {
            var result = context.DailyTargetDetail
                                           .Where(x => (x.CustomerID == customerId || string.IsNullOrEmpty(customerId))
                                               && (x.Operation == operation || string.IsNullOrEmpty(operation))
                                               && x.ProduceDate.Day == produceDate.Day
                                               && x.ProduceDate.Month == produceDate.Month
                                               && x.ProduceDate.Year == produceDate.Year
                                                   );
            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }

        public IEnumerable<DailyTargetDetailDtos> GetByOffset(string companyId, DateTime produceDate, string operation)
        {
            var result = context.DailyTargetDetail
                                           .Where(x => (x.WorkCenterID.Contains(companyId) || string.IsNullOrEmpty(companyId))
                                               && (x.Operation == operation || string.IsNullOrEmpty(operation))
                                               && x.ProduceDate == produceDate
                                               );
            var res = result.Select(x => mapper.Map<DailyTargetDetailDtos>(x)).ToList();
            foreach (var item in res)
            {
                var planCode = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.PlantCode).FirstOrDefault();
                var sortIndex = context.WorkCenter.Where(x => x.Name == item.WorkCenterName).Select(s => s.SortIndex).FirstOrDefault();
                item.PlantCode = planCode;
                item.SortIndex = sortIndex;
            }
            res = res.OrderBy(o => o.PlantCode).ThenBy(o => o.SortIndex).ToList();
            return res;
        }
    }
}
