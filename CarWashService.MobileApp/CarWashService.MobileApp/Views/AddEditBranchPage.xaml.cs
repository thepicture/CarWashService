using CarWashService.MobileApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditBranchPage : ContentPage
    {
        public AddEditBranchPage(AddEditBranchViewModel addEditBranchViewModel)
        {
            InitializeComponent();
            BindingContext = addEditBranchViewModel;
        }
    }
}