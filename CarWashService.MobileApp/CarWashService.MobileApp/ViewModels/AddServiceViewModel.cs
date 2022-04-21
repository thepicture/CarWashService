using CarWashService.MobileApp.Models.Serialized;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddServiceViewModel : BaseViewModel
    {
        public SerializedService CurrentService { get; set; }

        private Command saveChangesCommand;

        public ICommand SaveChangesCommand
        {
            get
            {
                if (saveChangesCommand == null)
                {
                    saveChangesCommand = new Command(SaveChangesAsync);
                }

                return saveChangesCommand;
            }
        }

        private async void SaveChangesAsync()
        {

            CurrentService.ServiceTypes = new List<string>
            {
                CurrentType
            };
            CurrentService.PriceString = PriceString;
            if (await ServiceDataStore.AddItemAsync(CurrentService))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private string priceString;

        public string PriceString
        {
            get => priceString;
            set => SetProperty(ref priceString, value);
        }

        private string currentType;

        public AddServiceViewModel(SerializedService inputService = null)
        {
            if (inputService != null)
            {
                CurrentService = inputService;
                PriceString = CurrentService.Price.ToString("F0");
                CurrentType = CurrentService.ServiceTypes.First();
            }
            else
            {
                CurrentService = new SerializedService();
            }
        }

        public string CurrentType
        {
            get => currentType;
            set => SetProperty(ref currentType, value);
        }

        private Command deleteServiceCommand;

        public ICommand DeleteServiceCommand
        {
            get
            {
                if (deleteServiceCommand == null)
                {
                    deleteServiceCommand = new Command(DeleteServiceAsync);
                }

                return deleteServiceCommand;
            }
        }

        private async void DeleteServiceAsync()
        {
            if (await ServiceDataStore
                .DeleteItemAsync(CurrentService
                .Id
                .ToString()))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        public bool IsCanDeleteService => CurrentService.Id > 0;
    }
}