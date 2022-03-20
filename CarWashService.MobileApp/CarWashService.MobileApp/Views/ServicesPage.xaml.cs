
using CarWashService.MobileApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesPage : ContentPage
    {
        private readonly ServicesViewModel _viewModel;
        public ServicesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ServicesViewModel();
            if (AppIdentity.Role == "Клиент")
            {
                ToolbarItems.Clear();
            }
        }
        protected override void OnAppearing()
        {
            _viewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}