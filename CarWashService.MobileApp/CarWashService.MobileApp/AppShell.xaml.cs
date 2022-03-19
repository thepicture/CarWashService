using CarWashService.MobileApp.Services;
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
            Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ServicesPage), typeof(ServicesPage));
            Routing.RegisterRoute(nameof(AddServicePage), typeof(AddServicePage));
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
                LoadLoginAndRegisterShell();
            }
        }

        public void LoadLoginAndRegisterShell()
        {
            CommonTabBar.Items.Clear();
            CommonTabBar
                .Items.Add(new ShellContent
                {
                    Route = nameof(LoginPage),
                    Icon = "authorization",
                    Title = "Авторизация",
                    ContentTemplate = new DataTemplate(typeof(LoginPage))
                });
            CommonTabBar
              .Items.Add(new ShellContent
              {
                  Route = nameof(LoginPage),
                  Icon = "registration",
                  Title = "Регистрация",
                  ContentTemplate = new DataTemplate(typeof(RegisterPage))
              });
        }

        private bool IsLoggedIn()
        {
            return SecureStorage.GetAsync("Identity").Result != null;
        }

        public void SetShellStacksDependingOnRole()
        {
            CommonTabBar.Items.Clear();
            switch (AppIdentity.Role)
            {
                case "Администратор":
                    CommonTabBar
                        .Items.Add(new ShellContent
                        {
                            Route = nameof(BranchesPage),
                            Icon = "branch",
                            Title = "Филиалы",
                            ContentTemplate = new DataTemplate(typeof(BranchesPage))
                        });
                    CommonTabBar
                       .Items.Add(new ShellContent
                       {
                           Route = nameof(ServicesPage),
                           Icon = "logo",
                           Title = "Услуги",
                           ContentTemplate = new DataTemplate(typeof(ServicesPage))
                       });
                    break;
                case "Сотрудник":
                    CommonTabBar
                      .Items.Add(new ShellContent
                      {
                          Route = nameof(BranchesPage),
                          Icon = "branch",
                          Title = "Филиалы",
                          ContentTemplate = new DataTemplate(typeof(BranchesPage))
                      });
                    CommonTabBar
                      .Items.Add(new ShellContent
                      {
                          Route = nameof(ServicesPage),
                          Icon = "logo",
                          Title = "Услуги",
                          ContentTemplate = new DataTemplate(typeof(ServicesPage))
                      });
                    break;
                case "Клиент":
                    break;
                default:
                    break;
            }
            CommonTabBar
                 .Items.Add(new ShellContent
                 {
                     Route = nameof(AccountPage),
                     Icon = "profile",
                     Title = "Профиль",
                     ContentTemplate = new DataTemplate(typeof(AccountPage))
                 });
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
