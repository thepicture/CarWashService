using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.ViewModels;
using CarWashService.MobileApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public class ServicesViewModel : BaseViewModel
    {
        public bool IsClientCheckingForOrder => IsForOrder && !IsCanDelete;


        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    LoadServicesAsync();
                }
            }
        }

        private ObservableCollection<SerializedService> services;

        public ObservableCollection<SerializedService> Services
        {
            get => services;
            set => SetProperty(ref services, value);
        }

        internal void OnAppearing()
        {
            SelectedServices = new ObservableCollection<SerializedService>();
            Services = new ObservableCollection<SerializedService>();
            LoadServicesAsync();
        }

        private async Task LoadServicesAsync()
        {
            Services.Clear();
            SelectedServices.Clear();
            IEnumerable<SerializedService> currentServices =
                await ServiceDataStore
                    .GetItemsAsync();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                currentServices = currentServices
                    .Where(s =>
                    {
                        return s.Name.ToLower()
                        .Contains(
                            SearchText.ToLower());
                    });
            }
            foreach (SerializedService service in currentServices)
            {
                await Task.Delay(200);
                Services.Add(service);
            }
        }

        private Command goToAddServicePage;

        public ICommand GoToAddServicePage
        {
            get
            {
                if (goToAddServicePage == null)
                {
                    goToAddServicePage = new Command(PerformGoToAddServicePageAsync);
                }

                return goToAddServicePage;
            }
        }

        private async void PerformGoToAddServicePageAsync()
        {
            await Shell.Current.Navigation.PushAsync(
                new AddServicePage(
                    new AddServiceViewModel()));
        }


        private Command goToDiscountsCommand;

        public ICommand GoToDiscountsCommand
        {
            get
            {
                if (goToDiscountsCommand == null)
                {
                    goToDiscountsCommand = new Command(GoToDiscountsAsync);
                }

                return goToDiscountsCommand;
            }
        }

        private async void GoToDiscountsAsync(object parameter)
        {
            (App.Current as App).CurrentService =
                parameter as SerializedService;
            await Shell.Current.GoToAsync(
                $"{nameof(ServiceDiscountsPage)}");
        }

        private Command goToMakeOrderCommand;

        public ICommand GoToMakeOrderCommand
        {
            get
            {
                if (goToMakeOrderCommand == null)
                {
                    goToMakeOrderCommand = new Command(GoToMakeOrderAsync);
                }

                return goToMakeOrderCommand;
            }
        }

        private async void GoToMakeOrderAsync()
        {
            (App.Current as App).CurrentOrder = null;
            (App.Current as App).CurrentServices = SelectedServices;
            await Shell.Current.GoToAsync($"{nameof(MakeOrderPage)}");
        }
        public bool IsAbleToMakeOrder
        {
            get => isAbleToMakeOrder;
            set => SetProperty(ref isAbleToMakeOrder, value);
        }

        public ObservableCollection<SerializedService> SelectedServices
        {
            get => selectedServices;
            set => SetProperty(ref selectedServices, value);
        }

        private Command toggleServiceCommand;
        private ObservableCollection<SerializedService> selectedServices;
        private bool isAbleToMakeOrder;

        public ServicesViewModel(bool isForOrder)
        {
            IsForOrder = isForOrder;
        }

        public ICommand ToggleServiceCommand
        {
            get
            {
                if (toggleServiceCommand == null)
                {
                    toggleServiceCommand = new Command(ToggleService);
                }

                return toggleServiceCommand;
            }
        }

        public bool IsForOrder { get; }

        private void ToggleService(object parameter)
        {
            if (SelectedServices.Contains(parameter as SerializedService))
            {
                _ = SelectedServices.Remove(parameter as SerializedService);
            }
            else
            {
                SelectedServices.Add(parameter as SerializedService);
            }
            IsAbleToMakeOrder = SelectedServices.Count > 0 && IsClientCheckingForOrder;
        }

        private Command<SerializedService> goToEditDiscountPage;

        public Command<SerializedService> GoToEditDiscountPage
        {
            get
            {
                if (goToEditDiscountPage == null)
                {
                    goToEditDiscountPage = new Command<SerializedService>(PerformGoToEditDiscountPageAsync);
                }

                return goToEditDiscountPage;
            }
        }

        private async void PerformGoToEditDiscountPageAsync(SerializedService service)
        {
            await Shell.Current.Navigation.PushAsync(
                new AddServicePage(
                    new AddServiceViewModel(service)));
        }
    }
}