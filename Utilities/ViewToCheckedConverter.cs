using System;
using System.Globalization;
using System.Windows.Data;

namespace Page_Navigation_App.Utilities
{
    public class ViewToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            // Kiểm tra xem kiểu của CurrentView có chứa tên ViewModel mong muốn không
            return value.GetType().Name.Contains(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}