
using CarWashService.MobileApp.Services;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesPage : ContentPage
    {
        private readonly ServicesViewModel _viewModel;
        public ServicesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ServicesViewModel(isForOrder: false);
            if (AppIdentity.User.UserTypeName == "Клиент")
            {
                ToolbarItem itemToRemove = ToolbarItems
                    .First(t => t.Text == "Добавить");
                _ = ToolbarItems.Remove(itemToRemove);
            }
            if (AppIdentity.User.UserTypeName == "Сотрудник" ||
                AppIdentity.User.UserTypeName == "Администратор")
            {
                ToolbarItem itemToRemove = ToolbarItems
                    .First(t => t.Text == "Оформить выбранное");
                _ = ToolbarItems.Remove(itemToRemove);
            }
        }

        public ServicesPage(ServicesViewModel servicesViewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = servicesViewModel;
            if (AppIdentity.User.UserTypeName == "Клиент")
            {
                ToolbarItem itemToRemove = ToolbarItems
                    .First(t => t.Text == "Добавить");
                _ = ToolbarItems.Remove(itemToRemove);
            }
            if (AppIdentity.User.UserTypeName == "Сотрудник" ||
                AppIdentity.User.UserTypeName == "Администратор")
            {
                ToolbarItem itemToRemove = ToolbarItems
                    .First(t => t.Text == "Оформить выбранное");
                _ = ToolbarItems.Remove(itemToRemove);
            }
        }

        protected override void OnAppearing()
        {
            _viewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}