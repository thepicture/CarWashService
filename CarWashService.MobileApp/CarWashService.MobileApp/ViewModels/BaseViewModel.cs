using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public IDataStore<byte[]> UserImageDataStore =>
            DependencyService.Get<IDataStore<byte[]>>();
        public ICaptchaService CaptchaService =>
            DependencyService.Get<ICaptchaService>();
        public string Role => AppIdentity.Role;
        public bool IsCanDelete => "Администратор, Сотрудник".Contains(Role);

        bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }
        private bool isRefreshing = false;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set => SetProperty(ref isRefreshing, value);
        }

        string title = string.Empty;

        public string Title
        {
            get { return title; }
            set { _ = SetProperty(ref title, value); }
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
