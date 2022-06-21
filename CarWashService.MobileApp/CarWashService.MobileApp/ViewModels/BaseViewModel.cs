using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<SerializedBranch> BranchDataStore =>
            DependencyService.Get<IDataStore<SerializedBranch>>();
        public IFeedbackService FeedbackService =>
            DependencyService.Get<IFeedbackService>();
        public IDataStore<SerializedService> ServiceDataStore =>
            DependencyService.Get<IDataStore<SerializedService>>();
        public IDataStore<SerializedDiscount> DiscountDataStore =>
            DependencyService.Get<IDataStore<SerializedDiscount>>();
        public IDataStore<SerializedOrder> OrderDataStore =>
            DependencyService.Get<IDataStore<SerializedOrder>>();
        public IDataStore<SerializedRegistrationUser> RegistrationDataStore =>
            DependencyService.Get<IDataStore<SerializedRegistrationUser>>();
        public IDataStore<SerializedLoginUser> LoginDataStore =>
            DependencyService.Get<IDataStore<SerializedLoginUser>>();
        public IDataStore<IEnumerable<SerializedService>> OrderServicesDataStore =>
            DependencyService.Get<IDataStore<IEnumerable<SerializedService>>>();
        public IDataStore<byte[]> UserImageDataStore =>
            DependencyService.Get<IDataStore<byte[]>>();
        public ICaptchaService CaptchaService =>
            DependencyService.Get<ICaptchaService>();
        public IImageResizer ImageResizer =>
            DependencyService.Get<IImageResizer>();
        public bool IsCanDelete => "Администратор"
            .Contains(AppIdentity.User.UserTypeName);

        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (SetProperty(ref isBusy, value))
                {
                    OnPropertyChanged(
                        nameof(IsNotBusy));
                }
            }
        }
        public bool IsNotBusy => !IsBusy;
        private bool isRefreshing = false;
        public virtual bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        string title = string.Empty;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private Command exitCommand;

        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                    exitCommand = new Command(ExitAsync);

                return exitCommand;
            }
        }

        private async void ExitAsync()
        {
            if (await FeedbackService.Ask("Выйти из приложения?"))
            {
                System.Environment.Exit(0);
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
