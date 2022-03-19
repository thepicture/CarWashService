﻿using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddServiceViewModel : BaseViewModel
    {
        public SerializedService CurrentService { get; set; }
            = new SerializedService();

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
                    "услуги");
            }
            if (string.IsNullOrWhiteSpace(PriceString)
                || !int.TryParse(PriceString, out _)
                || int.Parse(PriceString) <= 0)
            {
                _ = validationErrors.AppendLine("Стоимость - " +
                    "это положительное целое число в рублях");
            }
            if (CurrentType == null)
            {
                _ = validationErrors.AppendLine("Выберите тип услуги");
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
                CurrentType.Name
            };
            if (await ServiceDataStore.AddItemAsync(CurrentService))
            {
                await FeedbackService.Inform("Услуга добавлена");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await FeedbackService.Inform("Не удалось " +
                    "добавить услугу. Проверьте подключение к интернету");
            }
        }

        private string priceString;

        public string PriceString
        {
            get => priceString;
            set => SetProperty(ref priceString, value);
        }

        private ServiceTypeHelper currentType;

        public ServiceTypeHelper CurrentType
        {
            get => currentType;
            set => SetProperty(ref currentType, value);
        }
    }
}