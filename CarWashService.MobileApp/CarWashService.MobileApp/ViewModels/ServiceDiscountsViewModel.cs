using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class ServiceDiscountsViewModel : BaseViewModel
    {
        internal void OnAppearing()
        {
            LoadDiscounts();
        }

        private async void LoadDiscounts()
        {
            IEnumerable<SerializedDiscount> currentDiscounts =
                await DiscountDataStore.GetItemsAsync();
            currentDiscounts = currentDiscounts.Where(d =>
            {
                return d.ServiceId == serviceId;
            });
            Discounts.Clear();
            foreach (SerializedDiscount discount in currentDiscounts)
            {
                Discounts.Add(discount);
            }
        }

        private ObservableCollection<SerializedDiscount> discounts;

        public ObservableCollection<SerializedDiscount> Discounts
        {
            get => discounts;
            set => SetProperty(ref discounts, value);
        }

        private Command goToAddDiscountPage;

        public ICommand GoToAddDiscountPage
        {
            get
            {
                if (goToAddDiscountPage == null)
                {
                    goToAddDiscountPage = new Command(PerformGoToAddDiscountPageAsync);
                }

                return goToAddDiscountPage;
            }
        }

        private async void PerformGoToAddDiscountPageAsync()
        {
            await Shell.Current.Navigation.PushAsync(
                new AddDiscountPage(
                    new AddDiscountViewModel(ServiceId)));
        }

        private Command<SerializedDiscount> deleteDiscountCommand;

        public Command<SerializedDiscount> DeleteDiscountCommand
        {
            get
            {
                if (deleteDiscountCommand == null)
                {
                    deleteDiscountCommand = new Command<SerializedDiscount>
                        (DeleteDiscountAsync);
                }

                return deleteDiscountCommand;
            }
        }

        private async void DeleteDiscountAsync(SerializedDiscount discount)
        {
            if (await DiscountDataStore
                     .DeleteItemAsync(discount.Id
                     .ToString()))
            {
                LoadDiscounts();
            }
        }

        private Command<SerializedDiscount> goToDiscountPageCommand;
        private int serviceId;

        public ServiceDiscountsViewModel(int serviceId)
        {
            Discounts = new ObservableCollection<SerializedDiscount>();
            ServiceId = serviceId;
        }

        public Command<SerializedDiscount> GoToDiscountPageCommand
        {
            get
            {
                if (goToDiscountPageCommand == null)
                {
                    goToDiscountPageCommand =
                        new Command<SerializedDiscount>(GoToDiscountPageAsync);
                }

                return goToDiscountPageCommand;
            }
        }

        public int ServiceId
        {
            get => serviceId;
            set => SetProperty(ref serviceId, value);
        }

        private async void GoToDiscountPageAsync(SerializedDiscount discount)
        {
            await Shell.Current.Navigation.PushAsync(
                new AddDiscountPage(
                    new AddDiscountViewModel(ServiceId, discount)));
        }
    }
}