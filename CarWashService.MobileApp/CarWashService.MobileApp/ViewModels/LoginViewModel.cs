using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public IAuthenticator Authenticator =>
            DependencyService.Get<IAuthenticator>();
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClickedAsync);
        }

        private async void OnLoginClickedAsync(object obj)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(Login))
            {
                validationErrors.AppendLine("Введите логин");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                validationErrors.AppendLine("Введите пароль");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }

            bool isAuthenticated = false;
            try
            {
                isAuthenticated = await Authenticator
                .IsCorrectAsync(Login, Password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                await FeedbackService.Inform("Подключение к интернету " +
                     "отсутствует, проверьте подключение " +
                     "и попробуйте ещё раз");
                return;
            }
            if (isAuthenticated)
            {
                if (IsRememberMe)
                {
                    string encodedLoginAndPassword =
                        new LoginAndPasswordToBasicEncoder()
                        .Encode(Login, Password);
                    await SecureStorage
                        .SetAsync("Identity",
                                  encodedLoginAndPassword);
                    await SecureStorage
                       .SetAsync("Role",
                                 Authenticator.Role);
                }
                await FeedbackService.Inform("Вы авторизованы " +
                    $"как {Authenticator.Role}");
                (AppShell.Current as AppShell).SetShellStacksDependingOnRole();
            }
            else
            {
                await FeedbackService.InformError("Неверный логин или пароль");
            }
        }

        private string login;

        public string Login { get => login; set => SetProperty(ref login, value); }

        private string password;

        public string Password { get => password; set => SetProperty(ref password, value); }

        private Command exitCommand;

        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new Command(ExitAsync);
                }

                return exitCommand;
            }
        }

        private async void ExitAsync()
        {
            if (await FeedbackService.Ask("Выйти из приложения?"))
            {
                App.Current.Quit();
            }
        }

        private bool isRememberMe = false;

        public bool IsRememberMe
        {
            get => isRememberMe;
            set => SetProperty(ref isRememberMe, value);
        }
    }
}
