using CarWashService.MobileApp.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddEditBranchPage), typeof(AddEditBranchPage));
            if (VersionTracking.IsFirstLaunchForCurrentBuild)
            {
                SecureStorage.RemoveAll();
            }
            if (IsLoggedIn())
            {
                SetShellStacksDependingOnRole();
            }
            else
            {
                CommonTabBar
                    .Items.Add(new ShellContent
                    {
                        Route = "LoginPage",
                        Title = "Авторизация",
                        Icon = "authorization",
                        ContentTemplate = new DataTemplate(typeof(LoginPage))
                    });
                CommonTabBar
                  .Items.Add(new ShellContent
                  {
                      Route = "LoginPage",
                      Title = "Регистрация",
                      Icon = "registration",
                      ContentTemplate = new DataTemplate(typeof(RegisterPage))
                  });
            }
        }

        private bool IsLoggedIn()
        {
            return SecureStorage.GetAsync("Identity").Result != null;
        }

        public async void SetShellStacksDependingOnRole()
        {
            CommonTabBar.Items.Clear();
            string role = await SecureStorage.GetAsync("Role");
            switch (role)
            {
                case "Администратор":
                    CommonTabBar
                        .Items.Add(new ShellContent
                        {
                            Route = "BranchesPage",
                            Title = "Филиалы",
                            Icon = "branch",
                            ContentTemplate = new DataTemplate(typeof(BranchesPage))
                        });
                    break;
                case "Сотрудник":
                    CommonTabBar
                      .Items.Add(new ShellContent
                      {
                          Route = "BranchesPage",
                          Title = "Филиалы",
                          Icon = "branch",
                          ContentTemplate = new DataTemplate(typeof(BranchesPage))
                      });
                    break;
                case "Клиент":
                    break;
                default:
                    break;
            }
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
