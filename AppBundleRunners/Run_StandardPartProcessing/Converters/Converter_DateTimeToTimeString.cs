using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Run_StandardPartProcessing.Converters
{
    public class Converter_DateTimeToTimeString : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime tmp;

            if (!(value is DateTime))
            {
                return "N/A";
            }

            tmp = (DateTime)value;

            return tmp.ToLongTimeString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
