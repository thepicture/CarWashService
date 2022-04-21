using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System;
using System.Collections.Generic;
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
        public static TimeSpan HttpClientTimeout = TimeSpan.FromSeconds(10);
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
