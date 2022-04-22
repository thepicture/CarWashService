using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class App : Application
    {
        public static string BaseUrl { get; set; } =
            "https://carwashservice-web.conveyor.cloud/api/";
        public static SerializedBranch CurrentBranch { get; set; }
        public static IEnumerable<SerializedService> CurrentServices
        { get; set; }
        public static SerializedService CurrentService { get; set; }
        public static SerializedOrder CurrentOrder { get; set; }
        public static SerializedUser User { get; set; }
        public static string AuthorizationValue { get; set; }
        public static TimeSpan HttpClientTimeout = TimeSpan.FromSeconds(20);
        private readonly string andAccelerometerIsOff = "Вы не сможете "
            + "встряхнуть устройство "
            + "для изменения таймаута API в сек.";
        private readonly string accelerometerNotSupported = "На вашем устройстве "
            + "не поддерживается акселерометр.";
        private readonly string cannotHandleAccelerometer = "Не удалось " +
            "настроить акселерометр.";
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);

            DependencyService.Register<AndroidFeedbackService>();
            DependencyService.Register<BranchDataStore>();
            DependencyService.Register<RegistrationDataStore>();
            DependencyService.Register<UserImageDataStore>();
            DependencyService.Register<ServiceDataStore>();
            DependencyService.Register<DiscountDataStore>();
            DependencyService.Register<LoginDataStore>();
            DependencyService.Register<OrderDataStore>();
            DependencyService.Register<OrderServicesDataStore>();
            DependencyService.Register<CaptchaService>();

            try
            {
                Accelerometer.Start(SensorSpeed.UI);
                Accelerometer.ShakeDetected += async (_, __) =>
                {
                    Vibration.Vibrate(TimeSpan.FromMilliseconds(100));
                    string secondsString = await MainPage.DisplayPromptAsync(
                        "Установить таймаут API",
                        $"Текущий таймаут API: " +
                        $"{HttpClientTimeout.TotalSeconds:F0} сек. " +
                        $"Введите новый таймаут",
                        keyboard: Keyboard.Numeric,
                        initialValue: HttpClientTimeout.TotalSeconds.ToString("F0"));
                    if (int.TryParse(secondsString, out int seconds))
                    {
                        HttpClientTimeout = TimeSpan.FromSeconds(seconds);
                    }
                    if (await DependencyService
                    .Get<IFeedbackService>()
                    .Ask("Вызвать "
                            + "исключение "
                            + "для аварийного завершения "
                            + "приложения?"))
                    {
                        throw new Exception("Тестовое исключение");
                    }
                };
            }
            catch (FeatureNotSupportedException)
            {
                DependencyService
                    .Get<IFeedbackService>()
                    .Warn($"{accelerometerNotSupported} {andAccelerometerIsOff}");
            }
            catch (ArgumentNullException)
            {
                DependencyService
                    .Get<IFeedbackService>()
                    .Warn($"{cannotHandleAccelerometer} {andAccelerometerIsOff}");
            }

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
