using LS_ERP.Mobile.Models;
using LS_ERP.Mobile.Services;
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

namespace LS_ERP.Mobile.ViewModels
{
	public class ScanForSearchLocationInSupperMarketPageViewModel : ViewModelBase
    {
        public ScanForSearchLocationInSupperMarketPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService,pageDialogService)
        {
            Title = "Siêu thị tìm hàng";           
            StartScanCommand = new DelegateCommand(async () => await StartScanCommandHandler());          
            Cards = new ObservableCollection<CuttingCardModel>();
        }

        #region properties
        private ObservableCollection<CuttingCardModel> cards;
        public ObservableCollection<CuttingCardModel> Cards
        {
            get => cards;
            set
            {
                SetProperty(ref cards, value);
                RaisePropertyChanged(nameof(Cards));
            }
        }
        private string qrCode;
        public string QRCode
        {
            get => qrCode;
            set
            {
                SetProperty(ref qrCode, value);
                RaisePropertyChanged(nameof(QRCode));
                if (!string.IsNullOrEmpty(QRCode))
                {
                    if (QRCode.Length == 12)
                    {
                        IsBusy = true;
                        getCuttingCard(QRCode).Await();
                        IsBusy = false;
                    }
                }
            }
        }
        public async Task getCuttingCard(string id)
        {
            Cards.Clear();
            var cuttingCardService = new CuttingCardService();
            var cuttingCardResult = await cuttingCardService.GetLocationCuttingCard(id);
            if (cuttingCardResult != null)
            {
                if (cuttingCardResult.Success)
                {
                    if (cuttingCardResult.Data.Count > 0)
                    {
                        foreach (var item in cuttingCardResult.Data)
                        {
                            var newCard = new CuttingCardModel();
                            newCard = item;
                            Cards.Add(newCard);
                        }
                        QRCode = "";
                    }
                    else
                    {
                        await PageDialogService.DisplayAlertAsync("Thông báo", "Mã Qrcode không tồn tại!", "OK");
                    }
                }
            }
            else
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Mã Qrcode không tồn tại!", "OK");
            }    
            await Task.CompletedTask;
        }
        #endregion

        #region command
        public DelegateCommand StartScanCommand { get; private set; }
        public async Task StartScanCommandHandler()
        {
            //IsBusy = true;
            try
            {
                var QrScanner = new QrScanningService();
                var result = await QrScanner.ScanAsync();
                if (result != null)
                {
                    Cards.Clear();
                    var cuttingCardID = result;
                    var cuttingCardService = new CuttingCardService();
                    var cuttingCardResult = await cuttingCardService.GetLocationCuttingCard(cuttingCardID);
                    if (cuttingCardResult != null)
                    {
                        if (cuttingCardResult.Success)
                        {
                            if (cuttingCardResult.Data.Count > 0)
                            {
                                foreach(var item in cuttingCardResult.Data)
                                {
                                    var newCard = new CuttingCardModel();
                                    newCard = item;
                                    Cards.Add(newCard);
                                }                               
                            }
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //IsBusy = false;
            await Task.CompletedTask;
        }
        #endregion 
    }
}
