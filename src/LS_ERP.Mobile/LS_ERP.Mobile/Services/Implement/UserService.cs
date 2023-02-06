using Common;
using LS_ERP.Mobile.Models;
using LS_ERP.Mobile.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Implement
{
    public class UserService : IUserService
    {
       
        public async Task<CommonResponseModel<UserLoginDataTransferObject>> Login(LoginModel loginModel)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostHelpDeskApi();
                    uri += "users/login";
                    var content = JsonConvert.SerializeObject(loginModel);
                    HttpContent contentPost = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(uri, contentPost);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<UserLoginDataTransferObject>();
                        //return JsonConvert.DeserializeObject<UserLoginDataTransferObject>(result);
                        result = JsonConvert.DeserializeObject<CommonResponseModel<UserLoginDataTransferObject>>(resultString);                       
                        return result;
                    }
                }
                catch (Exception exp)
                { 
                    // TODO LOG.
                    return null;

                }
            }
        }
        public async Task<CommonResponseModel<EmployeesDataTransferObject>> GetCompanyID(string userName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostHelpDeskApi();
                    uri += "employees/"+userName;                 
                   
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<EmployeesDataTransferObject>();                      
                        result = JsonConvert.DeserializeObject<CommonResponseModel<EmployeesDataTransferObject>>(resultString);
                        return result;
                    }
                }
                catch (Exception exp)
                {
                    // TODO LOG.
                    return null;

                }
            }
        }
        public async Task<List<NavigationPermissionModel>> getNavigationPermission(string userName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
              
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostHelpDeskApi() + "users/with_username?userName="+userName;                  
                    var response = await  httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<UserLoginDataTransferObject>();                       
                        result = JsonConvert.DeserializeObject<CommonResponseModel<UserLoginDataTransferObject>>(resultString);
                        if(result != null)
                        {
                            var uri1 = config.GetHostHelpDeskApi() + "navigationpermissions/with_userid?userId=" + result.Data.Id;
                            var reponse1 = await httpClient.GetAsync(uri1);
                            if(!reponse1.IsSuccessStatusCode)
                            {
                                throw new HttpRequestException();
                            }   
                            else
                            {
                                resultString = await reponse1.Content.ReadAsStringAsync();
                                var result1 = new CommonResponseModel<List<NavigationPermissionModel>>();
                                result1 = JsonConvert.DeserializeObject<CommonResponseModel<List<NavigationPermissionModel>>>(resultString);
                                return result1.Data;
                            }    
                        }  
                    }
                    return null;
                }
                catch (Exception exp)
                {
                    // TODO LOG.
                    return null;

                }
            }
        }
        public async  Task<ObservableCollection<NavigationPermissionRoleModel>> getNavigationPermissionRole(string userName)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();   
                    var navigationPermissionRoleModels = new ObservableCollection<NavigationPermissionRoleModel>();
                    var navigations = await getNavigationPermission(userName);
                    var navigationPermissions = navigations;
                    var navigationPermission = new NavigationPermissionModel(); 
                    for( int i =0; i< navigationPermissions.Count; i++ )
                    {
                        if( navigationPermissions[i].RoleId.Contains("LS_Tools"))
                        {
                            navigationPermission = navigationPermissions[i];
                            var uri1 = config.GetHostHelpDeskApi() + "navigationpermissionroles/with_roleid?roleId=" + navigationPermission.RoleId;
                            var reponse1 =  httpClient.GetAsync(uri1).Result;
                            if (!reponse1.IsSuccessStatusCode)
                            {
                                throw new HttpRequestException();
                            }
                            else
                            {
                                var resultString = await  reponse1.Content.ReadAsStringAsync();
                                var result1 = new CommonResponseModel<List<NavigationPermissionRoleModel>>();
                                result1 = JsonConvert.DeserializeObject<CommonResponseModel<List<NavigationPermissionRoleModel>>>(resultString);
                                foreach(var item in result1.Data)
                                {
                                    navigationPermissionRoleModels.Add(item);
                                } 
                            }
                        }    
                    }
                    return navigationPermissionRoleModels;
                    
                }
                catch (Exception exp)
                {
                    // TODO LOG.
                    return null;

                }
            }
        }
       
    }
}
