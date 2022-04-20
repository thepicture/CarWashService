using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new LoginViewModel();
            MessagingCenter.Subscribe<LoginViewModel>(this,
                                                      "ReloadCaptcha",
                                                      OnReloadCaptcha);
        }

        private void OnReloadCaptcha(LoginViewModel obj)
        {
            Captcha.Text = _viewModel.CaptchaService.Text;
        }
    }
}