using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddServicePage : ContentPage
    {
        public AddServicePage()
        {
            InitializeComponent();
            BindingContext = new AddServiceViewModel();
        }

        public AddServicePage(AddServiceViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}