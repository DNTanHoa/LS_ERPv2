using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;

using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class DailyTargetQueries : IDailyTargetQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        public DailyTargetQueries(SqlServerAppDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<DailyTargetDtos> GetAll()
        {
            var result =  context.DailyTarget;
            return result.Select(x=>mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetByDate(DateTime date,List<string> departmentIds)
        {
            var a = date.Date;
            var b = date.Date.AddDays(1);
            //var result = context.DailyTarget.Where(x=>x.ProduceDate.Day == date.Day 
            //                                        && x.ProduceDate.Month == date.Month
            //                                        && x.ProduceDate.Year == date.Year);
            var listWorkCenterID = context.WorkCenter.Where(x => departmentIds.Contains(x.DepartmentID)).Select(x=>x.ID).ToList();
            var result = context.DailyTarget.Include(i=>i.JobOutput).Where(x=>x.ProduceDate.Date >= a.Date && x.ProduceDate.Date < b.Date
                                                                            && listWorkCenterID.Contains(x.WorkCenterID)
                                                                            );           
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetByCompanyID( string companyID,DateTime date)
        {
            var a = date.Date;
            var b = date.Date.AddDays(1);
            var result = context.DailyTarget.Include(i => i.JobOutput).Where(x => x.ProduceDate.Date >= a.Date && x.ProduceDate.Date < b.Date
                                                                            && x.WorkCenterID.Contains(companyID)
                                                                            ); 
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetByWorkCenter(DateTime date, string departmentId)
        {
            var result = context.DailyTarget.Where(x => x.ProduceDate.Day == date.Day
                                                                             && x.ProduceDate.Month == date.Month
                                                                             && x.ProduceDate.Year == date.Year
                                                                            && x.WorkCenterID == departmentId
                                                                            );
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetById(int id)
        {
            var result = context.DailyTarget.Where(x=>x.ID ==id);
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetCuttingTarget(string companyID, DateTime fromDate, DateTime toDate, string operation)
        {
            var result = context.DailyTarget.Where(x => x.Operation.Equals(operation)
                                                    && x.ToDate.Date <= toDate.Date
                                                    && x.FromDate.Date >= fromDate.Date
                                                    && x.CompanyID.Equals(companyID)
                                                    
                                                    );
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetSewingTarget(string companyID, DateTime fromDate, DateTime toDate, string operation)
        {
            var result = context.DailyTarget.Where(x => x.Operation.Equals(operation)
                                                    && x.ProduceDate.Date <= toDate.Date
                                                    && x.ProduceDate.Date >= fromDate.Date
                                                    //&& x.CompanyID.Equals(companyID)
                                                    && x.WorkCenterID.Contains(companyID)
                                                    );
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<string> GetMergeBlockLSStyle(string companyID, DateTime fromDate, DateTime toDate,bool IsPrint)
        {
            //var result = context.DailyTarget.Where(x => x.ToDate.Date <= toDate.Date
            //                                        && x.FromDate.Date >= fromDate.Date
            //                                        && x.CompanyID.Equals(companyID)
            //                                        && x.Operation == "CUTTING"
            //                                        && x.IsCanceled == false
            //                                        ).OrderBy(x=>x.MergeBlockLSStyle).Select(x=>x.MergeBlockLSStyle).Distinct().ToList();
            //return result;

            var result = (from d in context.DailyTarget.Where(x=> x.CompanyID.Equals(companyID)
                                                                && x.Operation == "CUTTING"
                                                                && x.IsCanceled == false)
                         from a in context.AllocDailyOutput.Where(a=>a.TargetID == d.ID && (a.IsFull == false || IsPrint == true))
                         select new  { d.MergeBlockLSStyle }).Distinct().AsEnumerable().OrderBy(x => x.MergeBlockLSStyle).Select(x => x.MergeBlockLSStyle).ToList();
            return result;
        }
        public IEnumerable<string> GetAllMergeBlockLSStyle(string companyID)
        {
            var result = context.DailyTarget.Where(x =>  x.CompanyID.Equals(companyID)
                                                    && x.Operation == "CUTTING"
                                                    && x.IsCanceled == false
                                                    ).OrderBy(x => x.MergeBlockLSStyle).Select(x => x.MergeBlockLSStyle).Distinct().ToList();
            return result;


        }
        public IEnumerable<string> GetMergeBlockLSStyleQuantity(string companyID, DateTime fromDate, DateTime toDate
            , string mergeBlockLSStyle,string size)
        {
            var result = new List<string>();
            var dailyTarget = context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                    && x.Operation == "CUTTING"
                                                    && x.IsCanceled == false
                                                    && x.MergeBlockLSStyle == mergeBlockLSStyle).ToList();                                                  
                                                   
            var quantityTarget =  context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                            && dailyTarget.Select(x=>x.ID).ToList().Contains(a.TargetID)
                                                                             && (a.Size == size || string.IsNullOrEmpty(size)))
                                                            .Select(a => a.OrderQuantity + a.PercentQuantity + a.Sample).Sum();
            var Set = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                        && dailyTarget.Select(x => x.ID).ToList().Contains(a.TargetID)
                                                        && (a.Size == size || string.IsNullOrEmpty(size)))
                                                .Select(a => a.Set).Distinct().ToList();
            if(Set.Count>0)
            {
                quantityTarget = quantityTarget / Set.Count();
            }   

            var allocQuantity = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                            && dailyTarget.Select(x => x.ID).ToList().Contains(a.TargetID)
                                                                             && (a.Size == size || string.IsNullOrEmpty(size)))
                                                            .Select(a => a.Quantity).Sum();
            if (Set.Count > 0)
            {
                allocQuantity = allocQuantity / Set.Count();
            }

            result.Add(((int)quantityTarget).ToString());
            var issue = quantityTarget - allocQuantity;
            result.Add(((int)issue).ToString());
     
            return result;
        }
        public IEnumerable<string> GetMergeLSStyleQuantity(string companyID, DateTime fromDate, DateTime toDate, string mergeLSStyle,string size)
        {
            var result = new List<string>();
            var dailyTarget = context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                   && x.Operation == "CUTTING"
                                                   && x.IsCanceled == false
                                                   && x.MergeLSStyle == mergeLSStyle).ToList();
                                                   
            var quantityTarget = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                    && dailyTarget.Select(d=>d.ID).ToList().Contains(a.TargetID)
                                                                        && (a.Size == size || string.IsNullOrEmpty(size)))
                                                        .Select(a => a.OrderQuantity + a.PercentQuantity + a.Sample).Sum();
            var Set = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                        && dailyTarget.Select(x => x.ID).ToList().Contains(a.TargetID)
                                                        && (a.Size == size || string.IsNullOrEmpty(size)))
                                                .Select(a => a.Set).Distinct().ToList();
            if (Set.Count > 0)
            {
                quantityTarget = quantityTarget / Set.Count();
            }
           
            var allocQuantity = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                            && dailyTarget.Select(x => x.ID).ToList().Contains(a.TargetID)
                                                                             && (a.Size == size || string.IsNullOrEmpty(size)))
                                                            .Select(a => a.Quantity).Sum();
            if (Set.Count > 0)
            {
                allocQuantity = allocQuantity / Set.Count();
            }
            result.Add(((int)quantityTarget).ToString());
            var issue = quantityTarget - allocQuantity;
            result.Add(((int)issue).ToString());
            return result;
        }
        public IEnumerable<string> GetLSStyleQuantity(string companyID, DateTime fromDate, DateTime toDate, string lsStyle,string size)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(lsStyle))
            {
                var dailyTarget = context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                        && x.Operation == "CUTTING"
                                                        && x.IsCanceled == false
                                                        && x.LSStyle == lsStyle).FirstOrDefault();
                //var set = dailyTarget.Set.Split(";").ToList();
                var quantityTarget = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                    && dailyTarget.ID == a.TargetID
                                                                        && (a.Size == size || string.IsNullOrEmpty(size)))
                                                        .Select(a => a.OrderQuantity + a.PercentQuantity + a.Sample).Sum();
                var Set = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                            && dailyTarget.ID == a.TargetID
                                                            && (a.Size == size || string.IsNullOrEmpty(size)))
                                                    .Select(a => a.Set).Distinct().ToList();
                if (Set.Count > 0)
                {
                    quantityTarget = quantityTarget / Set.Count();
                }
                var allocQuantity = context.AllocDailyOutput.Where(a => a.FabricContrastName == "A"
                                                                           && dailyTarget.ID == a.TargetID
                                                                            && (a.Size == size || string.IsNullOrEmpty(size)))
                                                           .Select(a => a.Quantity).Sum();
                if (Set.Count > 0)
                {
                    allocQuantity = allocQuantity / Set.Count();
                }
                result.Add(((int)quantityTarget).ToString());
                var issue = quantityTarget - allocQuantity;
                result.Add(((int)issue).ToString());
            }
            return result;
        }
        public IEnumerable<string> GetCuttingCustomer(string companyID, DateTime fromDate, DateTime toDate)
        {
            var result = context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                    && x.Operation == "CUTTING"
                                                    && x.IsCanceled == false
                                                    && x.CustomerName != null
                                                    ).OrderBy(x => x.CustomerName).Select(x => x.CustomerName).Distinct().ToList();
            return result;
            //////
            //var result = (from d in context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
            //                                                    && x.Operation == "CUTTING"
            //                                                    && x.IsCanceled == false
            //                                                    && x.CustomerName != null)
            //              from a in context.AllocDailyOutput.Where(a => a.TargetID == d.ID && a.IsFull == false)
            //              select new { d.CustomerName }).Distinct().AsEnumerable().OrderBy(x => x.CustomerName).Select(x => x.CustomerName).ToList();
            //return result;
        }
        public IEnumerable<string> GetCuttingLSStyle(string companyID, DateTime fromDate, DateTime toDate,string mergeLSStyle, string mergeBlockLSStyle,bool isPrint)
        {
            var result = new List<string>();
            if(string.IsNullOrEmpty(mergeLSStyle))
            {
                //result = context.DailyTarget.Where(x => x.ToDate.Date <= toDate.Date
                //                                    && x.FromDate.Date >= fromDate.Date
                //                                    && x.CompanyID.Equals(companyID)
                //                                    && x.Operation == "CUTTING"
                //                                    && x.IsCanceled == false
                //                                    && x.LSStyle.Contains(mergeBlockLSStyle)
                //                                    ).OrderBy(x => x.LSStyle).Select(x => x.LSStyle).ToList();

                result = (from d in context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                                && x.Operation == "CUTTING"
                                                                && x.IsCanceled == false
                                                                && x.LSStyle.Contains(mergeBlockLSStyle))
                              from a in context.AllocDailyOutput.Where(a => a.TargetID == d.ID && (a.IsFull == false || isPrint == true))
                              select new { d.LSStyle }).Distinct().AsEnumerable().OrderBy(x => x.LSStyle).Select(x => x.LSStyle).ToList();
            }  
            else
            {
                if(!string.IsNullOrEmpty(mergeBlockLSStyle))
                {
                    var prefix = mergeBlockLSStyle.Substring(0, mergeBlockLSStyle.LastIndexOf("-"));
                    var items = mergeLSStyle.Split("+");
                    var lsStyles= new List<string>();
                    foreach (var item in items)
                    {
                        lsStyles.Add(prefix + "-" + item);
                    }
                    //result = context.DailyTarget.Where(x => x.ToDate.Date <= toDate.Date
                    //                                && x.FromDate.Date >= fromDate.Date
                    //                                && x.CompanyID.Equals(companyID)
                    //                                && x.Operation == "CUTTING"
                    //                                && x.IsCanceled == false
                    //                                && lsStyles.Contains(x.LSStyle)
                    //                                ).OrderBy(x => x.LSStyle).Select(x => x.LSStyle).ToList();
                    result = (from d in context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                                && x.Operation == "CUTTING"
                                                                && x.IsCanceled == false
                                                                && lsStyles.Contains(x.LSStyle))
                              from a in context.AllocDailyOutput.Where(a => a.TargetID == d.ID && (a.IsFull == false|| isPrint == true))
                              select new { d.LSStyle }).Distinct().AsEnumerable().OrderBy(x => x.LSStyle).Select(x => x.LSStyle).ToList();
                }    
            }    
            
            return result;
        }
        public IEnumerable<string> GetMergeLSStyle(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle,bool isPrint)
        {
            //var result = context.DailyTarget.Where(x => x.ToDate.Date <= toDate.Date
            //                                        && x.FromDate.Date >= fromDate.Date
            //                                        && x.CompanyID.Equals(companyID)
            //                                        && x.Operation == "CUTTING"
            //                                        && x.IsCanceled == false
            //                                        && x.MergeBlockLSStyle.Equals(mergeBlockLSStyle)
            //                                        ).OrderBy(x => x.MergeLSStyle).Select(x => x.MergeLSStyle).Distinct().ToList();
            //return result;
            var result = (from d in context.DailyTarget.Where(x => x.CompanyID.Equals(companyID)
                                                   && x.Operation == "CUTTING"
                                                   && x.IsCanceled == false
                                                   && x.MergeBlockLSStyle.Equals(mergeBlockLSStyle))
                          from a in context.AllocDailyOutput.Where(a => a.TargetID == d.ID && (a.IsFull == false || isPrint == true))
                          select new { d.MergeLSStyle }).Distinct().AsEnumerable().OrderBy(x => x.MergeLSStyle).Select(x => x.MergeLSStyle).ToList();

            return result;
        }
        public IEnumerable<DailyTargetDtos> GetByWorkCenterId(string workCenterId)
        {
            var a = DateTime.Now;
            var b = a.AddDays(1);
            var result = context.DailyTarget.Include(i=>i.JobOutput)
                                            .Where(x => x.WorkCenterID == workCenterId
                                                  && (x.ProduceDate.Date >= a.Date && x.ProduceDate.Date < b.Date)
                                                    );
            return result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<DailyTargetDtos> GetByListWorkCenterId(List<string> listWorkCenterId)
        {
            var a = DateTime.Now;
            var b = a.AddDays(1);
            var result = context.DailyTarget.Include(i => i.JobOutput)
                                            .Where(x => listWorkCenterId.Contains(x.WorkCenterID)
                                                  && (x.ProduceDate.Date >= a.Date && x.ProduceDate.Date < b.Date)
                                                    );
            var listDailyTarget = result.Select(x => mapper.Map<DailyTargetDtos>(x)).ToList();
            int i = 0;
            foreach (DailyTargetDtos dailyTargetDtos in listDailyTarget)
            {
                decimal total = 0;
                foreach (var Job in dailyTargetDtos.JobOutput)
                {
                    if (Job.Quantity != null)
                    {
                        total += (decimal)Job.Quantity;
                    }
                }
                dailyTargetDtos.TotalQuantity = total;     
            }
            return listDailyTarget;//result.Select(x => mapper.Map<DailyTargetDtos>(x));
        }
        public IEnumerable<LSStyleCompareDtos> CheckOffsetLSStyle(BulkLSStyleCompareDtos bulkLSStyleCompareDtos)
        {
            var result = (from l in bulkLSStyleCompareDtos.Data
                         from o in context.OrderDetail.Where(o=>o.ItemStyle.LSStyle == l.LSStyle)
                         select new LSStyleCompareDtos
                         {
                             LSStyle = l.LSStyle,
                             TargetQuantity = l.TargetQuantity,
                             OrderQuantity = (decimal)o.ReservedQuantity
                         }).ToList().GroupBy(g=>new { g.LSStyle,g.TargetQuantity})
                                    .Select(s=>new LSStyleCompareDtos
                                    {
                                        LSStyle = s.Key.LSStyle,
                                        TargetQuantity = s.Key.TargetQuantity,
                                        OrderQuantity = s.Sum(t=>t.OrderQuantity)
                                    }).OrderBy(x=>x.LSStyle).ToList();

            foreach(var r in result)
            {
                if(r.TargetQuantity== r.OrderQuantity)
                {
                    r.Result = "New";
                }    
                else
                {
                    r.Result = "Offset";
                }    
            }    

            return result;

        }
        public IEnumerable<DailyTagetOverviewDtos> GetOverview(string companyID)
        {
            var a = DateTime.Now;
            var b = a.AddDays(1);            
            
            var dailyTargets = context.DailyTarget.Where(x => x.ProduceDate.Date >= a.Date 
                                                            && x.ProduceDate.Date < b.Date
                                                            && x.Operation=="SEWING"
                                                            && x.WorkCenterID.Contains(companyID));
            //var resultOverviews = result.Select(x => mapper.Map<DailyTagetOverviewDtos>(x));
            var dailyTargetOverviewDtoss = new List<DailyTagetOverviewDtos>();
            if(dailyTargets != null)
            {
                if(dailyTargets.Count()>0)
                {
                    foreach(var dailyTarget in dailyTargets)
                    {
                        var dailyTargetOverviewDtos = mapper.Map<DailyTagetOverviewDtos>(dailyTarget);                      
                        var lastJobOutput = context.JobOuput.Where(x => x.DailyTargetID == dailyTarget.ID
                                                                    //&& x.Efficiency != null
                                                                    && x.Quantity != null
                                                                    ).OrderBy(x=>x.WorkingTimeID).LastOrDefault();                        
                        if(lastJobOutput!=null)
                        {
                            dailyTargetOverviewDtos.NumberOfWorker = lastJobOutput.NumberOfWorker;                            
                            //dailyTargetOverviewDtos.TotalTargetQuantity = (decimal)lastJobOutput.TargetQuantity;
                            decimal percent = 0;
                            if (lastJobOutput.Quantity!=null)
                            {
                                dailyTargetOverviewDtos.TotalOutputQuantity = (decimal)lastJobOutput.Quantity;
                                if(lastJobOutput.TargetQuantity !=null)
                                {
                                    dailyTargetOverviewDtos.TotalTargetQuantity = (decimal)lastJobOutput.TargetQuantity;
                                    if (lastJobOutput.TargetQuantity !=0)
                                    {
                                        percent = decimal.Round((decimal)(100 * lastJobOutput.Quantity / lastJobOutput.TargetQuantity), 2);
                                    }                                        
                                } 
                                dailyTargetOverviewDtos.EfficiencyStr = percent.ToString("F") + "%";
                                dailyTargetOverviewDtos.Efficiency = percent;
                                dailyTargetOverviewDtos.LastUpdateAt = lastJobOutput.LastUpdatedAt;
                                dailyTargetOverviewDtos.Problem = lastJobOutput.Problem;                                
                            }
                            
                            if(percent<85)
                            {
                                dailyTargetOverviewDtos.Status = "danger";
                            }    
                            else if(percent< 100)
                            {
                                dailyTargetOverviewDtos.Status = "warning";
                            }    
                            else
                            {
                                dailyTargetOverviewDtos.Status = "success";
                            }    
                        }
                        else
                        {
                            dailyTargetOverviewDtos.EfficiencyStr = "0%";
                            dailyTargetOverviewDtos.Status = "danger";

                        }
                        dailyTargetOverviewDtoss.Add(dailyTargetOverviewDtos);
                    }    
                }    
            }  

            return dailyTargetOverviewDtoss.OrderBy(x=>x.Efficiency).ToList();
        }
    }
}
