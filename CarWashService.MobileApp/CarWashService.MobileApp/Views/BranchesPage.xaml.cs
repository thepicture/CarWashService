using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.Views
{
    public partial class BranchesPage : ContentPage
    {
        BranchesViewModel _viewModel;
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

        protected override void OnAppearing()
        {
            _viewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}