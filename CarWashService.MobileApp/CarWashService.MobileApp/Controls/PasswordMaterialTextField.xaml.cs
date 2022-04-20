using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarWashService.MobileApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordMaterialTextField : ContentView
    {
        public static readonly BindableProperty PasswordProperty =
        BindableProperty.Create("Password",
                                typeof(string),
                                typeof(PasswordMaterialTextField),
                                default(string),
                                BindingMode.TwoWay, propertyChanged: OnPasswordChanged);

        private static void OnPasswordChanged(BindableObject bindable,
                                              object oldValue,
                                              object newValue)
        {
            if (newValue == oldValue) return;
            ((PasswordMaterialTextField)bindable).PasswordField.Text = (string)newValue;
            bindable.SetValue(PasswordProperty, newValue);
        }

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly BindableProperty IsPasswordVisibleProperty =
        BindableProperty.Create("IsPasswordVisible",
                                typeof(bool),
                                typeof(PasswordMaterialTextField),
                                default(bool),
                                BindingMode.TwoWay);

        public bool IsPasswordVisible
        {
            get => (bool)GetValue(IsPasswordVisibleProperty);
            set => SetValue(IsPasswordVisibleProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create("Placeholder",
                                typeof(string),
                                typeof(PasswordMaterialTextField),
                                default(string),
                                BindingMode.TwoWay);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty HelperTextProperty =
        BindableProperty.Create("HelperText",
                                typeof(string),
                                typeof(PasswordMaterialTextField),
                                default(string),
                                BindingMode.TwoWay);

        public string HelperText
        {
            get => (string)GetValue(HelperTextProperty);
            set => SetValue(HelperTextProperty, value);
        }

        public PasswordMaterialTextField()
        {
            InitializeComponent();
        }

        private void OnClicked(object sender, EventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}