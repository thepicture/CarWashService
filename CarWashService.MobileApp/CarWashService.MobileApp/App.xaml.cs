using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class App : Application
    {
        public static string BaseUrl
        {
            get => Preferences.Get(
                nameof(BaseUrl), baseUrl);

            set => Preferences.Set(
                nameof(BaseUrl), value);
        }
        public static SerializedUser User { get; set; }
        public static string AuthorizationValue { get; set; }
        public static TimeSpan HttpClientTimeout = TimeSpan.FromSeconds(30);
        private static readonly string baseUrl = "https://lostbluephone14.conveyor.cloud/api/";
        private readonly string andAccelerometerIsOff = "Вы не сможете "
            + "встряхнуть устройство "
            + "для изменения таймаута API в сек.";
        private readonly string accelerometerNotSupported = "На вашем устройстве "
            + "не поддерживается акселерометр.";
        private readonly string cannotHandleAccelerometer = "Не удалось "
            + "настроить акселерометр.";
        public static int DefaultImageWidth = 300;
        public static int DefaultImageHeight = 300;
        public static int DefaultQuality = 30;

        public static readonly string EmployeeCode = "123456";

        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);

            DependencyService.Register<BranchToLocationHelperListConverter>();
            DependencyService.Register<HttpFactoryService>();
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
            DependencyService.Register<AndroidImageResizer>();

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
