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
    [ValueConversion (typeof(bool), typeof(Visibility))]
    public class ConvertBooleanToVisibilityCollapsed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? res = value as bool?;
            if(res != null && res.HasValue)
            {
                if(res.Value)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility res = (Visibility)value;
            return res == Visibility.Visible ? true : false;
        }
    }

    [ValueConversion(typeof(bool), typeof(int))]
    public class ConvertBooleanToBlurRadius : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? res = value as bool?;
            if (res != null && res.HasValue)
            {
                if (res.Value)
                {
                    return 0;
                }
                else
                {
                    return 10;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility res = (Visibility)value;
            return res == Visibility.Visible ? true : false;
        }
    }

    public class ConvertBooleanToColorBackgraund : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? res = value as bool?;
            if (res != null && res.HasValue)
            {
                if(res.Value)
                {
                    return new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }

            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ConvertIntToColorBackgraund : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? res = value as int?;
            if (res != null && res.HasValue)
            {
                if (res.Value<=0)
                {
                    return new SolidColorBrush(Colors.LightGreen);
                }
                else if(res.Value<=30)
                {
                    return new SolidColorBrush(Colors.LightSalmon);
                }
                else if(res.Value<=60)
                {
                    return new SolidColorBrush(Colors.IndianRed);
                }
                else
                {
                    return new SolidColorBrush(Color.FromRgb(217, 1, 21));
                }
            }

            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
