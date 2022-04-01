using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Text;
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
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(DiscountPercent)
                || !int.TryParse(DiscountPercent, out int value)
                || value < 0
                || value > 100)
            {
                _ = validationErrors.AppendLine("Процент - " +
                    "это положительное целое число " +
                    "в диапазоне от 0 до 100.");
            }
            if (WorkFrom >= WorkTo)
            {
                _ = validationErrors.AppendLine("Дата окончания " +
                    "должна быть позднее даты начала.");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }
            CurrentDiscount.DiscountPercent = int.Parse(DiscountPercent);
            CurrentDiscount.WorkFrom = WorkFrom.ToString();
            CurrentDiscount.WorkTo = WorkTo.ToString();
            CurrentDiscount.ServiceId = (App.Current as App).CurrentService.Id;
            if (await DiscountDataStore.AddItemAsync(CurrentDiscount))
            {
                string action = CurrentDiscount.Id == 0 ? 
                    "добавлена" : 
                    "изменена";
                await FeedbackService.Inform($"Скидка {action}.");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await FeedbackService.Inform("Не удалось " +
                    "добавить скидку. Проверьте подключение к интернету.");
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
            if (await FeedbackService.Ask("Удалить скидку?"))
            {
                if (await DiscountDataStore
                    .DeleteItemAsync(CurrentDiscount.Id
                    .ToString()))
                {
                    await FeedbackService.Inform("Скидка удалена.");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await FeedbackService.InformError("Не удалось удалить скидку.");
                }
            }
        }
    }
}