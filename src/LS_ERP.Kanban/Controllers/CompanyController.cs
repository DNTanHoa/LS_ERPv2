using Common.Model;
using Logic.Config;
using LS_ERP.Kanban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ultils.Config;
using Ultils.Extensions;

namespace LS_ERP.Kanban.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly BackEndConfig backEndConfig;
        private readonly ErrorConfig errorConfig;
        public CompanyController(IHttpClientFactory httpClientFactory,
           IOptions<BackEndConfig> backEndConfig,
           IOptions<ErrorConfig> errorConfig)
        {
            this.httpClientFactory = httpClientFactory;
            this.backEndConfig = backEndConfig.Value;
            this.errorConfig = errorConfig.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetCompany(string? keyWord)
        {
            var companies = new List<CompanyModel>();
            var response = new CommonResponseModel<List<CompanyModel>>();

            if (ModelState.IsValid)
            {
                var client = httpClientFactory.CreateClient();
                var url = backEndConfig.Host + backEndConfig.GetCompany;

                if (!string.IsNullOrEmpty(keyWord))
                {
                    url += "?keyWord=" + keyWord;
                }
                var remoteResponse = client
                        .ExecuteGet<CommonResponseModel<List<CompanyModel>>>(url);

                if (remoteResponse.Result.StatusCode ==
                System.Net.HttpStatusCode.OK)
                {
                    var remoteData = remoteResponse.Result.Data;

                    if (remoteData != null)
                    {
                        if (remoteData.Success)
                        {
                            companies = remoteData.Data ?? new List<CompanyModel>();
                            response.Success = true;
                            response.Data = companies;
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

            return Json(response);
        }

    }
}
