using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Converters
{
    public class BooleanInversionValueConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
