using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.ViewModels;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public class AccountViewModel : BaseViewModel
    {
        private string login;

        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }

        private string role;

        public string Role
        {
            get => role;
            set => SetProperty(ref role, value);
        }

        private Command exitLoginCommand;

        public AccountViewModel()
        {
            string[] loginAndPassword = new LoginAndPasswordFromBasicDecoder()
                .DecodeAsync().Result;
            Login = loginAndPassword[0];

            if ((App.Current as App).Role != null)
            {
                Role = (App.Current as App).Role;
            }
            else
            {
                Role = SecureStorage.GetAsync("Role").Result;
            }
        }

        public ICommand ExitLoginCommand
        {
            get
            {
                if (exitLoginCommand == null)
                {
                    exitLoginCommand = new Command(ExitLoginAsync);
                }

                return exitLoginCommand;
            }
        }

        private async void ExitLoginAsync()
        {
            if (await FeedbackService.Ask("Выйти из аккаунта?"))
            {
                SecureStorage.RemoveAll();
                (App.Current as App).Role = null;
                (App.Current as App).Identity = null;
                (AppShell.Current as AppShell).LoadLoginAndRegisterShell();
            }
        }
    }
}