using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IPageDialogService
    {
        protected INavigationService NavigationService { get; private set; }
        protected IPageDialogService PageDialogService { get; private set; }

        #region Properties
        private string title;
        public string Title
        {
            get => title;
            set { SetProperty(ref title, value); }
        }
       
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set { SetProperty(ref isBusy, value); }
        }
        #endregion

        public ViewModelBase(INavigationService navigationService,IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton, FlowDirection flowDirection)
        {
            throw new NotImplementedException();
        }

        public Task DisplayAlertAsync(string title, string message, string cancelButton)
        {
            throw new NotImplementedException();
        }

        public Task DisplayAlertAsync(string title, string message, string cancelButton, FlowDirection flowDirection)
        {
            throw new NotImplementedException();
        }

        public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            throw new NotImplementedException();
        }

        public Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, FlowDirection flowDirection, params string[] otherButtons)
        {
            throw new NotImplementedException();
        }

        public Task DisplayActionSheetAsync(string title, params IActionSheetButton[] buttons)
        {
            throw new NotImplementedException();
        }

        public Task DisplayActionSheetAsync(string title, FlowDirection flowDirection, params IActionSheetButton[] buttons)
        {
            throw new NotImplementedException();
        }

        public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, KeyboardType keyboardType = KeyboardType.Default, string initialValue = "")
        {
            throw new NotImplementedException();
        }

        
    }
}
