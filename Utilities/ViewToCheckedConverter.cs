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

            // Lấy tên kiểu dữ liệu của View hiện tại
            string currentViewName = value.GetType().Name;
            string targetViewName = parameter.ToString();

            // Kiểm tra xem View hiện tại có chứa chuỗi định danh mong muốn không
            // Ví dụ: HomeViewModel có chứa "Home"
            return currentViewName.Contains(targetViewName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}