using Common;
using LS_ERP.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Interface
{
    public interface IUserService
    {
        Task<CommonResponseModel<UserLoginDataTransferObject>> Login(LoginModel loginModel);
        Task<List<NavigationPermissionModel>> getNavigationPermission(string userName);
        Task<ObservableCollection<NavigationPermissionRoleModel>> getNavigationPermissionRole(string userName);

    }
}
