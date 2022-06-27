using System;
using System.Globalization;
using System.Windows.Data;

namespace SudokuClient.Converters
{
    public class NumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int num)
                return string.Empty;

            return num != 0 ? num.ToString() : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str || !int.TryParse(str, out int num))
                return 0;

            return num;
        }
    }
}
