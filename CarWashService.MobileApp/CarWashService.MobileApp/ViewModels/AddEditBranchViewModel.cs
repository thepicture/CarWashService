using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Diagnostics;
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
            if (string.IsNullOrWhiteSpace(Title))
            {
                validationErrors.AppendLine("Введите наименование " +
                    "филиала");
            }
            if (string.IsNullOrWhiteSpace(CityName))
            {
                validationErrors.AppendLine("Укажите город");
            }
            if (string.IsNullOrWhiteSpace(StreetName))
            {
                validationErrors.AppendLine("Введите название улицы");
            }
            if (WorkFrom >= WorkTo)
            {
                validationErrors.AppendLine("Время " +
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
            bool isSuccessfulAdding;
            try
            {
                isSuccessfulAdding = await BranchDataStore
                    .AddItemAsync(CurrentBranch);
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
                await FeedbackService.Inform($"Филиал {CurrentBranch.Title} " +
                    "добавлен");
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

        private string cityName;

        public string CityName
        {
            get => cityName;
            set => SetProperty(ref cityName, value);
        }
    }
}