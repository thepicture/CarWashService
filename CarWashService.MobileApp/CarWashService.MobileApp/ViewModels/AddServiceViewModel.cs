using CarWashService.MobileApp.Models.Serialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(CurrentService.Name))
            {
                _ = validationErrors.AppendLine("Введите наименование " +
                    "услуги.");
            }
            if (string.IsNullOrWhiteSpace(PriceString)
                || !int.TryParse(PriceString, out _)
                || int.Parse(PriceString) <= 0)
            {
                _ = validationErrors.AppendLine("Стоимость - " +
                    "это положительное целое число в рублях.");
            }
            if (CurrentType == null)
            {
                _ = validationErrors.AppendLine("Выберите тип услуги.");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }
            CurrentService.Price = int.Parse(PriceString);
            CurrentService.ServiceTypes = new List<string>
            {
                CurrentType
            };
            if (await ServiceDataStore.AddItemAsync(CurrentService))
            {
                string action = CurrentService.Id == 0
                    ? "добавлена"
                    : "изменена";
                await FeedbackService.Inform($"Услуга {action}.");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await FeedbackService.Inform("Не удалось " +
                    "сохранить услугу. Проверьте подключение к интернету.");
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
            if (await FeedbackService.Ask("Удалить услугу?"))
            {
                if (await ServiceDataStore
                    .DeleteItemAsync(CurrentService
                    .Id
                    .ToString()))
                {
                    await FeedbackService.Inform("Услуга удалена.");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await FeedbackService.InformError("Не удалось удалить услугу. " +
                        "Попробуйте ещё раз.");
                }
            }
        }

        public bool IsCanDeleteService => CurrentService.Id > 0;
    }
}