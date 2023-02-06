using Common;
using LS_ERP.Mobile.Models;
using LS_ERP.Mobile.Services.Implement;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LS_ERP.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            LoginCommand = new DelegateCommand(async() => await LoginCommandHandler());
            VersionTracking.Track();
            Version = VersionTracking.CurrentVersion;            
            var properties = App.Current.Properties;
            if(properties.Count > 1)
            {
                properties.TryGetValue("UserName", out var remember_user);
                UserName = remember_user.ToString();
                properties.TryGetValue("PassWord", out var remember_password);
                PassWord = remember_password.ToString();
            }    
            
        }

        #region Properties
        private string userName;
        public string UserName
        {
            get => userName;
            set
            {
                SetProperty(ref userName, value);
                RaisePropertyChanged(nameof(UserName));
            }
        }
        
        private string passWord;
        public string PassWord
        {
            get => passWord;
            set
            {
                SetProperty(ref passWord, value);
                RaisePropertyChanged(nameof(PassWord));
            }
        }
        private string version;
        public string Version
        {
            get => version;
            set
            {
                SetProperty(ref version, value);
                RaisePropertyChanged(nameof(Version));
            }
        }

        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get; set; }
        public async Task LoginCommandHandler()
        {
            if(!IsBusy)
            {
                this.IsBusy = true;
                //Task.Delay(5000);
                var loginModel = new LoginModel();
                loginModel.UserName = userName;    
                loginModel.PassWord = passWord;  
                var userService = new UserService();
                var result = await userService.Login(loginModel);
                if(result != null)
                {
                    if(result.Success)
                    {
                       
                        var navigationPermissions = new List<NavigationPermissionModel>();
                        UserName = result.Data.UserName;
                        navigationPermissions = await userService.getNavigationPermission(UserName);

                        var employees = new CommonResponseModel<EmployeesDataTransferObject>();
                        employees = await userService.GetCompanyID(userName);
                        Preferences.Set("UserName", result.Data.UserName);
                        if (employees != null)
                        {
                            if (employees.Data != null)
                            {
                                Preferences.Set("CompanyId", employees.Data.CompanyId);
                            }
                        }
                        if (result.Data.isAdmin)
                        {                            
                            Preferences.Set("UserName",result.Data.UserName);
                            App.Current.Properties.Clear();
                            App.Current.Properties.Add("UserName", UserName);
                            App.Current.Properties.Add("PassWord", PassWord);
                            await App.Current.SavePropertiesAsync();
                            await NavigationService.NavigateAsync("HomePage");
                        }  
                        else if(navigationPermissions!=null)
                        {
                            if(navigationPermissions.Count>0)
                            {                      
                                
                                App.Current.Properties.Clear();
                                App.Current.Properties.Add("UserName", UserName);
                                App.Current.Properties.Add("PassWord", PassWord);
                                await App.Current.SavePropertiesAsync();
                                await NavigationService.NavigateAsync("HomePage");                                
                            }
                            else
                            {
                                await PageDialogService.DisplayAlertAsync("Login", "Bạn chưa được phân quyền!", "OK");
                                IsBusy = false;
                            }
                        }    
                        else
                        {
                            await PageDialogService.DisplayAlertAsync("Login", "Lỗi", "OK");
                            IsBusy = false;
                        }
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync("Login", "Lỗi", "OK");
                        IsBusy = false;
                    }
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Login", "Lỗi", "OK");
                    IsBusy = false;
                }
               // NavigationService.NavigateAsync("HomePage");
            }
            IsBusy = false;
            await Task.CompletedTask;
        }        
        #endregion
    }
}
