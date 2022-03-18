using CarWashService.MobileApp.Services;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class App : Application
    {
        public string BaseUrl { get; set; } = "https://carwashservice-web.conveyor.cloud/api";
        public App()
        {
            InitializeComponent();

            XF.Material.Forms.Material.Init(this);

            DependencyService.Register<ToastFeedbackService>();
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<ApiAuthenticator>();
            DependencyService.Register<ApiRegistrator>();
            MainPage = new AppShell();
            Shell.Current.GoToAsync("//LoginPage");
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
