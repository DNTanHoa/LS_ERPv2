using LS_ERP.Mobile.Services.Implement;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LS_ERP.Mobile.ViewModels
{
    public class CuttingCardComplingScanPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public CuttingCardComplingScanPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService,pageDialogService)
        {
            Title = "Scanner";
            ViewCuttingCardCommand = new DelegateCommand(async () => await ViewCuttingCardCommandHandler());
            
        }
        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            UserName = Preferences.Get("UserName", null);

        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            QRCode = string.Empty;
            
        }
        
        #region properties
        public string UserName { get; set; }
        private string qrCode;
        public string QRCode
        {
            get => qrCode;
            set
            {
                SetProperty(ref qrCode, value);
                RaisePropertyChanged(nameof(QRCode));
                if(!string.IsNullOrEmpty(QRCode))
                {
                    if(QRCode.Length==12)
                    {
                        var parameter = new NavigationParameters();
                        parameter.Add("cuttingCardID", qrCode);
                        parameter.Add("UserName", UserName);
                        NavigationService.NavigateAsync("CuttingCardComplingPage", parameter);
                    }    
                }    
            }
        }
       
        public DelegateCommand ViewCuttingCardCommand { get; }
        #endregion
        public async Task ViewCuttingCardCommandHandler()
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Vui lòng quét mã QRCode hoặc nhập ID", "OK");
                return;
            }
            IsBusy = true;   
            var parameter = new NavigationParameters();
            parameter.Add("cuttingCardID", qrCode);           
            await NavigationService.NavigateAsync("CuttingCardComplingPage", parameter);
            
            IsBusy = false;
        }
       
    }
}
