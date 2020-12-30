using System;
using System.Globalization;
using System.Windows.Data;

namespace Mmc.Mspace.UavModule.Converter
{
    public class LockUavSaftyBtnTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "解锁" : "锁定";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

