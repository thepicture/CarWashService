using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
            BindingContext = new AccountViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as AccountViewModel).OnAppearing();
        }
    }
}