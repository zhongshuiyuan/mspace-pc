using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Mmc.Mspace.UavModule.Converter
{
    public class StateColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int state = int.Parse(values[0].ToString());
            int isReported = int.Parse(values[1].ToString());

            if (isReported == 1)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x58, 0xc4, 0xca));
            }


            if (state == 1)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x66, 0x96, 0x33));
            }
            else if (state == 2)
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0xee, 0x8f, 0x8f));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0xcb, 0xd8, 0xe4));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
