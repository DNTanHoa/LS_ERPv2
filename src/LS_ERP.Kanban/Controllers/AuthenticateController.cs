
using Common.Model;
using Logic.Config;
using LS_ERP.Kanban.Models;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using Ultils.Config;
using Ultils.Extensions;

namespace Admin.Controllers;
public class AuthenticateController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AuthenticateConfig authenConfig;
    private readonly ErrorConfig errorConfig;
    private readonly BackEndConfig backEndConfig;

    public AuthenticateController(IHttpClientFactory httpClientFactory,
        IOptions<BackEndConfig> backEndConfig,
        IOptions<ErrorConfig> errorConfig,
        IOptions<AuthenticateConfig> authenConfig)
    {
        this.httpClientFactory = httpClientFactory;
        this.authenConfig = authenConfig.Value;
        this.errorConfig = errorConfig.Value;
        this.backEndConfig = backEndConfig.Value;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login(LoginViewModel model)
    {
        ///TODO: check remember password to redirect to default
        return View(model);
    }

    public async Task Logout()
    {
        HttpContext.Response.Cookies.Delete("UserName");
        HttpContext.Response.Cookies.Delete("EmployeeId");
        HttpContext.Response.Cookies.Delete("EmployeeName");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Response.Redirect("Login");
    }

    public async Task<ActionResult> LoginSubmit(LoginViewModel model)
    {
        var response = new CommonResponseModel<LoginViewModel>();

        if(ModelState.IsValid)
        {
            var client = httpClientFactory.CreateClient();
            var remoteResponse  = client
                .ExecutePost<CommonResponseModel<UserLoginDataTransferObject>>
                (backEndConfig.HostHeplDesk + backEndConfig.Login, model);

            if(remoteResponse.Result.StatusCode == 
                System.Net.HttpStatusCode.OK)
            {
                var remoteData = remoteResponse.Result.Data;

                if(remoteData != null)
                {
                    if(remoteData.Data != null)
                    {
                        //get userId
                        var userId = "";
                        if (!string.IsNullOrEmpty(remoteData.Data.UserName))
                        {
                            var client1 = httpClientFactory.CreateClient();
                            var url = backEndConfig.Host + backEndConfig.GetUserByUserName;
                            url += "?userName=" + remoteData.Data.UserName;

                            var remoteResponse1 = client
                                    .ExecuteGet<CommonResponseModel<UserDetailDataTransferObject>>(url);

                            if (remoteResponse.Result.StatusCode ==
                            System.Net.HttpStatusCode.OK)
                            {
                                var remoteData1 = remoteResponse1.Result.Data;

                                if (remoteData1 != null)
                                {
                                    if (remoteData1.Success)
                                    {
                                        userId = remoteData1.Data!.Id;
                                    }
                                }
                            }
                        }
                        //get role
                        var navigationPermission = new List<NavigationPermissionModel>();
                        var client2 = httpClientFactory.CreateClient();
                        var url2 = backEndConfig.Host + backEndConfig.GetNavigationPermission;
                        url2 += "?userId=" + userId;

                        var remoteResponse2 = client2
                                .ExecuteGet<CommonResponseModel<List<NavigationPermissionModel>>>(url2);

                        if (remoteResponse2.Result.StatusCode ==
                        System.Net.HttpStatusCode.OK)
                        {
                            var remoteData2 = remoteResponse2.Result.Data;
                            if (remoteData2 != null)
                            {
                                if (remoteData2.Success)
                                {
                                    navigationPermission = remoteData2.Data;
                                }
                            }
                        }
                        bool allow_Kanban = false;
                        foreach(var navigationPermissionModel in navigationPermission)
                        {
                            if(navigationPermissionModel.Role.Name=="KANBAN")
                            {
                                allow_Kanban = true;
                                break;
                            }    
                        }    
                        //
                        if (remoteData.Data!.isAdmin || allow_Kanban)
                        {
                            if (remoteData.Success)
                            {
                                response.Success = true;
                                remoteData.Data!.ExpiredDate = DateTime.Now.AddMinutes(30);

                                var claims = new List<Claim>()
                        {
                            new Claim("UserName", remoteData.Data!.UserName),
                            new Claim("AccessToken", remoteData.Data!.AccessToken),
                            new Claim("RefreshToken", remoteData.Data!.RefreshToken),
                            new Claim("IsAuthenticate", "true"),
                            new Claim("IsRememberPassword", model.RemmemberMe.ToString()),
                            new Claim("ExpiredDate", remoteData.Data!.ExpiredDate.ToString("yyyy-MM-dd HH:mm:ss")),
                        };
                                var claimIdentites = new List<ClaimsIdentity>()
                        {
                            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme),
                            new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme)
                        };

                                var claimPrincipal = new ClaimsPrincipal(claimIdentites);
                                HttpContext.Response.Cookies.Append("UserName", remoteData.Data!.UserName);
                                HttpContext.Response.Cookies.Append("EmployeeId", remoteData.Data!.EmployeeId);
                                HttpContext.Response.Cookies.Append("EmployeeName", remoteData.Data!.FullName);

                                await HttpContext.SignInAsync(claimPrincipal);

                                model.RedirectUrl = string.IsNullOrEmpty(model.ReturnUrl) ?
                                    authenConfig.DefaultRedirect : model.ReturnUrl;

                                response.Success = true;
                                response.Data = model;
                            }
                            else
                            {
                                response.Message = remoteData.Message;
                            }
                        }
                        else
                        {
                            response.Message = "Bạn không có quyền truy cập!";//errorConfig.GetByKey("InvalidUserRole");
                        }
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
            var error = string.Join(";", ModelState.ErrorCount.ToString());
            response.Message = error;
        }

        return Json(response);
    }
}
