using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MITM_UI.View.Resources.Converters
{
    public class BoolToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isConnected = (bool)value;

            if (isConnected)
            {
                return new Uri(Directory.GetCurrentDirectory() +  "/../../View/Resources/Images/connected.png");
            }
            else
            {
                return new Uri(Directory.GetCurrentDirectory() + "/../../View/Resources/Images/notconnected.png");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
