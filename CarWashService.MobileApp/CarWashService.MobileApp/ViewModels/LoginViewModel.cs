using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public SerializedLoginUser LoginUser
        {
            get => loginUser;
            set => SetProperty(ref loginUser, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClickedAsync);
            LoginUser = new SerializedLoginUser();
        }

        private const int InterfaceFreezeTimeoutInSeconds = 5;

        private async void OnLoginClickedAsync(object obj)
        {
            IsBusy = true;
            IsRefreshing = true;
            if (string.IsNullOrWhiteSpace(CaptchaText)
                && CaptchaService.CountOfAttempts > 2)
            {
                await FeedbackService.InformError("Введите captcha.");
            }
            else
            {
                if (CaptchaText != null
                    && !CaptchaText.Equals(CaptchaService.Text,
                                           StringComparison.OrdinalIgnoreCase))
                {
                    await FeedbackService.InformError(
                        "Неверная captcha. "
                        + "Интерфейс заблокирован "
                        + $"на {InterfaceFreezeTimeoutInSeconds} секунд.");
                    IsNotBlocked = false;
                    Device.StartTimer(
                        TimeSpan.FromSeconds(InterfaceFreezeTimeoutInSeconds), () =>
                        {
                            _ = FeedbackService
                                .Inform("Интерфейс разблокирован.");
                            IsNotBlocked = true;
                            return false;
                        });
                }
                else
                {
                    if (await LoginDataStore.AddItemAsync(LoginUser))
                    {
                        AppShell.SetShellStacksDependingOnRole();
                        CaptchaService.Invalidate();
                    }
                    else
                    {
                        CaptchaService.CountOfAttempts++;
                        if (CaptchaService.CountOfAttempts == 3)
                        {
                            CaptchaService.GenerateNew();
                        }
                    }
                }
            }
            IsRefreshing = false;
            IsBusy = false;
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
        }

        private Command changeBaseUrlCommand;
        private SerializedLoginUser loginUser;

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
