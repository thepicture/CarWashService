using CarWashService.MobileApp.Models;
using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddEditBranchViewModel : BaseViewModel
    {
        private string streetName;

        public string StreetName
        {
            get => streetName;
            set => SetProperty(ref streetName, value);
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
            if (string.IsNullOrWhiteSpace(CurrentBranch.Title))
            {
                _ = validationErrors.AppendLine("Введите наименование " +
                    "филиала");
            }
            if (string.IsNullOrWhiteSpace(CityName))
            {
                _ = validationErrors.AppendLine("Укажите город");
            }
            if (string.IsNullOrWhiteSpace(StreetName))
            {
                _ = validationErrors.AppendLine("Введите название улицы");
            }
            if (WorkFrom >= WorkTo)
            {
                _ = validationErrors.AppendLine("Время " +
                    "начала работы должно быть " +
                    "раньше времени окончания работы");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }

            CurrentBranch.CityName = CityName;
            CurrentBranch.StreetName = StreetName;
            CurrentBranch.WorkFrom = WorkFrom.ToString();
            CurrentBranch.WorkTo = WorkTo.ToString();
            foreach (PhoneNumberHelper phoneNumber in PhoneNumbers)
            {
                CurrentBranch.PhoneNumbers.Add(phoneNumber.PhoneNumber);
            }
            bool isSuccessfulAdding;
            bool isNewBranch;
            try
            {
                isSuccessfulAdding = await BranchDataStore
                    .AddItemAsync(CurrentBranch);
                isNewBranch = CurrentBranch.Id == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                await FeedbackService.Inform("Подключение к интернету " +
                     "отсутствует, проверьте подключение " +
                     "и попробуйте ещё раз");
                return;
            }
            if (isSuccessfulAdding)
            {
                string action = CurrentBranch.Id == 0 ? "добавлен" : "обновлен";
                await FeedbackService.Inform($"Филиал {CurrentBranch.Title} " +
                    $"{action}");
                await Shell.Current.GoToAsync($"..");
            }
            else
            {
                await FeedbackService.InformError("Не удалось " +
                    "добавить филиал. " +
                    "Вероятно, политика компании изменилась. " +
                    "Обратитесь к системному администратору");
            }
        }

        private bool isNotInReadMode;
        private SerializedBranch currentBranch;

        public SerializedBranch CurrentBranch
        {
            get => currentBranch;
            set => SetProperty(ref currentBranch, value);
        }

        public AddEditBranchViewModel()
        {
            CurrentBranch = (App.Current as App)
                .CurrentBranch;
            if (CurrentBranch.Id != 0)
            {
                Title = "Филиал " + CurrentBranch.Title;
                StreetName = CurrentBranch.StreetName;
                CityName = CurrentBranch.CityName;
                WorkFrom = TimeSpan.Parse(CurrentBranch.WorkFrom);
                WorkTo = TimeSpan.Parse(CurrentBranch.WorkTo);
                foreach (string phoneNumber in CurrentBranch.PhoneNumbers)
                {
                    PhoneNumbers.Add(new PhoneNumberHelper
                    {
                        PhoneNumber = phoneNumber
                    });
                }
            }
            else
            {
                Title = "Добавление филиала";
            }
            IsNotInReadMode = CurrentBranch.Id == 0;
        }

        public bool IsNotInReadMode
        {
            get => isNotInReadMode;
            set => SetProperty(ref isNotInReadMode, value);
        }

        private TimeSpan workFrom = TimeSpan.FromHours(8);

        public TimeSpan WorkFrom
        {
            get => workFrom;
            set => SetProperty(ref workFrom, value);
        }

        private TimeSpan workTo = TimeSpan.FromHours(18);

        public TimeSpan WorkTo
        {
            get => workTo;
            set => SetProperty(ref workTo, value);
        }
        public ObservableCollection<PhoneNumberHelper> PhoneNumbers
        {
            get;
            set;
        } = new ObservableCollection<PhoneNumberHelper>();

        private string cityName;

        public string CityName
        {
            get => cityName;
            set => SetProperty(ref cityName, value);
        }

        private Command addContactCommand;

        public ICommand AddContactCommand
        {
            get
            {
                if (addContactCommand == null)
                {
                    addContactCommand = new Command(AddContactAsync);
                }

                return addContactCommand;
            }
        }

        private async void AddContactAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPhone))
            {
                await FeedbackService.InformError("Чтобы добавить "
                    + "контакт, введите номер телефона "
                    + "и попробуйте ещё раз");
                return;
            }
            if (PhoneNumbers.Any(
                p => p.PhoneNumber == MaskDeleter.DeleteMask(CurrentPhone)))
            {
                await FeedbackService.InformError("Такой " +
                    "контакт уже есть");
                return;
            }
            if (CurrentPhone.Length != 18)
            {
                await FeedbackService.Warn("Номер телефона " +
                    "некорректен. Убедитесь, что вы ввели номер " +
                    "в формате +7...");
                return;
            }
            PhoneNumbers.Add(new PhoneNumberHelper
            {
                PhoneNumber = MaskDeleter.DeleteMask(CurrentPhone)
            });
            CurrentPhone = string.Empty;
            await FeedbackService.Inform("Контакт добавлен " +
                "локально. " +
                "После нажатия кнопки сохранения " +
                "информация будет обновлена");
        }

        private string currentPhone;

        public string CurrentPhone
        {
            get => currentPhone;
            set => SetProperty(ref currentPhone, value);
        }
    }
}