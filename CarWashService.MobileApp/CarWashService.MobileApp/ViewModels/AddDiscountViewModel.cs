using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddDiscountViewModel : BaseViewModel
    {

        private SerializedDiscount currentService;

        public AddDiscountViewModel(SerializedDiscount inputDiscount = null)
        {
            if (inputDiscount == null)
            {
                CurrentDiscount = new SerializedDiscount();
            }
            else
            {
                CurrentDiscount = inputDiscount;
                DiscountPercent = CurrentDiscount.DiscountPercent.ToString();
                WorkFrom = CurrentDiscount.WorkFromAsDate;
                WorkTo = CurrentDiscount.WorkToAsDate;
            }
        }

        public SerializedDiscount CurrentDiscount
        {
            get => currentService;
            set => SetProperty(ref currentService, value);
        }

        private string discountPercent;

        public string DiscountPercent
        {
            get => discountPercent;
            set => SetProperty(ref discountPercent, value);
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
            CurrentDiscount.DiscountPercentAsString = DiscountPercent;
            if (WorkFrom == DateTime.MinValue)
            {
                CurrentDiscount.WorkFrom = DateTime.Now.ToString();
            }
            else
            {
                CurrentDiscount.WorkFrom = WorkFrom.ToString();
            }
            if (WorkTo == DateTime.MinValue)
            {
                CurrentDiscount.WorkFrom = DateTime.Now.ToString();
            }
            else
            {
                CurrentDiscount.WorkTo = WorkTo.ToString();
            }
            CurrentDiscount.ServiceId = App.CurrentService.Id;
            if (await DiscountDataStore.AddItemAsync(CurrentDiscount))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private DateTime workFrom = DateTime.Now;

        public DateTime WorkFrom
        {
            get => workFrom;
            set => SetProperty(ref workFrom, value);
        }

        private DateTime workTo = DateTime.Now.AddDays(1);

        public DateTime WorkTo
        {
            get => workTo;
            set => SetProperty(ref workTo, value);
        }


        public bool IsCanDeleteDiscount => CurrentDiscount.Id != 0;
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