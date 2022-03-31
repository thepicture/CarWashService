using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Converters
{
    public class RoleToGreatingValueConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            return "Вы "
                + ((string)value).ToLower();
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
