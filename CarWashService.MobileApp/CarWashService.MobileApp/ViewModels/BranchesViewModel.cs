using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.ViewModels
{
    public class BranchesViewModel : BaseViewModel
    {
        public ObservableCollection<LocationHelper> Locations { get; set; } =
            new ObservableCollection<LocationHelper>();

        internal void OnAppearing()
        {
            InsertBranchesIntoPositions();
        }

        private async void InsertBranchesIntoPositions()
        {
            IsRefreshing = true;
            Locations.Clear();
            IEnumerable<SerializedBranch> branches =
                await BranchDataStore.GetItemsAsync();
            try
            {
                Geocoder geoCoder = new Geocoder();
                foreach (SerializedBranch branch in branches)
                {
                    IEnumerable<Position> approximateLocations =
                        await geoCoder
                        .GetPositionsForAddressAsync(
                        string.Format("{0}, {1}, {2}",
                                      branch.StreetName,
                                      branch.CityName,
                                      "Россия")
                        );
                    Position position = approximateLocations
                        .FirstOrDefault();
                    Locations.Add(new LocationHelper
                    {
                        Address = $"{branch.StreetName}, " +
                        $"{branch.CityName}",
                        Description = "С "
                        + TimeSpan.Parse(branch.WorkFrom)
                        .ToString(@"hh\:mm")
                        + " по "
                        + TimeSpan.Parse(branch.WorkTo)
                        .ToString(@"hh\:mm"),
                        Position = position,
                        Branch = branch
                    });
                }
            }
            catch (Exception ex)
            {
                await FeedbackService.InformError(ex);
            }
            IsRefreshing = false;
        }

        private Command goToSelectedBranchPageCommand;

        public ICommand GoToSelectedBranchPageCommand
        {
            get
            {
                if (goToSelectedBranchPageCommand == null)
                {
                    goToSelectedBranchPageCommand =
                        new Command(GoToSelectedBranchPageAsync);
                }

                return goToSelectedBranchPageCommand;
            }
        }

        private bool canGoToSelectedBranchPageExecute;

        private async void GoToSelectedBranchPageAsync()
        {
            await AppShell.Current.Navigation.PushAsync(
                new AddEditBranchPage(
                    new AddEditBranchViewModel(SelectedLocation.Branch)));
        }

        private Command goToAddBranchPageCommand;

        public ICommand GoToAddBranchPageCommand
        {
            get
            {
                if (goToAddBranchPageCommand == null)
                {
                    goToAddBranchPageCommand = new Command(GoToAddBranchPageAsync);
                }

                return goToAddBranchPageCommand;
            }
        }

        private async void GoToAddBranchPageAsync()
        {
            await AppShell.Current.Navigation.PushAsync(
                new AddEditBranchPage(
                    new AddEditBranchViewModel(
                        new SerializedBranch())));
        }

        private LocationHelper selectedLocation;

        public LocationHelper SelectedLocation
        {
            get => selectedLocation;
            set
            {
                if (SetProperty(ref selectedLocation, value))
                {
                    CanGoToSelectedBranchPageExecute = selectedLocation != null;
                }
            }
        }

        public bool CanGoToSelectedBranchPageExecute
        {
            get => canGoToSelectedBranchPageExecute;
            set => SetProperty(ref canGoToSelectedBranchPageExecute, value);
        }
    }
}