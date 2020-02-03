using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Utilidades
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class ConvertStringToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = value as string;

            if(string.IsNullOrEmpty(res))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
