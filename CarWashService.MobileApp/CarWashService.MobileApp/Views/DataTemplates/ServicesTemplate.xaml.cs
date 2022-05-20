using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Views.DataTemplates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesTemplate : ContentView
    {
        public ServicesTemplate()
        {
            InitializeComponent();
        }

        private void OnToggle(object sender, System.EventArgs e)
        {
            if ((sender as Button).Text == "В корзину")
            {
                (sender as Button).Text = "Удалить из корзины";
                (sender as Button).BackgroundColor = Color.Red;
            }
            else
            {
                (sender as Button).Text = "В корзину";
                (sender as Button).BackgroundColor = (Color)App.Current
                    .Resources["Primary"];
            }
        }
    }
}