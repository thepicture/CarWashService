using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public partial class AppShell : Shell
    {
        public static TabBar TabBar = new TabBar();

        public AppShell()
        {
            InitializeComponent();
            Items.Add(TabBar);

            Routing.RegisterRoute(
                nameof(AddEditBranchPage),
                typeof(AddEditBranchPage));
            Routing.RegisterRoute(
                nameof(LoginPage),
                typeof(LoginPage));
            Routing.RegisterRoute(
                nameof(ServiceDiscountsPage),
                typeof(ServiceDiscountsPage));
            Routing.RegisterRoute(
                nameof(AddDiscountPage),
                typeof(AddDiscountPage));
            Routing.RegisterRoute(
               nameof(MakeOrderPage),
               typeof(MakeOrderPage));
            if (IsLoggedIn())
            {
                SetShellStacksDependingOnRole();
            }
            else
            {
                LoadLoginAndRegisterShell();
            }
        }

        public static void LoadLoginAndRegisterShell()
        {
            TabBar.Items.Clear();
            TabBar
                .Items.Add(new ShellContent
                {
                    Route = nameof(LoginPage),
                    Icon = "authorization",
                    Title = "Авторизация",
                    ContentTemplate = new DataTemplate(typeof(LoginPage))
                });
            TabBar
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

        public static void SetShellStacksDependingOnRole()
        {
            TabBar.Items.Clear();
            switch (AppIdentity.Role)
            {
                case "Администратор":
                    TabBar
                        .Items.Add(new ShellContent
                        {
                            Route = nameof(BranchesPage),
                            Icon = "branch",
                            Title = "Филиалы",
                            ContentTemplate = new DataTemplate(typeof(BranchesPage))
                        });
                    TabBar
                       .Items.Add(new ShellContent
                       {
                           Route = nameof(ServicesPage),
                           Icon = "logo",
                           Title = "Услуги",
                           ContentTemplate = new DataTemplate(typeof(ServicesPage))
                       });
                    break;
                case "Сотрудник":
                    TabBar
                      .Items.Add(new ShellContent
                      {
                          Route = nameof(BranchesPage),
                          Icon = "branch",
                          Title = "Филиалы",
                          ContentTemplate = new DataTemplate(typeof(BranchesPage))
                      });
                    TabBar
                      .Items.Add(new ShellContent
                      {
                          Route = nameof(ServicesPage),
                          Icon = "logo",
                          Title = "Услуги",
                          ContentTemplate = new DataTemplate(typeof(ServicesPage))
                      });
                    break;
                case "Клиент":
                    TabBar
                      .Items.Add(new ShellContent
                      {
                          Route = nameof(BranchesPage),
                          Icon = "branch",
                          Title = "Филиалы",
                          ContentTemplate = new DataTemplate(typeof(BranchesPage))
                      });
                    TabBar
                   .Items.Add(new ShellContent
                   {
                       Route = nameof(ServicesPage),
                       Icon = "logo",
                       Title = "Услуги",
                       ContentTemplate = new DataTemplate(typeof(ServicesPage))
                   });
                    break;
                default:
                    break;
            }
            TabBar
               .Items.Add(new ShellContent
               {
                   Route = nameof(OrdersPage),
                   Icon = "icon_feed",
                   Title = "Заказы",
                   ContentTemplate = new DataTemplate(typeof(OrdersPage))
               });
            TabBar
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
