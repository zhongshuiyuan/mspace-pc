using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MMC.MSpace.Converter
{
    public class ConverterPngToGif : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string png = value.ToString();
            var indexF = png.LastIndexOf("/");
            var index = png.LastIndexOf(".");
            string str1 = png.Substring(0, indexF);
            string str2 = png.Substring(indexF, index - indexF);
            string str = $"{str1}/BarMenu{str2}.gif";

            return new BitmapImage(new Uri(str));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
