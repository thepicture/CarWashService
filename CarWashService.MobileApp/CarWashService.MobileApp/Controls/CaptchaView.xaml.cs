
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
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text",
                                    typeof(string),
                                    typeof(CaptchaView),
                                    default(string),
                                    BindingMode.TwoWay,
                                    propertyChanged: OnTextChanged);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(BindableObject bindable,
                                              object oldValue,
                                              object newValue)
        {
            if (newValue == oldValue) return;
            IEnumerable<CaptchaLetter> textSource = ((string)newValue)
                .ToCharArray()
                .Select(c => new CaptchaLetter
                {
                    Letter = c.ToString()
                });
            BindableLayout.SetItemsSource(((CaptchaView)bindable).TextLayout,
                                          textSource);
            bindable.SetValue(TextProperty, newValue);
        }

        public CaptchaView()
        {
            InitializeComponent();
        }
    }
}