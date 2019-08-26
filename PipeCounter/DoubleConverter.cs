using System;
using System.Globalization;
using System.Windows.Data;

namespace PipeCounter {
  public class DoubleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value.ToString() == "")
        return 0;
      try { return System.Convert.ToDouble(value.ToString().Replace('.', ',')); } catch {
        try {
          return System.Convert.ToDouble(value.ToString().Substring(0, value.ToString().Length - 1).Replace('.', ','));
        } catch (Exception e) {
          System.Windows.MessageBox.Show(e.Message, "Ошибка");
          return 0;
        }
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      if (value.ToString() == "")
        return 0;
      //return System.Convert.ToDouble(value.ToString().Replace('.', ','));
      try { return System.Convert.ToDouble(value.ToString().Replace('.', ',')); } catch {
        try {
          return System.Convert.ToDouble(value.ToString().Substring(0, value.ToString().Length - 1).Replace('.', ','));
        } catch {
          System.Windows.MessageBox.Show("Данные: " + value.ToString() + " не соответствуют формату ввода", "Ошибка");
          return 0;
        }
      }
    }
  }
}
