using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            MessagingCenter.Subscribe<LoginViewModel>(this, "ReloadCaptcha", OnReloadCaptcha);
        }

        private void OnReloadCaptcha(LoginViewModel obj)
        {
            Captcha.Text = (BindingContext as LoginViewModel).CaptchaService.Text;
        }
    }
}