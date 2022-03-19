using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    _ = LoadServicesAsync();
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
            _ = LoadServicesAsync();
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
    }
}