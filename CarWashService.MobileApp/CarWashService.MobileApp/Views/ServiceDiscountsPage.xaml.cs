using CarWashService.MobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceDiscountsPage : ContentPage
    {
        private readonly ServiceDiscountsViewModel _viewModel;
        public ServiceDiscountsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ServiceDiscountsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}