using CarWashService.MobileApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}