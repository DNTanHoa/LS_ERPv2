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
using Xamarin.Forms;

namespace LS_ERP.Mobile.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        public  HomePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService,pageDialogService)
        {
            Title = "Trang chủ";
            NavigateCommand = new DelegateCommand<string>(async(route) => await NavigateCommandHandler(route));
            Functions = new ObservableCollection<NavigationPermissionRoleModel>
            {
                //new NavigationPermissionRoleModel{ Description = "Xác nhận phối hàng",Path ="CuttingCardComplingScanPage", Image ="qr.png" },
                //new NavigationPermissionRoleModel{ Description = "Cập nhật vị trí",Path ="CuttingCardScanPage",Image ="qr.png" },
                new NavigationPermissionRoleModel{ Description = "Xác nhận phối hàng",Path ="ScanForComplingPage", Image ="compling.png" },
                new NavigationPermissionRoleModel{ Description = "Siêu thị nhập hàng",Path ="ScanForReceiveToSupperMarketPage",Image ="receive.png" },
                new NavigationPermissionRoleModel{ Description = "Siêu thị tìm hàng",Path ="ScanForSearchLocationInSupperMarketPage",Image ="search.png" },
                new NavigationPermissionRoleModel{ Description = "Quét giao hàng in",Path ="ScanForDeliveryNotePage",Image ="note.png" },
                new NavigationPermissionRoleModel{ Description = "Quét nhận hàng in",Path ="ScanForReceiveFromExternalPrintPage",Image ="receive_print.png" }
            };

            //GetNavigationPermissionRole().Await();
            

        }
        public override async void Initialize(INavigationParameters parameters)
        {
           
            base.Initialize(parameters);            
            UserName = Preferences.Get("UserName", null);  
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);          
        }

        #region properties
        public string UserName { get; set; }
        
       

        private ObservableCollection<NavigationPermissionRoleModel> functions;
        public ObservableCollection<NavigationPermissionRoleModel> Functions
        {
            get => functions;
            set
            {
                functions = value;
                RaisePropertyChanged(nameof(Functions));
            }
        }
        #endregion

        #region commands
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public async Task NavigateCommandHandler(string route)
        {
            if (!IsBusy)
            {
                IsBusy = true;  
                var userService = new UserService();
                var navigationPermissionRoles = await userService.getNavigationPermissionRole(UserName);
                bool permission = false;
                foreach(var item in navigationPermissionRoles)
                {
                    if(item.Path == route)
                    {
                        permission = true;break;
                        
                    }    
                }    
                if(permission)
                {
                    await NavigationService.NavigateAsync(route);
                    IsBusy = false;
                }   
                else
                {
                    await PageDialogService.DisplayAlertAsync("Thông báo", "Bạn chưa được phân quyền!", "OK");
                    IsBusy = false;
                }    
                
            }

            await Task.CompletedTask;
        }
        //public DelegateCommand GetNavigateCommand { get; private set; }
        public  async Task GetNavigationPermissionRole()
        {
            if(!IsBusy)
            {
                IsBusy=true;
                var parameter = new NavigationParameters();
                parameter.Add("UserName", UserName);
                var navigationPermissionRoles = new ObservableCollection<NavigationPermissionRoleModel>();
                UserName = Preferences.Get("UserName",null);
                var userService = new UserService();
                Functions = new ObservableCollection<NavigationPermissionRoleModel>(navigationPermissionRoles);
                Functions =  await  userService.getNavigationPermissionRole(UserName);
                //Functions = new ObservableCollection<NavigationPermissionRoleModel>( navigationPermissionRoles);
                //Functions = new ObservableCollection<NavigationPermissionRoleModel>();
                
                //Functions = new ObservableCollection<NavigationPermissionRoleModel>
                //{                   
                //    new NavigationPermissionRoleModel{ Description = "Xác nhận phối hàng",Path ="CuttingCardComplingScanPage" },
                //    new NavigationPermissionRoleModel{ Description = "Cập nhật vị trí",Path ="CuttingCardScanPage" }
                //};
                IsBusy = false;
            }
            await Task.CompletedTask;
        }
        #endregion
    }
}
