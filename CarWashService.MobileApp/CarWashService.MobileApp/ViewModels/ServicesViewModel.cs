using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
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

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    _ = Task.Run(() =>
                      {
                          return LoadServicesAsync();
                      });
                }
            }
        }

        private IEnumerable<SerializedService> services;

        public IEnumerable<SerializedService> Services
        {
            get => services;
            set => SetProperty(ref services, value);
        }

        internal void OnAppearing()
        {
            SelectedServices = new ObservableCollection<SerializedService>();
            _ = Task.Run(() => LoadServicesAsync());
        }

        private async Task LoadServicesAsync()
        {
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
            Services = currentServices;
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
            await Shell.Current.GoToAsync($"{nameof(AddServicePage)}");
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

        public ObservableCollection<SerializedService> SelectedServices { get; set; }

        private Command toggleServiceCommand;

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
        }
    }
}