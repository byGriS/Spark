using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Service {
  public class Themes {
    public static ImageSource MainBackGround(bool dark) {
      return dark ? 
        new BitmapImage(new Uri("pack://application:,,,/Spark;component/Resources/BackD.jpg")) : 
        new BitmapImage(new Uri("pack://application:,,,/Spark;component/Resources/Back.jpg"));
    }

    public static Brush MainTop(bool dark) {
      return dark ?
        new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Spark;component/Resources/topD.jpg")) } :
        new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Spark;component/Resources/top.jpg")) };
    }

    public static Brush MainMenu(bool dark) {
      return dark ?
        new SolidColorBrush(Color.FromRgb(47, 47, 47)) :
        null;        
    }

    public static Brush ForeGroundWhiteBlack(bool dark) {
      return dark ?
        new SolidColorBrush(Colors.White) :
        new SolidColorBrush(Colors.Black);
    }
  }
}
