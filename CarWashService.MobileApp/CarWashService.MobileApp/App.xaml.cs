using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class App : Application
    {
        public string BaseUrl { get; set; } = "https://carwashservice-web.conveyor.cloud/api/";
        public SerializedBranch CurrentBranch { get; set; }
        public IEnumerable<SerializedService> CurrentServices { get; set; }
        public SerializedService CurrentService { get; set; }
        public SerializedOrder CurrentOrder { get; set; }
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
            DependencyService.Register<DiscountDataStore>();
            DependencyService.Register<OrderDataStore>();
            DependencyService.Register<CaptchaService>();

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
