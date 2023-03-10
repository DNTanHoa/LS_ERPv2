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
using Xamarin.Essentials;

namespace LS_ERP.Mobile.ViewModels
{
    public class ScanForDeliveryNotePageViewModel : ViewModelBase
    {
        public ScanForDeliveryNotePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Title = "Giao hàng in";
            SaveChangeCommand = new DelegateCommand(async () => await SaveChangeCommandHandler());
            StartScanCommand = new DelegateCommand(async () => await StartScanCommandHandler());
            DeleteCommand = new DelegateCommand(async () => await DeleteCommandHandler());
            Cards = new ObservableCollection<CuttingCardModel>();
        }
        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            var deliveryNoteService = new DeliveryNoteService();
            var companyID = Preferences.Get("CompanyId", "");
            var type = "ExternalPrint";
            var reponse = await deliveryNoteService.GetDeliveryNote(companyID,type);            
            var deliveryNotes = reponse?.Data;
            DeliveryNotes = deliveryNotes;
        }
        #region properties
        private string totalCard;
        public string TotalCard
        {
            get => totalCard;
            set
            {
                SetProperty(ref totalCard, value);
                RaisePropertyChanged(nameof(TotalCard));
            }
        }
        private ObservableCollection<CuttingCardModel> cards;
        public ObservableCollection<CuttingCardModel> Cards
        {
            get => cards;
            set
            {
                SetProperty(ref cards, value);
                RaisePropertyChanged(nameof(Cards));
                TotalCard = "Đã quét: " + Cards.Count();
            }
        }
        private DeliveryNoteModel selectedDeliveryNote;
        public DeliveryNoteModel SelectedDeliveryNote
        {
            get => selectedDeliveryNote;
            set
            {
                SetProperty(ref selectedDeliveryNote, value);
                RaisePropertyChanged(nameof(SelectedDeliveryNote));
            }
        }
        private List<DeliveryNoteModel> deliveryNotes;
        public List<DeliveryNoteModel> DeliveryNotes
        {
            get => deliveryNotes;
            set
            {
                SetProperty(ref deliveryNotes, value);
                RaisePropertyChanged(nameof(DeliveryNotes));
            }
        }
        private CuttingCardModel currentCard;
        public CuttingCardModel CurrentCard
        {
            get => currentCard;
            set
            {
                SetProperty(ref currentCard, value);
                RaisePropertyChanged(nameof(CurrentCard));
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
        #endregion
        #region commands
        public DelegateCommand StartScanCommand { get; private set; }

        public async Task getCuttingCard(string id)
        {
            var cuttingCardService = new CuttingCardService();
            var cuttingCardResult = await cuttingCardService.GetCuttingCard(id);
            if (cuttingCardResult != null)
            {
                if (cuttingCardResult.Success)
                {
                    if (cuttingCardResult.Data.Count > 0)
                    {
                        var newCard = new CuttingCardModel();
                        newCard = cuttingCardResult.Data[0];
                        var newCards = Cards;
                        var existCard = Cards.Where(x => x.ID == newCard.ID).FirstOrDefault();
                        if (existCard == null)
                        {
                            newCards.Add(newCard);
                            Cards = newCards;
                            CurrentCard = newCard;
                            QRCode = "";
                        }
                        else
                        {
                            QRCode = "";
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }
        public async Task StartScanCommandHandler()
        {
            //IsBusy = true;
            try
            {
                var QrScanner = new QrScanningService();
                var result = await QrScanner.ScanAsync();
                if (result != null)
                {
                    var cuttingCardID = result;
                    var cuttingCardService = new CuttingCardService();
                    var cuttingCardResult = await cuttingCardService.GetCuttingCard(cuttingCardID);
                    if (cuttingCardResult != null)
                    {
                        if (cuttingCardResult.Success)
                        {
                            if (cuttingCardResult.Data.Count > 0)
                            {
                                var newCard = new CuttingCardModel();
                                newCard = cuttingCardResult.Data[0];
                                var newCards = Cards;
                                if(newCards != null)
                                {
                                    if(newCard.DeliveryNote == null)
                                    {
                                        var existCard = Cards.Where(x => x.ID == newCard.ID).FirstOrDefault();
                                        if (existCard == null)
                                        {
                                            newCards.Add(newCard);
                                            Cards = newCards;
                                            CurrentCard = newCard;
                                        }
                                    }   
                                    else
                                    {
                                        await PageDialogService.DisplayAlertAsync("Thông báo", "Thẻ bài này đã được quét cho phiếu: "+newCard.DeliveryNote.Code, "OK");
                                    }    
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
        public DelegateCommand SaveChangeCommand { get; private set; }
        public async Task SaveChangeCommandHandler()
        {

            IsBusy = true;
            var bulkCuttingCard = new BulkCuttingCardModel();
            bulkCuttingCard.UserName = Preferences.Get("UserName", null);
            if (Cards.Count > 0 && SelectedDeliveryNote != null)
            {
                bulkCuttingCard.Ids = Cards.Select(s => s.ID).ToList();
                bulkCuttingCard.DeliveryNote = SelectedDeliveryNote;
                bulkCuttingCard.CurrentOperation = "EXTERNALPRINT";
                var cuttingCardService = new CuttingCardService();
                var result = await cuttingCardService.UpdateBulkCuttingCard(bulkCuttingCard);
                if (result.Success)
                {
                    await PageDialogService.DisplayAlertAsync("Thông báo", "Lưu phiếu giao hàng thành công!", "OK");
                    Cards.Clear();
                    TotalCard = "Đã quét: " + Cards.Count();
                    SelectedDeliveryNote = null;
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Thông báo", "Lỗi", "OK");
                }
            }
            else if (SelectedDeliveryNote == null)
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Bạn chưa chọn phiếu giao hàng", "OK");
            }
            else if (Cards.Count == 0)
            {
                await PageDialogService.DisplayAlertAsync("Thông báo", "Bạn chưa quét thẻ bài", "OK");
            }
            await Task.CompletedTask;
            IsBusy = false;
        }
        public DelegateCommand DeleteCommand { get; private set; }
        public async Task DeleteCommandHandler()
        {
            IsBusy = true;
            if (Cards.Count > 0)
            {
                Cards.Remove(currentCard);
                TotalCard = "Đã quét: " + Cards.Count();
            }
            await Task.CompletedTask;
            IsBusy = false;
        }
        #endregion
    }
}
