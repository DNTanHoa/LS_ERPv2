using Common.Model;
using LS_ERP.Kanban.Models;
using LS_ERP.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ultils.Config;
using Ultils.Extensions;

namespace LS_ERP.Kanban.Controllers
{
    public class WorkCenterController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly BackEndConfig backEndConfig;
        public WorkCenterController(IHttpClientFactory httpClientFactory,
        IOptions<BackEndConfig> backEndConfig)
        {
            this.httpClientFactory = httpClientFactory;
            this.backEndConfig = backEndConfig.Value;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult GetAllWorkCenter()
        {
            var workCenters = new List<WorkCenterModel>();
            var response = new CommonResponseModel<List<WorkCenterModel>>();

            var client = httpClientFactory.CreateClient();
            var url = backEndConfig.HostErp + backEndConfig.GetAllWorkCenter;
           
            var remoteResponse = client
                    .ExecuteGet<CommonResponseModel<List<WorkCenterModel>>>(url);

            if (remoteResponse.Result.StatusCode ==
            System.Net.HttpStatusCode.OK)
            {
                var remoteData1 = remoteResponse.Result.Data;
                if (remoteData1 != null)
                {
                    workCenters = remoteData1.Data ?? new List<WorkCenterModel>();
                    response.Success = true;
                    response.Data = workCenters.Where(x=>x.WorkCenterTypeID=="SEWING").ToList();
                }

            }
            return Json(response);
        }
    }
}
