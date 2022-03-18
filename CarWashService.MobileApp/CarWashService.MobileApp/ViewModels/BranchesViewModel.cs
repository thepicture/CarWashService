using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.ViewModels
{
    public class BranchesViewModel : BaseViewModel
    {
        public ObservableCollection<LocationHelper> Locations { get; set; } =
            new ObservableCollection<LocationHelper>();
        public BranchesViewModel()
        {
            _ = InsertBranchesIntoPositions();
        }

        private async Task InsertBranchesIntoPositions()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization,
                                   await SecureStorage
                                    .GetAsync("Identity"));
                client.BaseAddress = (App.Current as App).BaseUrl;
                try
                {
                    byte[] response = client
                        .DownloadData("api/branches");
                    string branchesJsonString = Encoding.UTF8
                        .GetString(response);
                    IEnumerable<SerializedBranch> branches = JsonConvert
                        .DeserializeObject
                        <IEnumerable<SerializedBranch>>
                        (branchesJsonString);
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
                            Description = "С " +
                            $"{branch.WorkFrom} " +
                            "по " +
                            $"{branch.WorkTo}",
                            Position = position,
                            Branch = branch
                        });
                    }
                }
                catch (WebException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private Command goToBranchViewModelCommand;

        public ICommand GoToBranchViewModelCommand
        {
            get
            {
                if (goToBranchViewModelCommand == null)
                {
                    goToBranchViewModelCommand =
                        new Command(PerformGoToBranchViewModel);
                }

                return goToBranchViewModelCommand;
            }
        }

        private bool canGoToBranchViewModelExecute;

        private void PerformGoToBranchViewModel()
        {
        }

        private Command goToAddBranchCommand;

        public ICommand GoToAddBranchCommand
        {
            get
            {
                if (goToAddBranchCommand == null)
                {
                    goToAddBranchCommand = new Command(GoToBranchViewModel);
                }

                return goToAddBranchCommand;
            }
        }

        private void GoToBranchViewModel()
        {
        }

        private LocationHelper selectedLocation;

        public LocationHelper SelectedLocation
        {
            get => selectedLocation;
            set
            {
                if (SetProperty(ref selectedLocation, value))
                {
                    CanGoToBranchViewModelExecute = selectedLocation != null;
                }
            }
        }

        public bool CanGoToBranchViewModelExecute
        {
            get => canGoToBranchViewModelExecute;
            set => SetProperty(ref canGoToBranchViewModelExecute, value);
        }
    }
}