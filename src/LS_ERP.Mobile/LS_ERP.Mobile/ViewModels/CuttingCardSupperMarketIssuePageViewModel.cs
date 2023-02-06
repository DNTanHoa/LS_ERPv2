using LS_ERP.Mobile.Models;
using LS_ERP.Mobile.Services.Implement;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LS_ERP.Mobile.ViewModels
{
    public class CuttingCardSupperMarketIssuePageViewModel : ViewModelBase
    {
        public CuttingCardSupperMarketIssuePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Title = "Xuất hàng";
            SaveChangeCommand = new DelegateCommand(async () => await SaveChangeCommandHandler());
        }
        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);            
            var cuttingCardID = parameters.GetValue<string>("cuttingCardID");
            var userName = Preferences.Get("UserName", null);
            var cuttingCardService = new CuttingCardService();
            var result = await cuttingCardService.GetCuttingCard(cuttingCardID);
            if(result!=null)
            {
                if (result.Success)
                {
                    if (result.Data.Count > 0)
                    {
                        currentCuttingCard = result.Data[0];
                        currentCuttingCard.UserName = userName;

                        id = currentCuttingCard.ID;
                        Id = id;
                        fabricContrastDescription = FabricContrastDescription = currentCuttingCard.FabricContrastDescription;
                        cardType = CardType = currentCuttingCard.CardType;
                        lot = Lot = currentCuttingCard.Lot;
                        set = Set = currentCuttingCard.Set;
                        mergeBlockLSStyle = MergeBlockLSStyle = currentCuttingCard.MergeBlockLSStyle;
                        mergeLSStyle = MergeLSStyle = currentCuttingCard.MergeLSStyle;
                        size = Size = currentCuttingCard.Size;
                        fabricContrastColor = FabricContrastColor = currentCuttingCard.FabricContrastColor;
                        allocQuantity = AllocQuantity = (int)currentCuttingCard.AllocQuantity;
                        totalPackage = TotalPackage = currentCuttingCard.TotalPackage;
                        workCenterName = WorkCenterName = currentCuttingCard.WorkCenterName;
                        tableNO = TableNO = currentCuttingCard.TableNO;
                        produceDate = ProduceDate = currentCuttingCard.ProduceDate.ToString("dd/MM/yyyy");
                        isCompling = IsCompling = currentCuttingCard.IsCompling;
                    }
                }
            }   
        }

        #region properties
        public CuttingCardModel currentCuttingCard { get; set; }

        private string id;
        public string Id
        {
            get => id;
            set
            {
                SetProperty(ref id, value);
                RaisePropertyChanged(nameof(Id));
            }
        }
        private bool isCompling;
        public bool IsCompling
        {
            get => isCompling;
            set
            {
                SetProperty(ref isCompling, value);
                RaisePropertyChanged(nameof(IsCompling));
            }
        }
        private string fabricContrastDescription;
        public string FabricContrastDescription
        {
            get => fabricContrastDescription;
            set
            {
                SetProperty(ref fabricContrastDescription, value);
                RaisePropertyChanged(nameof(FabricContrastDescription));
            }
        }
        private string cardType;
        public string CardType
        {
            get => cardType;
            set
            {
                SetProperty(ref cardType, value);
                RaisePropertyChanged(nameof(CardType));
            }
        }
        private string lot;
        public string Lot
        {
            get =>lot;
            set
            {
                SetProperty(ref lot, value);
                RaisePropertyChanged(nameof(Lot));
            }
        }
        private string mergeBlockLSStyle;
        public string MergeBlockLSStyle
        {
            get => mergeBlockLSStyle;
            set
            {
                SetProperty(ref mergeBlockLSStyle, value);
                RaisePropertyChanged(nameof(MergeBlockLSStyle));
            }
        }
        private string mergeLSStyle;
        public string MergeLSStyle
        {
            get => mergeLSStyle;
            set
            {
                SetProperty(ref mergeLSStyle, value);
                RaisePropertyChanged(nameof(MergeLSStyle));
            }
        }
        private string size;
        public string Size
        {
            get => size;
            set
            {
                SetProperty(ref size, value);
                RaisePropertyChanged(nameof(Size));
            }
        }
        private string set;
        public string Set
        {
            get => set;
            set
            {
                SetProperty(ref set, value);
                RaisePropertyChanged(nameof(Set));
            }
        }
        private string fabricContrastColor;
        public string FabricContrastColor
        {
            get => fabricContrastColor;
            set
            {
                SetProperty(ref fabricContrastColor, value);
                RaisePropertyChanged(nameof(FabricContrastColor));
            }
        }
        private int  allocQuantity;
        public int AllocQuantity
        {
            get => allocQuantity;
            set
            {
                SetProperty(ref allocQuantity, value);
                RaisePropertyChanged(nameof(AllocQuantity));
            }
        }
        private int  totalPackage;
        public int TotalPackage
        {
            get => totalPackage;
            set
            {
                SetProperty(ref totalPackage, value);
                RaisePropertyChanged(nameof(TotalPackage));
            }
        }
        private string workCenterName;
        public string WorkCenterName
        {
            get => workCenterName;
            set
            {
                SetProperty(ref workCenterName, value);
                RaisePropertyChanged(nameof(WorkCenterName));
            }
        }
        private int tableNO;
        public int TableNO
        {
            get => tableNO;
            set
            {
                SetProperty(ref tableNO, value);
                RaisePropertyChanged(nameof(TableNO));
            }
        }
        private string produceDate;
        public string   ProduceDate
        {
            get => produceDate;
            set
            {
                SetProperty(ref produceDate, value);
                RaisePropertyChanged(nameof(ProduceDate));
            }
        }
        
       


        #endregion

        #region commands
        public DelegateCommand SaveChangeCommand { get; private set; }
        public async Task SaveChangeCommandHandler()
        {
            if(string.IsNullOrEmpty(Id))
            {
                await PageDialogService.DisplayAlertAsync("Lỗi", "Thẻ bài không tồn tại, trở về và quét mã QRcode!", "OK");
                return;
            }    
            IsBusy = true;           
            
            var cuttingCardService = new CuttingCardService();
            currentCuttingCard.IsCompling = IsCompling;
            currentCuttingCard.UserName = Preferences.Get("UserName", null);
            var result = await cuttingCardService.UpdateCuttingCard(currentCuttingCard);
            if(result.Success)
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Xác nhận phối thành công!", "OK");
            }    
            else
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Lỗi", "OK");
            }    
            await Task.CompletedTask;
            IsBusy = false;
        }
        

        #endregion

        #region support function only in this view models

        #endregion
    }
}
