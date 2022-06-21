using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System;
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

            if (IsLoggedIn())
            {
                SetShellStacksDependingOnRole();
            }
            else
            {
                LoadWelcomePage();
            }
        }

        private void LoadWelcomePage()
        {
            TabBar.Items.Clear();
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(WelcomePage),
                Icon = "authorization",
                Title = "Добро пожаловать",
                ContentTemplate = new DataTemplate(typeof(WelcomePage)),
            });
        }

        public static void LoadLoginAndRegisterShell()
        {
            TabBar.Items.Clear();
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(LoginPage),
                Icon = "authorization",
                Title = "Авторизация",
                ContentTemplate = new DataTemplate(typeof(LoginPage)),
            });
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(LoginPage),
                Icon = "registration",
                Title = "Регистрация",
                ContentTemplate = new DataTemplate(typeof(RegisterPage))
            });
        }

        private bool IsLoggedIn()
        {
            return AppIdentity.User != null;
        }

        public static void SetShellStacksDependingOnRole()
        {
            TabBar.Items.Clear();
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(BranchesPage),
                Icon = "branch",
                Title = "Филиалы",
                ContentTemplate = new DataTemplate(typeof(BranchesPage)),
            });
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(ServicesPage),
                Icon = "logo",
                Title = "Услуги",
                ContentTemplate = new DataTemplate(typeof(ServicesPage))
            });
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(OrdersPage),
                Icon = "icon_feed",
                Title = "Заказы",
                ContentTemplate = new DataTemplate(typeof(OrdersPage))
            });
            TabBar.Items.Add(new ShellContent
            {
                Route = nameof(AccountPage),
                Icon = "profile",
                Title = "Профиль",
                ContentTemplate = new DataTemplate(typeof(AccountPage))
            });
            foreach (ShellSection tabBarItem in TabBar.Items)
            {
                tabBarItem.Disappearing += (o, e) =>
                {
                    _ = tabBarItem.Navigation.PopToRootAsync();
                };
            }
        }
    }
}
