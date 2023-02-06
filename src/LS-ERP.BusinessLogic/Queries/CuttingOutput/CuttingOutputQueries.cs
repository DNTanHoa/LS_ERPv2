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
    public class CuttingOutputQueries : ICuttingOutputQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public CuttingOutputQueries(SqlServerAppDbContext context,IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<CuttingOutputDtos> Get(string companyID,DateTime fromDate, DateTime toDate)
        {
          
            var result =  context.CuttingOutput.Where(x=>x.ProduceDate.Date >= fromDate.Date
                                                    && x.ProduceDate.Date <= toDate.Date
                                                    && x.WorkCenterID.Contains(companyID)).OrderByDescending(x=>x.CreatedAt).ToList();
            return result.Select(x=>mapper.Map<CuttingOutputDtos>(x));
        }
        public IEnumerable<CuttingOutputDtos> Get(int ID)
        {

            var result = context.CuttingOutput.Where(x => x.ID == ID).ToList();
            return result.Select(x => mapper.Map<CuttingOutputDtos>(x));
        }
        public IEnumerable<CuttingOutputAllocDetailDtos> GetAllocDetail(int ID)
        {
            var result = (from atr in context.AllocTransaction.Where(atr => atr.CuttingOutputID == ID
                                                                && atr.Lot == null
                                                                && atr.Operation == "CUTTING")
                       from a in context.AllocDailyOutput.Where(a => a.LSStyle == atr.LSStyle
                                                               && a.Operation == "CUTTING"
                                                               && a.Size == atr.Size
                                                               && a.Set == atr.Set
                                                               && a.FabricContrastName == atr.FabricContrastName)
                       from d in context.DailyTarget.Where(d => d.LSStyle == a.LSStyle && d.Operation == "CUTTING")
                       from c in context.CuttingOutput.Where(c => c.ID == ID)
                       from f in context.FabricContrast.Where(f => f.ID == a.FabricContrastID)
                       select new CuttingOutputAllocDetailDtos
                       {
                           PSDD = a.PSDD,
                           MergeBlockLSStyle = d.MergeBlockLSStyle,
                           MergeLSStyle = d.MergeLSStyle,
                           PriorityLSStyle = d.LSStyle,
                           Size = atr.Size,
                           Set = a.Set,
                           Lot = c.Lot,
                           FabricContrastName = a.FabricContrastName,
                           FabricContrastDescription = f.Description,
                           OrderQuantity = a.OrderQuantity + a.PercentQuantity+a.Sample,
                           AllocQuantity = atr.AllocQuantity,
                           Quantity = a.Quantity,
                           Balance = a.Quantity - (a.OrderQuantity + a.PercentQuantity + a.Sample),
                           CuttingQuantity = c.Quantity,
                           CuttingOutput = c

                       }).AsEnumerable()                       
                       .GroupBy(g=> new
                       {
                           g.PSDD,g.MergeBlockLSStyle,g.MergeLSStyle,g.PriorityLSStyle,g.Size,g.Set,g.Lot,g.FabricContrastName,g.FabricContrastDescription
                           ,g.OrderQuantity,g.Quantity,g.Balance,g.CuttingQuantity,g.CuttingOutput
                       })
                       .Select(s=>new CuttingOutputAllocDetailDtos
                       {
                           PSDD = s.Key.PSDD,
                           MergeBlockLSStyle = s.Key.MergeBlockLSStyle,
                           MergeLSStyle = s.Key.MergeLSStyle,
                           PriorityLSStyle = s.Key.PriorityLSStyle,
                           Size = s.Key.Size,
                           Set = s.Key.Set,
                           Lot = s.Key.Lot,
                           FabricContrastName = s.Key.FabricContrastName,
                           FabricContrastDescription = s.Key.FabricContrastDescription,
                           OrderQuantity = s.Key.OrderQuantity,
                           AllocQuantity = s.Sum(t=>t.AllocQuantity),
                           Quantity = s.Key.Quantity,
                           Balance = s.Key.Balance,
                           CuttingQuantity = s.Key.CuttingQuantity,
                           CuttingOutput = s.Key.CuttingOutput,
                       }
                       ).OrderBy(a => a.PSDD).ToList();
            
            return result;
        }
        public IEnumerable<CuttingOutputDailyReportDtos> GetDailyReport(string companyID, DateTime produceDate)
        {
            //var result =  (from c in context.CuttingOutput.Where(c => c.WorkCenterID.Contains(companyID)
            //                                                     && c.ProduceDate.Day == produceDate.Day
            //                                                     && c.ProduceDate.Month == produceDate.Month
            //                                                     && c.ProduceDate.Year == produceDate.Year
            //                                                     && c.IsPrint == false
            //                                                    )
            //               from d in context.DailyTarget.Where(d => d.MergeBlockLSStyle == c.MergeBlockLSStyle
            //                                               && d.Operation == "CUTTING"
            //                                               && d.CompanyID == companyID
            //                                               && !string.IsNullOrEmpty(d.CustomerName)
            //                                                    ).Distinct()
            //               from a in context.AllocTransaction.Where(a => a.CuttingOutputID == c.ID
            //                                                        && a.LSStyle == d.LSStyle
            //                                                         && a.IsRetured == false
            //                                                         && a.Lot == null
            //                                                         && a.FabricContrastName == "A"
            //                                                         && ((d.Set.Contains(";") && a.Set.Equals("ÁO"))
            //                                                            || (!d.Set.Contains(";") && d.Set == a.Set)
            //                                                         ))

            //              select new CuttingOutputDailyReportDtos
            //              {
            //                  LSStyle = a.LSStyle,         
            //                  Set = a.Set,
            //                  CustomerName = d.CustomerName,
            //                  TargetQuantity = d.TargetQuantity,
            //                  WorkCenterName = c.WorkCenterName,
            //                  GarmentColor = d.GarmentColor,
            //                  Quantity = a.AllocQuantity,
            //                  Remark = d.CustomerName + " " + d.Remark
            //              }).AsEnumerable()
            //              .GroupBy(g=> new {g.LSStyle,g.Set,g.CustomerName,g.WorkCenterName,g.GarmentColor,g.TargetQuantity,g.Remark})
            //              .Select(s=>new CuttingOutputDailyReportDtos
            //              {
            //                  LSStyle = s.Key.LSStyle, 
            //                  Set = s.Key.Set,
            //                  CustomerName = s.Key.CustomerName,
            //                  TargetQuantity = s.Key.TargetQuantity,
            //                  WorkCenterName= s.Key.WorkCenterName,
            //                  GarmentColor= s.Key.GarmentColor,
            //                  Remark =s.Key.Remark,
            //                  Quantity = s.Sum(t=>t.Quantity)
            //              })
            //              .OrderBy(c=>c.WorkCenterName)
            //              .ThenBy(c=>c.LSStyle)                          
            //              .ToList();
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),
                new SqlParameter("@ProduceDate", produceDate),

            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectCuttingDailyReport", parameters);
            var result = table.AsListObject<CuttingOutputDailyReportDtos>();
            foreach (var item in result) // Set status
            {
                //item.setStatus();
                // edit 09/08/2022
                bool flag = true;
                var ls = context.AllocDailyOutput.Where(x => x.LSStyle == item.LSStyle && x.FabricContrastName == "A");
                foreach (var l in ls)
                {
                    if (!l.IsFull)
                    {
                        flag = false; break;
                    }
                }
                if (flag)
                {
                    item.setStatus("OK");
                }
                else
                {
                    item.setStatus("NO");
                }    
            }
            // Không tính sản lượng hàng % in vào báo cáo ngày để tính lương // commnet date 2022/11/15
            //var Print = context.CuttingOutput.Where(c => c.WorkCenterID.Contains(companyID)
            //                                                     && c.ProduceDate.Day == produceDate.Day
            //                                                     && c.ProduceDate.Month == produceDate.Month
            //                                                     && c.ProduceDate.Year == produceDate.Year
            //                                                     && c.FabricContrast == "A"
            //                                                     && c.IsPrint == true).Select(x=>new CuttingOutputDailyReportDtos
            //                                                     {
            //                                                         LSStyle = x.MergeBlockLSStyle,
            //                                                         Set = x.Set,
            //                                                         CustomerName = x.CustomerName,
            //                                                         GarmentColor = context.DailyTarget.Where(d=>d.MergeBlockLSStyle==x.MergeBlockLSStyle).FirstOrDefault().GarmentColor,
            //                                                         WorkCenterName = x.WorkCenterName,
            //                                                         Quantity = x.Quantity,
            //                                                         Status = "% In",
            //                                                         Remark = x.Remark,
            //                                                         IsAllSize = x.IsAllSize,
            //                                                         MergeLSStyle = x.MergeLSStyle
                                                                     
            //                                                     }).ToList();
            //foreach(var item in Print)
            //{
            //    if(item.IsAllSize)
            //    {
            //        var listSize = (from d in context.DailyTarget.Where(x => x.Operation == "CUTTING"
            //                                                 && item.LSStyle.Contains(x.MergeBlockLSStyle)
            //                                                 && (x.MergeLSStyle == item.MergeLSStyle  || string.IsNullOrEmpty(item.MergeLSStyle)))
            //                        from o in context.AllocDailyOutput.Where(a => a.LSStyle == d.LSStyle && a.TargetID == d.ID)
            //                        select o.Size).Distinct().ToList();
            //        item.Quantity = item.Quantity * listSize.Count();
            //    }    
            //    result.Add(item);
            //}    
            return result;
        }
        public IEnumerable<CuttingOutputDtos> GetMonthReport(string companyID, DateTime produceDate)
        {
            var result = context.CuttingOutput.Where(x => x.ProduceDate.Month == produceDate.Month
                                                    && x.ProduceDate.Year == produceDate.Year
                                                    && x.WorkCenterID.Contains(companyID)
                                                    && x.FabricContrast== "A"
                                                    && x.IsPrint == false)
                                                    .Select(x => mapper.Map<CuttingOutputDtos>(x)).ToList();
            // Không tính %IN , comment ngày 15/11/2022
            //var Prints = result.Where(x => x.IsPrint == true).ToList();
            //foreach(var item in Prints)
            //{
            //    if (item.IsAllSize)
            //    {
            //        result.Remove(item);
            //    }               
            //}
            //foreach (var item in Prints)
            //{
            //    if (item.IsAllSize)
            //    {
            //        var listSize = (from d in context.DailyTarget.Where(x => x.Operation == "CUTTING"
            //                                                 && item.MergeBlockLSStyle.Equals(x.MergeBlockLSStyle))
            //                        from o in context.AllocDailyOutput.Where(a => a.LSStyle == d.LSStyle && a.TargetID == d.ID)
            //                        select o.Size).Distinct().ToList();
            //        item.Quantity = item.Quantity * listSize.Count();
            //        result.Add(item);
            //    }                
            //}
            return result;
        }
        public IEnumerable<CuttingOutputDailyReportDtos> GetWorkCenterReportByMonth(string companyID, DateTime produceDate)
        {

            var result = (from c in context.CuttingOutput.Where(c => c.WorkCenterID.Contains(companyID)
                                                                 && c.ProduceDate.Month == produceDate.Month
                                                                 && c.ProduceDate.Year == produceDate.Year
                                                                 && c.FabricContrast == "A"
                                                                 && c.IsPrint == false
                                                                )                         
                          select new CuttingOutputDailyReportDtos
                          {
                              MergeBlockLSStyle = c.MergeBlockLSStyle,                             
                              WorkCenterName = c.WorkCenterName,
                              ProduceDate = c.ProduceDate,
                              ProduceDateString = c.ProduceDate.ToString("dd/MM/yyyy"),
                              Quantity = c.Quantity,
                              IsPrint = c.IsPrint,
                              IsAllSize = c.IsAllSize

                          }).AsEnumerable()
                          .GroupBy(g => new
                          {
                              g.MergeBlockLSStyle,                            
                              g.WorkCenterName,
                              g.ProduceDate,
                              g.ProduceDateString,
                              g.IsPrint,
                              g.IsAllSize
                          })
                          .Select(s => new CuttingOutputDailyReportDtos
                          {
                              MergeBlockLSStyle = s.Key.MergeBlockLSStyle,                             
                              WorkCenterName = s.Key.WorkCenterName,
                              ProduceDate = s.Key.ProduceDate,
                              ProduceDateString = s.Key.ProduceDateString,
                              Quantity = s.Sum(t => t.Quantity),
                              IsPrint = s.Key.IsPrint,
                              IsAllSize = s.Key.IsAllSize
                          })
                          .OrderBy(o => o.ProduceDate)
                          .ThenBy(c => c.WorkCenterName)
                          .ThenBy(c => c.LSStyle)
                          .ToList();
            // Không tính sản lượng %IN comment ngày 15/11/2022
            //var Prints = result.Where(x => x.IsPrint == true).ToList();
            //foreach (var item in Prints)
            //{
            //    if (item.IsAllSize)
            //    {
            //        result.Remove(item);
            //    }
            //}
            //foreach (var item in Prints)
            //{
            //    if (item.IsAllSize)
            //    {
            //        var listSize = (from d in context.DailyTarget.Where(x => x.Operation == "CUTTING"
            //                                                 && item.MergeBlockLSStyle.Equals(x.MergeBlockLSStyle))
            //                        from o in context.AllocDailyOutput.Where(a => a.LSStyle == d.LSStyle && a.TargetID == d.ID)
            //                        select o.Size).Distinct().ToList();
            //        item.Quantity = item.Quantity * listSize.Count();
            //        result.Add(item);
            //    }
            //}
            return result;
        }
        public IEnumerable<PivotCuttingLotDtos> GetPivotCuttingLot(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle, string mergeLSStyle)
        {
            //var dailyTargets = context.DailyTarget.Where(x => x.Operation == "CUTTING"
            //                                                  && x.ToDate.Date <= toDate.Date
            //                                                  && x.FromDate.Date >= fromDate.Date
            //                                                  && x.CompanyID.Equals(companyID)
            //                                                  && x.MergeBlockLSStyle == mergeBlockLSStyle
            //                                                  && (x.MergeLSStyle.Contains(mergeLSStyle) || string.IsNullOrEmpty(mergeLSStyle))
            //                                                  ).ToList();
            //if (dailyTargets.Count == 0)
            //    return null;
            //var set = dailyTargets.FirstOrDefault().Set.Split(";").ToList();
            //var result = (from a in context.AllocDailyOutput.Where(x => x.Operation == "CUTTING"
            //                                                     && x.FabricContrastName == "A"
            //                                                     && x.Quantity > 0
            //                                                     && dailyTargets.Select(d=>d.ID).ToList().Contains(x.TargetID)
            //                                                     && dailyTargets.Select(d=>d.LSStyle).ToList().Contains(x.LSStyle)
            //                                                     && set[0].Equals(x.Set)

            //                                                     )
            //              from c in context.CuttingLot.Where(x => x.LSStyle == a.LSStyle
            //                                                  && x.Size == a.Size
            //                                                  && x.Set == a.Set
            //                                                  && x.AllocDailyOutputID == a.ID
            //                                                  )

            //              select new PivotCuttingLotDtos
            //              {
            //                  LSStyle = a.LSStyle,                              
            //                  Size = a.Size,
            //                  Set = a.Set,
            //                  TotalLSStyleQuantity = context.AllocDailyOutput.Where(x=>x.LSStyle == a.LSStyle
            //                                                                         && x.FabricContrastName == "A"
            //                                                                         && set[0].Equals(x.Set)).Select(x=>x.OrderQuantity).Sum(),
            //                  OrderQuantity = a.OrderQuantity + a.PercentQuantity + a.Sample,
            //                  Lot = c.Lot,
            //                  Quantity = c.Quantity,
            //                  IsFull = a.IsFull

            //              }).ToList();
            //foreach(var item in result)
            //{
            //    item.SetStatus();
            //    item.MergeLSStyle = dailyTargets.Where(d => d.LSStyle == item.LSStyle).FirstOrDefault()?.MergeLSStyle ?? "";
            //    item.GarmentColor = dailyTargets.Where(d => d.LSStyle == item.LSStyle).FirstOrDefault()?.GarmentColor ?? "";
            //}    
            //return result;
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),
                new SqlParameter("@FROMDATE", fromDate),
                new SqlParameter("@TODATE", toDate),
                new SqlParameter("@MergeBlockLSStyle",mergeBlockLSStyle ?? string.Empty),
                new SqlParameter("@MergeLSStyle",mergeLSStyle ?? string.Empty),
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectCuttingLotReport", parameters);
            var result = table.AsListObject<PivotCuttingLotDtos>();
            foreach (var item in result)
            {
                item.SetStatus();               
            }
            return result;
        }

        public IEnumerable<string> GetIssueCuttingLot(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle, int fabricContrastID)
        {
            var result = (from f in context.FabricRequest.Where(f => f.CustomerStyleNumber.Contains(mergeBlockLSStyle)
                                                                && f.CompanyCode == companyID)
                         from i in context.Issued.Where(i => i.FabricRequestID == f.ID)
                         from il in context.IssuedGroupLine.Where(il => il.IssuedNumber == i.Number)
                         from fc in context.FabricContrast.Where(fc=>fc.ID == fabricContrastID)
                         from fd in context.FabricRequestDetail.Where(fd => fd.ID == il.FabricRequestDetailID
                                                                        && fd.FabricColor.Substring(3, 5).Contains(fc.Name))
                         select il.DyeLotNumber).ToList();
            return result;
        }
        public IEnumerable<string> GetCuttingSize(string companyID, DateTime fromDate, DateTime toDate
                                                , string mergeBlockLSStyle,string mergeLSStyle,string lsStyle)
            {
             var result = (from d in context.DailyTarget.Where(x => x.Operation == "CUTTING"
                                                             //&& x.ToDate.Date <= toDate.Date
                                                              //&& x.FromDate.Date >= fromDate.Date
                                                              && x.CompanyID.Equals(companyID)                                                              
                                                              && ( x.LSStyle == lsStyle || string.IsNullOrEmpty(lsStyle))
                                                              && (x.MergeLSStyle == mergeLSStyle || string.IsNullOrEmpty(mergeLSStyle))
                                                              && x.MergeBlockLSStyle == mergeBlockLSStyle)
                          from o in context.AllocDailyOutput.Where(a => a.LSStyle == d.LSStyle && a.TargetID == d.ID)
                          select o.Size.Trim().ToUpper()).Distinct().ToList();
            return result;
        }
        public IEnumerable<string> GetCuttingSet(string companyID, DateTime fromDate, DateTime toDate
                                                , string mergeBlockLSStyle, string lsStyle)
        {
            var result = new List<string>();
            var set = context.DailyTarget.Where(x => x.Operation == "CUTTING"
                                                            //&& x.ToDate.Date <= toDate.Date
                                                            // && x.FromDate.Date >= fromDate.Date
                                                             && x.CompanyID.Equals(companyID)
                                                             && (x.LSStyle == lsStyle || string.IsNullOrEmpty(lsStyle))
                                                             && x.MergeBlockLSStyle == mergeBlockLSStyle)
                                        .Select(d => d.Set).FirstOrDefault();      
            if(!string.IsNullOrEmpty(set))
            {
                result.Add(set);
                if(set.Contains(";"))
                {
                    var sets = set.Split(";").ToList();
                    foreach (var s in sets)
                    {
                        result.Add(s);
                    }
                } 
            }    
            return result;
        }
        public IEnumerable<FabricContrastDtos> GetCuttingFabricContrast(string mergeBlockLSStyle)
        {
            return context.FabricContrast.Where(f => f.MergeBlockLSStyle == mergeBlockLSStyle).Select(x=>mapper.Map<FabricContrastDtos>(x));
        }
        public IEnumerable<CuttingOutputStatusDtos> GetCuttingOutputStatus(string companyID, DateTime fromDate, DateTime toDate,string customerName,string searchLSStyle)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),                
                new SqlParameter("@FROMDATE", fromDate),
                new SqlParameter("@TODATE", toDate),
                new SqlParameter("@CustomerName",customerName ?? string.Empty),
                new SqlParameter("@LSStyle",searchLSStyle ?? string.Empty),
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectCuttingStatus", parameters);
            var result = table.AsListObject<CuttingOutputStatusDtos>();
            var lsStyles = result.Select(c => c.LSStyle).Distinct().ToList();
            foreach(var lsStyle in lsStyles)
            {
                
                var ls = result.Where(x => x.LSStyle == lsStyle && x.FabricContrastName == "A");
                foreach (var l in ls)
                {
                    if (l.IsFull)
                    {
                        var ls1 = result.Where(x => x.LSStyle == l.LSStyle && x.Size == l.Size );
                        foreach(var l1 in ls1)
                        {
                            l1.Status = "OK";
                        }  
                    }                    
                }
                ///
                ///
                var orderTargetQuanity = context.AllocDailyOutput.Where(a => a.Operation == "CUTTING"
                                                                        && a.FabricContrastName == "A"
                                                                        && a.LSStyle == lsStyle
                                                                        ).Select(a => a.OrderQuantity + a.PercentQuantity + a.Sample).Sum();
                var totalAllocQuantity = context.AllocDailyOutput.Where(a => a.Operation == "CUTTING"
                                                                        && a.FabricContrastName == "A"
                                                                        && a.LSStyle == lsStyle
                                                                        ).Select(a => a.Quantity).Sum();
                var Set = context.AllocDailyOutput.Where(a => a.Operation == "CUTTING"
                                                                        && a.FabricContrastName == "A"
                                                                        && a.LSStyle == lsStyle
                                                                        ).Select(a => a.Set).Distinct().ToList();
                if(Set.Count() > 0)
                {
                    orderTargetQuanity = orderTargetQuanity / Set.Count();
                }    
                var lsStyleSetTargets = result.Where(x => x.LSStyle == lsStyle).ToList();
                foreach(var lsStyleSetTarget in lsStyleSetTargets)
                {
                    lsStyleSetTarget.TargetQuantity = orderTargetQuanity;
                    lsStyleSetTarget.PSDDString = lsStyleSetTarget.PSDD.ToString("MM/dd/yyyy");
                    lsStyleSetTarget.TotalAllocQuantity = totalAllocQuantity;
                    lsStyleSetTarget.TotalBalanceQuantity = totalAllocQuantity - orderTargetQuanity;
                    if(lsStyleSetTarget.IsCanceled)
                    {
                        if(string.IsNullOrEmpty(lsStyleSetTarget.Remark))
                        {
                            lsStyleSetTarget.Remark = "Hủy";
                        }    
                        else
                        {
                            lsStyleSetTarget.Remark = "Hủy " + lsStyleSetTarget.Remark;
                        }    
                    }    
                }    

            }    
            return result;
        }
        public IEnumerable<CuttingCardDtos> GetCardMergeBlockLSStyle(string companyID, DateTime produceDate)
        {
            var ProduceDateStr = produceDate.ToString("yyyy-MM-dd");
            
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyID",companyID ?? string.Empty),
                new SqlParameter("@ProduceDate", produceDate),
               
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectCardLSStyle", parameters);
            var result = table.AsListObject<CuttingCardDtos>();
            
            return result;
        }
        public IEnumerable<AllocDailyOutputChangeDtos> CheckChangeOrderQuantity(string companyID)
        {
            var result = (from d in context.DailyTarget.Where(d => d.Operation == "CUTTING" && d.CompanyID == companyID && d.IsCanceled == false)
                          from a in context.AllocDailyOutput.Where(a => a.Operation == "CUTTING" && a.TargetID == d.ID)
                          from i in context.ItemStyle.Where(i => i.LSStyle == a.LSStyle 
                                                          && i.TotalQuantity !=0  
                                                          && ((
                                                                i.SalesOrderID.Contains("HADDAD") && i.SalesOrder.CustomerID.Equals("HA"))
                                                                || !i.SalesOrder.CustomerID.Equals("HA")
                                                          ))
                          from s in context.SalesOrders.Where(s => s.ID == i.SalesOrderID)
                          from o in context.OrderDetail.Where(o => o.ItemStyleNumber == i.Number && o.Size.Trim().ToUpper() == a.Size.Trim().ToUpper()
                                                             && (o.Quantity - (o.ConsumedQuantity == null ? 0 : o.ConsumedQuantity)) != a.OrderQuantity)
                          select new AllocDailyOutputChangeDtos
                          {
                              ID = a.ID,
                              LSStyle = a.LSStyle,
                              TotalQuantity = d.TargetQuantity,
                              FabricContrastName = a.FabricContrastName,
                              Set = a.Set,
                              Size = a.Size,
                              OldQuantity = a.OrderQuantity,
                              NewQuantity = (decimal)(o.Quantity - (o.ConsumedQuantity == null ? 0 : o.ConsumedQuantity)),
                              CuttingQuantity = a.Quantity,
                              IsFull = a.IsFull,
                              Operation = a.Operation,
                              Remark = d.Remark
                          }).ToList();
            return result;
        }
        public IEnumerable<AllocDailyOutputChangeDtos> UpdateChangeOrderQuantity(List<string> listID )
        {
            var userName = listID.Last();
            listID.Remove(userName);
            var result = (from a in context.AllocDailyOutput.Where(a => a.Operation == "CUTTING" && listID.Contains(a.ID.ToString()))
                          from i in context.ItemStyle.Where(i => i.LSStyle == a.LSStyle
                                                          && i.TotalQuantity != 0
                                                          && ((
                                                                i.SalesOrderID.Contains("HADDAD") && i.SalesOrder.CustomerID.Equals("HA"))
                                                                || !i.SalesOrder.CustomerID.Equals("HA")
                                                          ))
                          from s in context.SalesOrders.Where(s => s.ID == i.SalesOrderID)
                          from o in context.OrderDetail.Where(o => o.ItemStyleNumber == i.Number && o.Size.Trim().ToUpper() == a.Size.Trim().ToUpper()
                                                             && (o.Quantity - (o.ConsumedQuantity == null ? 0 : o.ConsumedQuantity)) != a.OrderQuantity)
                          select new AllocDailyOutputChangeDtos
                          {
                              ID = a.ID,
                              LSStyle = a.LSStyle,                             
                              FabricContrastName = a.FabricContrastName,
                              Size = a.Size,
                              Set = a.Set,
                              OldQuantity = a.OrderQuantity,
                              NewQuantity = (decimal)(o.Quantity - (o.ConsumedQuantity == null ? 0 : o.ConsumedQuantity)),
                              CuttingQuantity = a.Quantity,
                              IsFull = a.IsFull,
                              Operation = a.Operation
                          }).ToList();
            foreach (var updateAllocDailyOutput in result)
            {
                var existAllocDaiylyOutputs = context.AllocDailyOutput.Where(x=>x.LSStyle == updateAllocDailyOutput.LSStyle
                                                && x.Size == updateAllocDailyOutput.Size
                                                && x.Operation == updateAllocDailyOutput.Operation
                                                && x.FabricContrastName == updateAllocDailyOutput.FabricContrastName
                                                && x.ID == updateAllocDailyOutput.ID 
                                                ).ToList();
                foreach(var existAllocDaiylyOutput in existAllocDaiylyOutputs)
                {
                    existAllocDaiylyOutput.OrderQuantity = updateAllocDailyOutput.NewQuantity;  
                    if(existAllocDaiylyOutput.Quantity < existAllocDaiylyOutput.OrderQuantity + existAllocDaiylyOutput.Sample + existAllocDaiylyOutput.PercentQuantity)
                    {
                        existAllocDaiylyOutput.IsFull = false;
                    }    
                    existAllocDaiylyOutput.SetUpdateAudit(userName);
                }  
                context.UpdateRange(existAllocDaiylyOutputs);
                context.SaveChanges();
            }    
            return result;
        }
        public IEnumerable<CuttingOutputReportDtos> GetCuttingOutputReport(string customerID, string lsStyle, DateTime fromDate, DateTime toDate)
        {

            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID", customerID ?? string.Empty),
                new SqlParameter("@LSStyle", lsStyle ?? string.Empty),
                new SqlParameter("@FROMDATE", fromDate),
                new SqlParameter("@TODATE", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_CuttingOutput", parameters);
            return table.AsListObject<CuttingOutputReportDtos>();
        }
    }
}
