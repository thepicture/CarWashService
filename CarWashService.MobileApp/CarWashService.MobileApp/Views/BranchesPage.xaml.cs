using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.Views
{
    public partial class BranchesPage : ContentPage
    {
        public BranchesPage()
        {
            InitializeComponent();
            BindingContext = new BranchesViewModel();
        }

        private void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            (BindingContext as BranchesViewModel)
              .SelectedLocation = (sender as Pin)
              .BindingContext as LocationHelper;
        }
    }
}