using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class BranchesViewModel : BaseViewModel
    {
        public IListConverter<SerializedBranch, LocationHelper> BranchToPositionConverter =
           DependencyService.Get<IListConverter<SerializedBranch, LocationHelper>>();

        public ObservableCollection<LocationHelper> Locations
        {
            get => locations;
            set => SetProperty(ref locations, value);
        }
        internal async Task OnAppearing()
        {
            await InsertBranchesIntoPositions();
        }

        private async Task InsertBranchesIntoPositions()
        {
            IsRefreshing = true;
            IEnumerable<SerializedBranch> branches = await BranchDataStore.GetItemsAsync();
            IEnumerable<LocationHelper> currentBranches =
                await BranchToPositionConverter.ConvertAllAsync(branches);
            Locations = new ObservableCollection<LocationHelper>(currentBranches);
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
        private ObservableCollection<LocationHelper> locations = new ObservableCollection<LocationHelper>();

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