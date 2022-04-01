using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDiscountPage : ContentPage
    {
        public AddDiscountPage()
        {
            InitializeComponent();
            BindingContext = new AddDiscountViewModel();
        }

        public AddDiscountPage(AddDiscountViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}