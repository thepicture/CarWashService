using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MakeOrderPage : ContentPage
    {
        private readonly MakeOrderViewModel _viewModel;
        public MakeOrderPage(MakeOrderViewModel makeOrderViewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = makeOrderViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}