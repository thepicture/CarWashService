using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        private readonly AccountViewModel _viewModel;

        public AccountPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new AccountViewModel();
            _viewModel.IsRefreshing = true;
        }
    }
}