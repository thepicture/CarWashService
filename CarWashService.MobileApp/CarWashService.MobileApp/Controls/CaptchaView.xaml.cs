
using CarWashService.MobileApp.Models.Captcha;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CaptchaView : ContentView
    {
        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
                TextItems = value.ToCharArray()
                   .Select(c =>
                   {
                       return new CaptchaLetter
                       {
                           Letter = c.ToString()
                       };
                   });
            }
        }

        private IEnumerable<CaptchaLetter> textItems;
        public IEnumerable<CaptchaLetter> TextItems
        {
            get => textItems;
            set
            {
                textItems = value;
                OnPropertyChanged(nameof(TextItems));
            }
        }

        public CaptchaView()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}