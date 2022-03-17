using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
        }
    }
}