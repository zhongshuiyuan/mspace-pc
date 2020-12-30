using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Mmc.Mspace.UavModule.Converter
{
    public class StatusVisibilityConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int status = int.Parse(value.ToString());

            if (parameter != null)
            {
                if (parameter.ToString() == "Approval")
                {
                    if (status == 0)
                        return Visibility.Visible;

                    return Visibility.Collapsed;
                }
            }

            if (status == 2)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
