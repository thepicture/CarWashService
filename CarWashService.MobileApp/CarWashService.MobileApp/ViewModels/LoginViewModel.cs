using CarWashService.MobileApp.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public int CountOfIncorrectAttempts
        {
            get => countOfIncorrectAttempts;
            set => SetProperty(ref countOfIncorrectAttempts, value);
        }
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
                _ = validationErrors.AppendLine("Введите логин.");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                _ = validationErrors.AppendLine("Введите пароль.");

            }
            if (string.IsNullOrWhiteSpace(CaptchaText) && CountOfIncorrectAttempts > 2)
            {
                _ = validationErrors.AppendLine("Введите captcha.");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }

            if (CaptchaText != null && !CaptchaText.Equals(CaptchaService.Text,
                                   StringComparison.OrdinalIgnoreCase))
            {
                _ = FeedbackService.InformError("Неверная captcha. " +
                    "Интерфейс заблокирован на 5 секунд.");
                IsNotBlocked = false;
                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FeedbackService.Inform("Интерфейс разблокирован.");
                        IsNotBlocked = true;
                    });
                    return false;
                });
                return;
            }

            bool isAuthenticated;
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
                     "и попробуйте ещё раз.");
                return;
            }
            if (isAuthenticated)
            {
                string encodedLoginAndPassword =
                    new LoginAndPasswordToBasicEncoder()
                    .Encode(Login, Password);
                if (IsRememberMe)
                {
                    await SecureStorage
                        .SetAsync("Identity",
                                  encodedLoginAndPassword);
                    await SecureStorage
                       .SetAsync("User",
                                 JsonConvert.SerializeObject(Authenticator.User));
                }
                else
                {
                    (App.Current as App).User = Authenticator.User;
                    (App.Current as App).Role = Authenticator.User.UserTypeName;
                    (App.Current as App).Identity = encodedLoginAndPassword;
                }
                await FeedbackService.Inform("Вы авторизованы " +
                    $"как {AppIdentity.User.UserTypeName.ToLower()}.");
                (AppShell.Current as AppShell).SetShellStacksDependingOnRole();
                CountOfIncorrectAttempts = 0;
                CaptchaService.Invalidate();
            }
            else
            {
                await FeedbackService.InformError("Неверный логин или пароль.");
                CountOfIncorrectAttempts++;
                if (CountOfIncorrectAttempts == 3)
                {
                    CaptchaService.GenerateNew();
                    MessagingCenter.Send(this, "ReloadCaptcha");
                }
            }
        }
        private MemoryStream captchaImage;
        private string login;

        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }

        private string password;

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

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
                System.Environment.Exit(0);
            }
        }

        private bool isRememberMe = false;
        private int countOfIncorrectAttempts = 0;

        public bool IsRememberMe
        {
            get => isRememberMe;
            set => SetProperty(ref isRememberMe, value);
        }
        public MemoryStream CaptchaImage
        {
            get => captchaImage;
            set => SetProperty(ref captchaImage, value);
        }

        private Command regenerateCaptchaCommand;
        private string captchaText;
        private bool isNotBlocked = true;

        public ICommand RegenerateCaptchaCommand
        {
            get
            {
                if (regenerateCaptchaCommand == null)
                {
                    regenerateCaptchaCommand = new Command(RegenerateCaptcha);
                }

                return regenerateCaptchaCommand;
            }
        }

        public string CaptchaText
        {
            get => captchaText;
            set => SetProperty(ref captchaText, value);
        }
        public bool IsNotBlocked
        {
            get => isNotBlocked;
            set => SetProperty(ref isNotBlocked, value);
        }

        private void RegenerateCaptcha()
        {
            CaptchaService.GenerateNew();
            MessagingCenter.Send(this, "ReloadCaptcha");
        }
    }
}
