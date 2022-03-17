using CarWashService.MobileApp.Views;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }

        private string login;

        public string Login { get => login; set => SetProperty(ref login, value); }

        private string password;

        public string Password { get => password; set => SetProperty(ref password, value); }
    }
}
