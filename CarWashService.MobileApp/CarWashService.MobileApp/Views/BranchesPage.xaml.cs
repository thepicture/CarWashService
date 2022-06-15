using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.ViewModels;
using Plugin.Geolocator;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.Views
{
    public partial class BranchesPage : ContentPage
    {
        readonly BranchesViewModel _viewModel;
        public BranchesPage()
        {
            InitializeComponent();
            _viewModel = new BranchesViewModel();
            BindingContext = _viewModel;
        }

        private void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            _viewModel
              .SelectedLocation = (sender as Pin)
              .BindingContext as LocationHelper;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
            try
            {
                Plugin.Geolocator.Abstractions.Position position =
                    await CrossGeolocator.Current.GetPositionAsync();
                BranchesMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        new Position(
                            Math.Round(position.Latitude, 4), Math.Round(position.Longitude, 4)),
                        Distance.FromKilometers(1)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}