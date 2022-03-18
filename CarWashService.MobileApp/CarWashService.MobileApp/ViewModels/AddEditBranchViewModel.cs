using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class AddEditBranchViewModel : BaseViewModel
    {
        private IEnumerable<SerializedCity> cities;

        public IEnumerable<SerializedCity> Cities
        {
            get => cities;
            set => SetProperty(ref cities, value);
        }

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
                    saveChangesCommand = new Command(SaveChanges);
                }

                return saveChangesCommand;
            }
        }

        private void SaveChanges()
        {
        }

        private SerializedCity currentCity;

        public SerializedCity CurrentCity
        {
            get => currentCity;
            set => SetProperty(ref currentCity, value);
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
            SerializedBranch currentBranch = (App.Current as App)
                .CurrentBranch;
            if (currentBranch.Id != 0)
            {
                Title = "Редактирование филиала";
                StreetName = currentBranch.StreetName;
                WorkFrom = TimeSpan.Parse(currentBranch.WorkFrom);
                WorkTo = TimeSpan.Parse(currentBranch.WorkTo);
            }
            else
            {
                Title = "Добавление филиала";
            }
            IsNotInReadMode = currentBranch.Id == 0;
            CurrentBranch = currentBranch;
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
    }
}