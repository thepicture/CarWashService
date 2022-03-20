﻿using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.ViewModels;
using Plugin.Geolocator;
using System;
using System.Diagnostics;
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
            (BindingContext as BranchesViewModel)
              .SelectedLocation = (sender as Pin)
              .BindingContext as LocationHelper;
        }

        protected async override void OnAppearing()
        {
            _viewModel.OnAppearing();
            try
            {
                Plugin.Geolocator.Abstractions.Position position =
                    await CrossGeolocator.Current.GetPositionAsync(
                        timeout: TimeSpan.FromSeconds(10));
                BranchesMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        new Position(position.Latitude, position.Longitude),
                        Distance.FromKilometers(1)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            base.OnAppearing();
        }
    }
}