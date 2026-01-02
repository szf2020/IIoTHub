using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IIoTHub.App.Wpf.ViewModels.MultiValueConverter
{
    public class PercentageToDashArrayMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double percent &&
                values[1] is double width && values[2] is double thickness)
            {
                double radius = width / 2 - thickness / 2;
                double perimeter = 2 * Math.PI * radius / thickness;
                double step = percent * perimeter;
                return new DoubleCollection { step, 1000 };
            }
            return new DoubleCollection { 0, 100 };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
