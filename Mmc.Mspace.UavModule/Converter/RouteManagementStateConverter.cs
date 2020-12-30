using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mmc.Mspace.UavModule.Converter
{
    public class RouteManagementStateConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            int state = int.Parse(value[0].ToString());
            int isReported = int.Parse(value[1].ToString());
            if (isReported == 1)
            {
                return "已报备";
            }
            

            if (state == 1)
            {
                return "已通过";
            }
            else if (state == 2)
            {
                return "未通过";
            }
            else
            {
                return "待审核";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
