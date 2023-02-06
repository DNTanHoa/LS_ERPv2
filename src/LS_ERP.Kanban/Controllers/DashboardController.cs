using LS_ERP.Kanban.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Options;

using Common.Model;
using Ultils.Extensions;
using Ultils.Config;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Logic.Config;
using LS_ERP.Kanban.Logic;

namespace LS_ERP.Kanban.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly BackEndConfig backEndConfig;   
        private readonly ErrorConfig errorConfig;
        private readonly NoteConfig noteConfig;
        private readonly IAuthenticateHelper authenticateHelper;
        public DashboardController(IHttpClientFactory httpClientFactory,
    IOptions<BackEndConfig> backEndConfig,
    IOptions<ErrorConfig> errorConfig,
    IOptions<NoteConfig> noteConfig,
    IAuthenticateHelper authenticateHelper)
        {
            this.httpClientFactory = httpClientFactory;
            this.authenticateHelper = authenticateHelper;
            this.backEndConfig = backEndConfig.Value;
            this.errorConfig = errorConfig.Value;
            this.noteConfig = noteConfig.Value;
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Index()
        {
            return View();
            //var dailyTargetOverviews = new List<DailyTargetOverviewModel>();
           
            //var client = httpClientFactory.CreateClient();
            //var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetOverview;         


            //var remoteResponse = client
            //        .ExecuteGet<CommonResponseModel<List<DailyTargetOverviewModel>>>(url);

            //if (remoteResponse.Result.StatusCode ==
            //System.Net.HttpStatusCode.OK)
            //{
            //    var remoteData = remoteResponse.Result.Data;

            //    if (remoteData != null)
            //    {
            //        dailyTargetOverviews = remoteData.Data ?? new List<DailyTargetOverviewModel>();
                 
            //    }
            //}
            //var numberOfWorker = 0;
            //var totalLine = dailyTargetOverviews.Count();
            //var lineRed = 0;
            //var lineYellow = 0;

            //foreach(var item in dailyTargetOverviews)
            //{
            //    numberOfWorker += item.NumberOfWorker;
            //    if(item.Efficiency<85)
            //    {
            //        lineRed++;
            //    }    
            //    else if(item.Efficiency<100 )
            //    {
            //        lineYellow++;
            //    } 
            //}           
            //ViewBag.TotalLines = totalLine;
            //ViewBag.TotalNumberOfWorkers = numberOfWorker.ToString();
            //ViewBag.TotalRed = lineRed.ToString();
            //ViewBag.TotalYellow = lineYellow.ToString();
            //ViewBag.TotalGreen = totalLine - lineRed - lineYellow;
            //return View(dailyTargetOverviews);
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Overview(string companyID)
        {
            var dailyTargetOverviews = new List<DailyTargetOverviewModel>();

            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetOverview+"?CompanyID="+companyID;


            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<DailyTargetOverviewModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    dailyTargetOverviews = remoteData.Data ?? new List<DailyTargetOverviewModel>();

                }
            }
            var numberOfWorker = 0;
            var totalLine = dailyTargetOverviews.Count();
            var lineRed = 0;
            var lineYellow = 0;

            foreach (var item in dailyTargetOverviews)
            {
                numberOfWorker += item.NumberOfWorker;
                if (item.Efficiency < 85)
                {
                    lineRed++;
                }
                else if (item.Efficiency < 100)
                {
                    lineYellow++;
                }
            }
            ViewBag.TotalLines = totalLine;
            ViewBag.TotalNumberOfWorkers = numberOfWorker.ToString();
            ViewBag.TotalRed = lineRed.ToString();
            ViewBag.TotalYellow = lineYellow.ToString();
            ViewBag.TotalGreen = totalLine - lineRed - lineYellow;
            //return View(dailyTargetOverviews);
            return PartialView("Overview", dailyTargetOverviews);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Compare()
        {
            //var dailyTargetOverviews = new List<DailyTargetOverviewModel>();

            //var client = httpClientFactory.CreateClient();
            //var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetOverview;


            //var remoteResponse = client
            //        .ExecuteGet<CommonResponseModel<List<DailyTargetOverviewModel>>>(url);

            //if (remoteResponse.Result.StatusCode ==
            //System.Net.HttpStatusCode.OK)
            //{
            //    var remoteData = remoteResponse.Result.Data;

            //    if (remoteData != null)
            //    {
            //        dailyTargetOverviews = remoteData.Data ?? new List<DailyTargetOverviewModel>();

            //    }
            //}
            //var numberOfWorker = 0;
            //var totalLine = dailyTargetOverviews.Count();
            //var lineRed = 0;
            //var lineYellow = 0;

            //foreach (var item in dailyTargetOverviews)
            //{
            //    numberOfWorker += item.NumberOfWorker;
            //    if (item.Efficiency < 85)
            //    {
            //        lineRed++;
            //    }
            //    else if (item.Efficiency < 100)
            //    {
            //        lineYellow++;
            //    }
            //}            
            //ViewBag.TotalLines = totalLine;
            //ViewBag.TotalNumberOfWorkers = numberOfWorker.ToString();
            //ViewBag.TotalRed = lineRed.ToString();
            //ViewBag.TotalYellow = lineYellow.ToString();
            //ViewBag.TotalGreen = totalLine - lineRed - lineYellow;
            ////
            //var styleNOs = dailyTargetOverviews.Select(x => x.StyleNO).Distinct().ToList();
            //var dailyTargetCompareOverviews = new List<DailyTargetCompareOverviewModel>();
            //foreach(var styleNO in styleNOs)
            //{
            //    var dailyTargetCompareOverview = new DailyTargetCompareOverviewModel();
            //    dailyTargetCompareOverview.StyleNO = styleNO;
            //    dailyTargetCompareOverview.DailyTargetOverviewModels = dailyTargetOverviews.Where(x => x.StyleNO.Equals(styleNO)).ToList();
            //    if(dailyTargetCompareOverview.DailyTargetOverviewModels.Count>1)
            //    {
            //        dailyTargetCompareOverviews.Add(dailyTargetCompareOverview);
            //    } 
            //}    

            ////

            //return View(dailyTargetCompareOverviews);
            return View();
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult GetCompare(string companyID)
        {
            var dailyTargetOverviews = new List<DailyTargetOverviewModel>();

            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetOverview + "?CompanyID=" + companyID;


            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<DailyTargetOverviewModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    dailyTargetOverviews = remoteData.Data ?? new List<DailyTargetOverviewModel>();

                }
            }
            var numberOfWorker = 0;
            var totalLine = dailyTargetOverviews.Count();
            var lineRed = 0;
            var lineYellow = 0;

            foreach (var item in dailyTargetOverviews)
            {
                numberOfWorker += item.NumberOfWorker;
                if (item.Efficiency < 85)
                {
                    lineRed++;
                }
                else if (item.Efficiency < 100)
                {
                    lineYellow++;
                }
            }
            ViewBag.TotalLines = totalLine;
            ViewBag.TotalNumberOfWorkers = numberOfWorker.ToString();
            ViewBag.TotalRed = lineRed.ToString();
            ViewBag.TotalYellow = lineYellow.ToString();
            ViewBag.TotalGreen = totalLine - lineRed - lineYellow;
            //
            var styleNOs = dailyTargetOverviews.Select(x => x.StyleNO).Distinct().ToList();
            var dailyTargetCompareOverviews = new List<DailyTargetCompareOverviewModel>();
            foreach (var styleNO in styleNOs)
            {
                var dailyTargetCompareOverview = new DailyTargetCompareOverviewModel();
                dailyTargetCompareOverview.StyleNO = styleNO;
                dailyTargetCompareOverview.DailyTargetOverviewModels = dailyTargetOverviews.Where(x => x.StyleNO.Equals(styleNO)).ToList();
                if (dailyTargetCompareOverview.DailyTargetOverviewModels.Count > 1)
                {
                    dailyTargetCompareOverviews.Add(dailyTargetCompareOverview);
                }
            }

            //

            return PartialView("PartialCompare",dailyTargetCompareOverviews);
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Route(string ids)
        {
            var dailyTargetModels = new List<DailyTargetModel>();
            var dailyTargetModel = new DailyTargetModel();
            var model = new DailyTargetSearchModel();
            model.ListWorkCenterID = ids;
            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetByListWorkCenterID;
            //url += "?" + model.ToQueryString(); 
            //
            var str = ids;
            var listIds = new List<string>();
            if (ids != null)
            {
                str = str.Replace("\"", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                var arr = str.Split(',');

                for (int i = 0; i < arr.Length; i++)
                {
                    listIds.Add(arr[i].Trim());
                }
            }
            var remoteResponse = client.ExecutePost<CommonResponseModel<List<DailyTargetModel>>>(url, listIds);
            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    dailyTargetModels = remoteData.Data ?? new List<DailyTargetModel>();
                }
            }
            if (dailyTargetModels != null)
            {
                if (dailyTargetModels.Count > 0)
                {
                    dailyTargetModel = dailyTargetModels[0];
                }
            }
            return RedirectToAction("Detail", dailyTargetModels);
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Detail(string ids)
        {
            var dailyTargetModels = new List<DailyTargetModel>();
            var dailyTargetModel = new DailyTargetModel();
            var model = new DailyTargetSearchModel();
            model.ListWorkCenterID = ids;
            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetByListWorkCenterID;
            //url += "?" + model.ToQueryString(); 
            //
            var str = ids;
            var listIds = new List<string>();
            if (ids != null)
            {
                str = str.Replace("\"", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                var arr = str.Split(',');

                for (int i = 0; i < arr.Length; i++)
                {
                    listIds.Add(arr[i].Trim());
                }
            }

            var remoteResponse = client.ExecutePost<CommonResponseModel<List<DailyTargetModel>>>(url, listIds);
            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    dailyTargetModels = remoteData.Data ?? new List<DailyTargetModel>();
                }
            }
            if (dailyTargetModels != null)
            {
                if (dailyTargetModels.Count > 0)
                {
                    dailyTargetModel = dailyTargetModels[0];
                }
            }
      
            return View(dailyTargetModels);
            
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Config()
        {
            return View();
        }
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Chart(DailyTargetDetailSearchModel model)
        {
            //model.ProduceDate = DateTime.Now;
            var curDate = DateTime.Now;
            var dayOfWeek = curDate.DayOfWeek;
            if(dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Monday) 
            {
                curDate = curDate.AddDays(-2);
            } 
            else
            {
                curDate = curDate.AddDays(-1);
            }
            model.ProduceDate = curDate;
            return View(model); 
        }
        public List<JobOutputByCustomerModel> GetJobOutputSummaryGroupByCustomerByDate(JobOutputSearchModel model)
        {
            var a = new List<JobOutputByCustomerModel>();
            var JobOutputs = new List<JobOutputSummaryModel>();
            var resultJobOutputs = new List<JobOutputSummaryModel>();
            var resultPivotJobOutput = new List<JobOutputSummaryModel>();
            var response = new CommonResponseModel<List<JobOutputSummaryModel>>();
            if (ModelState.IsValid)
            {
                var client = httpClientFactory.CreateClient();
                var url = backEndConfig.HostErp + backEndConfig.GetJobOutputSummaryByDate;
                url += "?" + model.ToQueryString();

                var remoteResponse = client
                        .ExecuteGet<CommonResponseModel<List<JobOutputSummaryModel>>>(url);

                if (remoteResponse.Result.StatusCode ==
                System.Net.HttpStatusCode.OK)
                {
                    var remoteData = remoteResponse.Result.Data;

                    if (remoteData != null)
                    {
                        if (remoteData.Success)
                        {
                            JobOutputs = remoteData.Data ?? new List<JobOutputSummaryModel>();
                            //get list department by company
                            //url = backEndConfig.Host + backEndConfig.GetDepartment; //+ "?keyWord=" + model.CompanyID;
                            url = backEndConfig.HostErp + backEndConfig.GetSewingWorkCenter + "?CompanyID=" + model.CompanyID;
                            var response1 = new CommonResponseModel<List<WorkCenterModel>>();
                            var remoteResponse1 = client.ExecuteGet<CommonResponseModel<List<WorkCenterModel>>>(url);
                            if (remoteResponse1.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var remoteData1 = remoteResponse1.Result.Data;
                                if (remoteData1 != null)
                                {
                                    var listDepartment = new List<WorkCenterModel>();
                                    listDepartment = remoteData1.Data ?? new List<WorkCenterModel>();
                                    var listDepartmentID = listDepartment.Select(x => x.ID).ToList();
                                    resultJobOutputs = JobOutputs.Where(x => listDepartmentID.Contains(x.DepartmentID)).ToList();
                                    a = resultJobOutputs
                                        .AsEnumerable()
                                        .GroupBy(x => new { x.CustomerName })
                                        .Select(y => new JobOutputByCustomerModel()
                                        {
                                            CustomerName = y.Key.CustomerName,
                                            Quantity = (decimal)y.Sum(t => t.Quantity),
                                            Lines = y.Select(x => x.WorkCenterID).Count()
                                        }
                                        ).ToList();
                                }
                            }

                            response.Success = true;
                            response.Data = resultJobOutputs;
                            //response.Data = JobOutputs;
                        }
                        else
                        {
                            response.Message = remoteData.Message;
                        }
                    }
                    else
                    {
                        response.Message = errorConfig.GetByKey("Unknown");
                    }
                }
                else
                {
                    response.Message = errorConfig.GetByKey("Unknown");
                }
            }
            else
            {
                var error = string.Join(";", ModelState.ToString());
                response.Message = error;
            }
            return a;
        }
        public List<JobOutputSummaryModel> GetTopMinJobOutputSummaryByDate(JobOutputSearchModel model)
        {
            var JobOutputs = new List<JobOutputSummaryModel>();
            var resultJobOutputs = new List<JobOutputSummaryModel>();
            var resultTopMinJobOutput = new List<JobOutputSummaryModel>();
            var response = new CommonResponseModel<List<JobOutputSummaryModel>>();
            //if (ModelState.IsValid)
            //{
            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetJobOutputSummaryByDate;
            url += "?" + model.ToQueryString();

            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<JobOutputSummaryModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    if (remoteData.Success)
                    {
                        JobOutputs = remoteData.Data ?? new List<JobOutputSummaryModel>();
                        //get list department by company
                        //url = backEndConfig.Host + backEndConfig.GetDepartment + "?keyWord=" + model.CompanyID;
                        url = backEndConfig.HostErp + backEndConfig.GetSewingWorkCenter + "?CompanyID=" + model.CompanyID;
                        var response1 = new CommonResponseModel<List<WorkCenterModel>>();
                        var remoteResponse1 = client.ExecuteGet<CommonResponseModel<List<WorkCenterModel>>>(url);
                        if (remoteResponse1.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var remoteData1 = remoteResponse1.Result.Data;
                            if (remoteData1 != null)
                            {
                                var listDepartment = new List<WorkCenterModel>();
                                listDepartment = remoteData1.Data ?? new List<WorkCenterModel>();
                                var listDepartmentID = listDepartment.Select(x => x.ID).ToList();
                                resultJobOutputs = JobOutputs.Where(x => listDepartmentID.Contains(x.DepartmentID)).ToList();
                                int i = 0;
                                foreach (var item in resultJobOutputs.OrderBy(x => x.Efficiency))
                                {
                                    if (i++ < 3)
                                    {
                                        resultTopMinJobOutput.Add(item);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        response.Success = true;
                        response.Data = resultTopMinJobOutput;
                        //response.Data = JobOutputs;
                    }
                    else
                    {
                        response.Message = remoteData.Message;
                    }
                }
                else
                {
                    response.Message = errorConfig.GetByKey("Unknown");
                }
            }
            else
            {
                response.Message = errorConfig.GetByKey("Unknown");
            }
            //}
            //else
            //{
            //    var error = string.Join(";", ModelState.GetError());
            //    response.Message = error;
            //}
            return resultTopMinJobOutput;
        }

        public List<JobOutputByCustomerModel> GetDailyTargetDetailSummaryGroupByCustomerByDate(DailyTargetDetailSearchModel model)
        {
            var result = new List<JobOutputByCustomerModel>();
            var dailyTargetDetails = new List<DailyTargetDetailModel>();
            var resultDailyTargetDetails = new List<DailyTargetDetailModel>();
            var response = new CommonResponseModel<List<DailyTargetDetailModel>>();

            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetDetailByDate;
            url += "?" + model.ToQueryString();

            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<DailyTargetDetailModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    if (remoteData.Success)
                    {
                        dailyTargetDetails = remoteData.Data ?? new List<DailyTargetDetailModel>();
                        //get list department by company
                        //url = backEndConfig.Host + backEndConfig.GetDepartment + "?keyWord=" + model.CompanyId;
                        url = backEndConfig.HostErp + backEndConfig.GetSewingWorkCenter + "?CompanyID=" + model.CompanyId;
                        var response1 = new CommonResponseModel<List<WorkCenterModel>>();
                        var remoteResponse1 = client.ExecuteGet<CommonResponseModel<List<WorkCenterModel>>>(url);
                        if (remoteResponse1.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var remoteData1 = remoteResponse1.Result.Data;
                            if (remoteData1 != null)
                            {
                                var listDepartment = new List<WorkCenterModel>();
                                listDepartment = remoteData1.Data ?? new List<WorkCenterModel>();
                                var listDepartmentID = listDepartment.Select(x => x.ID).ToList();
                                resultDailyTargetDetails = dailyTargetDetails.Where(x => listDepartmentID.Contains(x.WorkCenterID)).OrderBy(x => x.WorkCenterName).ToList();
                                //group by Line
                                var temp = resultDailyTargetDetails
                                        .AsEnumerable()
                                        .GroupBy(x => new { x.CustomerName,x.WorkCenterName })
                                        .Select(y => new DailyTargetDetailModel()
                                        {
                                            CustomerName = y.Key.CustomerName,
                                            WorkCenterName = y.Key.WorkCenterName,
                                            Quantity = (decimal)y.Sum(t => t.Quantity),
                                            Efficiency = (decimal)y.Sum(t => t.Efficiency)
                                        }
                                        ).ToList();

                                //
                                for (int i = 1; i < temp.Count; i++)
                                {
                                    if (temp[i].WorkCenterName == temp[i - 1].WorkCenterName
                                        && temp[i].CustomerName != temp[i - 1].CustomerName)
                                    {
                                        temp[i].WorkCenterName = null;
                                    }
                                }
                                //group by customer
                                result = temp
                                        .AsEnumerable()
                                        .GroupBy(x => new { x.CustomerName })
                                        .Select(y => new JobOutputByCustomerModel()
                                        {
                                            CustomerName = y.Key.CustomerName,
                                            Quantity = (decimal)y.Sum(t => t.Quantity),
                                            Lines = y.Where(x => !string.IsNullOrEmpty(x.WorkCenterName)).Count()
                                        }
                                        ).ToList();
                            }
                        }

                        response.Success = true;
                        response.Data = resultDailyTargetDetails;
                        //response.Data = DailyTargetDetails;
                    }
                    else
                    {
                        response.Message = remoteData.Message;
                    }
                }
                else
                {
                    response.Message = errorConfig.GetByKey("Unknown");
                }
            }
            else
            {
                response.Message = errorConfig.GetByKey("Unknown");
            }
            return result;
        }

        public List<JobOutputSummaryModel> GetTopMinDailyTargetDetailSummaryByDate(DailyTargetDetailSearchModel model)
        {
            var jobOutputSummaryModels = new List<JobOutputSummaryModel>();
            var DailyTargetDetails = new List<DailyTargetDetailModel>();
            var resultDailyTargetDetails = new List<DailyTargetDetailModel>();
            var resultTopMinDailyJobOutputSummary = new List<JobOutputSummaryModel>();
            var response = new CommonResponseModel<List<DailyTargetDetailModel>>();
            //if (ModelState.IsValid)
            //{
            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetDailyTargetDetailByDate;
            url += "?" + model.ToQueryString();

            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<DailyTargetDetailModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if (remoteData != null)
                {
                    if (remoteData.Success)
                    {
                        DailyTargetDetails = remoteData.Data ?? new List<DailyTargetDetailModel>();
                        //get list department by company
                        //url = backEndConfig.Host + backEndConfig.GetDepartment + "?keyWord=" + model.CompanyId;
                        url = backEndConfig.HostErp + backEndConfig.GetSewingWorkCenter + "?CompanyID=" + model.CompanyId;
                        var response1 = new CommonResponseModel<List<WorkCenterModel>>();
                        var remoteResponse1 = client.ExecuteGet<CommonResponseModel<List<WorkCenterModel>>>(url);
                        if (remoteResponse1.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var remoteData1 = remoteResponse1.Result.Data;
                            if (remoteData1 != null)
                            {
                                var listDepartment = new List<WorkCenterModel>();
                                listDepartment = remoteData1.Data ?? new List<WorkCenterModel>();
                                var listDepartmentID = listDepartment.Select(x => x.ID).ToList();
                                resultDailyTargetDetails = DailyTargetDetails.Where(x => listDepartmentID.Contains(x.WorkCenterID)).ToList();
                                //group by
                                jobOutputSummaryModels = resultDailyTargetDetails
                                      .AsEnumerable()
                                      .GroupBy(x => new { x.WorkCenterName })
                                      .Select(y => new JobOutputSummaryModel()
                                      {
                                          WorkCenterName = y.Key.WorkCenterName,
                                          Quantity = (decimal)y.Sum(t => t.Quantity),
                                          Efficiency = (decimal)y.Sum(t => t.Efficiency)
                                      }
                                      ).ToList();
                                //
                                int i = 0;
                                foreach (var item in jobOutputSummaryModels.OrderBy(x => x.Efficiency))
                                {
                                    if (i++ < 3)
                                    {
                                        resultTopMinDailyJobOutputSummary.Add(item);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        //response.Success = true;
                        //response.Data = resultTopMinDailyTargetDetail;
                        //response.Data = DailyTargetDetails;
                    }
                    else
                    {
                        response.Message = remoteData.Message;
                    }
                }
                else
                {
                    response.Message = errorConfig.GetByKey("Unknown");
                }
            }
            else
            {
                response.Message = errorConfig.GetByKey("Unknown");
            }
            //}
            //else
            //{
            //    var error = string.Join(";", ModelState.GetError());
            //    response.Message = error;
            //}
            return resultTopMinDailyJobOutputSummary;
        }
    }
}
