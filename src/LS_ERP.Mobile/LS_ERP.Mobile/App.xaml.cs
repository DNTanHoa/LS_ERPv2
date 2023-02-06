using LS_ERP.Mobile.ViewModels;
using LS_ERP.Mobile.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace LS_ERP.Mobile
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
            
        }

        protected override async void OnInitialized()
        {
            DevExpress.XamarinForms.Editors.Initializer.Init();
            InitializeComponent();           
            await NavigationService.NavigateAsync("LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<CuttingCardScanPage, CuttingCardScanPageViewModel>();
            containerRegistry.RegisterForNavigation<CuttingCardComplingScanPage, CuttingCardComplingScanPageViewModel>();
            containerRegistry.RegisterForNavigation<CuttingCardPage, CuttingCardPageViewModel>();
            containerRegistry.RegisterForNavigation<CuttingCardComplingPage, CuttingCardComplingPageViewModel>();

            containerRegistry.RegisterForNavigation<ScanForReceiveToSupperMarketPage, ScanForReceiveToSupperMarketPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanForSearchLocationInSupperMarketPage, ScanForSearchLocationInSupperMarketPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanForComplingPage, ScanForComplingPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanForDeliveryNotePage, ScanForDeliveryNotePageViewModel>();
            containerRegistry.RegisterForNavigation<ScanForReceiveFromExternalPrintPage, ScanForReceiveFromExternalPrintPageViewModel>();
        }
    }
}
