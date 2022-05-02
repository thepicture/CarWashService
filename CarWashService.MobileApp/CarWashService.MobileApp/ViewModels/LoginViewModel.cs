using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
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
            if (string.IsNullOrWhiteSpace(CaptchaText)
                && CountOfIncorrectAttempts > 2)
            {
                await FeedbackService.InformError("Введите captcha.");
                return;
            }

            if (CaptchaText != null && !CaptchaText.Equals(CaptchaService.Text,
                                   StringComparison.OrdinalIgnoreCase))
            {
                await FeedbackService.InformError("Неверная captcha. " +
                    "Интерфейс заблокирован на 5 секунд.");
                IsNotBlocked = false;
                Device.StartTimer(
                    TimeSpan.FromSeconds(5), () =>
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

            string encodedLoginAndPassword =
                new LoginAndPasswordToBasicEncoder()
                .Encode(Login, Password);
            AppIdentity.AuthorizationValue = encodedLoginAndPassword;
            SerializedLoginUser serializedLoginUser =
                new SerializedLoginUser
                {
                    Login = Login,
                    Password = Password,
                    IsRememberMe = IsRememberMe
                };
            if (await LoginDataStore.AddItemAsync(serializedLoginUser))
            {
                AppShell.SetShellStacksDependingOnRole();
                CountOfIncorrectAttempts = 0;
                CaptchaService.Invalidate();
            }
            else
            {
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

        private Command changeBaseUrlCommand;

        public ICommand ChangeBaseUrlCommand
        {
            get
            {
                if (changeBaseUrlCommand == null)
                {
                    changeBaseUrlCommand = new Command(ChangeBaseUrl);
                }

                return changeBaseUrlCommand;
            }
        }

        private async void ChangeBaseUrl()
        {
            string currentUrl = App.BaseUrl;
            string url = await AppShell.Current.CurrentPage.DisplayPromptAsync(
                        "Установить URL",
                        "Текущий URL:\n" +
                        $"{currentUrl}\n" +
                        "Введите новый URL",
                        keyboard: Keyboard.Text,
                        initialValue: currentUrl);
            if (url != null)
            {
                App.BaseUrl = url;
            }
        }
    }
}
