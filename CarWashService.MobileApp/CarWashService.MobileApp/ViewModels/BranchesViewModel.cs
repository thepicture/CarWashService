﻿using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            Task.Run(() =>
            {
                return InsertBranchesIntoPositions();
            });
        }

        private async Task InsertBranchesIntoPositions()
        {
            Locations.Clear();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                                                AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    string response = await client
                        .GetAsync("branches")
                        .Result
                        .Content
                        .ReadAsStringAsync();
                    IEnumerable<SerializedBranch> branches = JsonConvert
                        .DeserializeObject
                        <IEnumerable<SerializedBranch>>
                        (response);
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
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }
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
            (App.Current as App).CurrentBranch = SelectedLocation.Branch;
            await Shell.Current.GoToAsync($"{nameof(AddEditBranchPage)}");
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
            (App.Current as App).CurrentBranch = new SerializedBranch();
            await Shell.Current.GoToAsync($"{nameof(AddEditBranchPage)}");
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