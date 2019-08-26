using System;
using System.Globalization;
using System.Windows.Data;

namespace SparkControls {
  public class StringFormatConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      string format = "{0:0.";
      int countDot = System.Convert.ToInt32(parameter);
      for(int i = 0; i < countDot; i++) {
        format += "0";
      }
      format += "}";
      string result = String.Format(format, value);
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return value;
    }
  }
}
