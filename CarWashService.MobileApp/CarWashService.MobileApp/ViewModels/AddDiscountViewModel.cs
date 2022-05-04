using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddDiscountViewModel : BaseViewModel
    {

        private SerializedDiscount currentService;

        public AddDiscountViewModel(int serviceId,
                                    SerializedDiscount inputDiscount = null)
        {
            if (inputDiscount == null)
            {
                CurrentDiscount = new SerializedDiscount
                {
                    WorkFrom = DateTime.Now.ToString(),
                    WorkTo = DateTime.Now
                    .AddDays(1)
                    .ToString()
                };
            }
            else
            {
                CurrentDiscount = inputDiscount;
                CurrentDiscount.DiscountPercentAsString =
                    CurrentDiscount.DiscountPercent.ToString();
            }
            CurrentDiscount.ServiceId = serviceId;
        }

        public SerializedDiscount CurrentDiscount
        {
            get => currentService;
            set => SetProperty(ref currentService, value);
        }

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
            if (await DiscountDataStore.AddItemAsync(CurrentDiscount))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        public bool IsCanDeleteDiscount => CurrentDiscount.Id != 0 && IsCanDelete;
        private Command deleteDiscountCommand;

        public ICommand DeleteDiscountCommand
        {
            get
            {
                if (deleteDiscountCommand == null)
                {
                    deleteDiscountCommand = new Command(DeleteDiscountAsync);
                }

                return deleteDiscountCommand;
            }
        }

        private async void DeleteDiscountAsync()
        {
            if (await DiscountDataStore
                .DeleteItemAsync(CurrentDiscount.Id
                .ToString()))
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}