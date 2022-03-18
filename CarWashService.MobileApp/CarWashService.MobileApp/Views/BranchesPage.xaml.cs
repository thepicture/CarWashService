using CarWashService.MobileApp.Models;
using CarWashService.MobileApp.ViewModels;
using CarWashService.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    public partial class BranchesPage : ContentPage
    {
        BranchesViewModel _viewModel;

        public BranchesPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new BranchesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}