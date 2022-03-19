using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class App : Application
    {
        public string BaseUrl { get; set; } = "https://carwashservice-web.conveyor.cloud/api/";
        public SerializedBranch CurrentBranch { get; set; }
        public string Role { get; set; }
        public string Identity { get; set; }

        public App()
        {
            InitializeComponent();

            XF.Material.Forms.Material.Init(this);

            DependencyService.Register<ToastFeedbackService>();
            DependencyService.Register<BranchDataStore>();
            DependencyService.Register<ApiAuthenticator>();
            DependencyService.Register<ApiRegistrator>();
            DependencyService.Register<ServiceDataStore>();
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
