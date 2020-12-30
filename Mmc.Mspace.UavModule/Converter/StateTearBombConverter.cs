using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Mmc.Mspace.UavModule.Converter
{
    public class StateTearBombConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                return (int)value == 1 ? new SolidColorBrush(Color.FromArgb(0xff, 0xee, 0x8f, 0x8f)) : new SolidColorBrush(Color.FromArgb(0xff, 0x94, 0xd8, 0x55));
            }

            return (int)value == 1 ? "已发射" : "已准备";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
