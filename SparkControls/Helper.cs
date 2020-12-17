using System.Windows;
using System.Windows.Media;

namespace SparkControls {
  public class Helper {
    public delegate void SendData(int idDataParam, float newValue);
    public delegate void SendStart(string titleInput);

    private static Color minColor = Color.FromRgb(136, 91, 0);
    public static Color MinColor { get { return minColor; } set { minColor = value; } }

    private static Color maxColor = Color.FromRgb(140, 0, 0);
    public static Color MaxColor { get { return maxColor; } set { maxColor = value; } }

    public static Color Text { get { return Color.FromRgb(16, 16, 16); } }
    //(Style)FindResource("FormLabelStyle");
    //public static Color Text { get { return (Style)FindResource("LabelStyle")} }
    public static Color Column { get { return Color.FromRgb(10, 78, 129); } }

    public static SolidColorBrush[] ListColor = new SolidColorBrush[] {
      new SolidColorBrush(Color.FromRgb(0, 0, 255)),
      new SolidColorBrush(Color.FromRgb(255, 0, 0)),
      new SolidColorBrush(Color.FromRgb(0, 255, 0)),
      new SolidColorBrush(Color.FromRgb(114, 219, 157)),
      new SolidColorBrush(Color.FromRgb(157, 232, 149)),
      new SolidColorBrush(Color.FromRgb(229, 196, 148)),
      new SolidColorBrush(Color.FromRgb(142, 131, 30)),
      new SolidColorBrush(Color.FromRgb(129, 76, 179)),
      new SolidColorBrush(Color.FromRgb(57, 228, 185)),
      new SolidColorBrush(Color.FromRgb(30, 236, 113)),
      new SolidColorBrush(Color.FromRgb(166, 104, 50)),
      new SolidColorBrush(Color.FromRgb(240, 140, 67)),
      new SolidColorBrush(Color.FromRgb(81, 128, 151)),
      new SolidColorBrush(Color.FromRgb(87, 152, 191)),
      new SolidColorBrush(Color.FromRgb(175, 187, 58)),
      new SolidColorBrush(Color.FromRgb(126, 226, 193)),
      new SolidColorBrush(Color.FromRgb(98, 92, 11)),
      new SolidColorBrush(Color.FromRgb(3, 220, 98)),
      new SolidColorBrush(Color.FromRgb(28, 137, 184)),
      new SolidColorBrush(Color.FromRgb(71, 44, 157)),
      new SolidColorBrush(Color.FromRgb(42, 83, 238)),
      new SolidColorBrush(Color.FromRgb(43, 245, 127)),
      new SolidColorBrush(Color.FromRgb(99, 244, 55)),
      new SolidColorBrush(Color.FromRgb(80, 139, 92)),
      new SolidColorBrush(Color.FromRgb(78, 54, 234)),
      new SolidColorBrush(Color.FromRgb(93, 208, 143)),
      new SolidColorBrush(Color.FromRgb(120, 185, 2)),
      new SolidColorBrush(Color.FromRgb(246, 187, 143)),
      new SolidColorBrush(Color.FromRgb(101, 163, 138)),
      new SolidColorBrush(Color.FromRgb(206, 212, 3)),
      new SolidColorBrush(Color.FromRgb(137, 9, 83)),
      new SolidColorBrush(Color.FromRgb(248, 44, 68)),
      new SolidColorBrush(Color.FromRgb(208, 132, 197)),
      new SolidColorBrush(Color.FromRgb(185, 27, 236)),
      new SolidColorBrush(Color.FromRgb(138, 17, 208)),
      new SolidColorBrush(Color.FromRgb(174, 180, 166)),
      new SolidColorBrush(Color.FromRgb(119, 189, 183)),
      new SolidColorBrush(Color.FromRgb(40, 205, 113)),
      new SolidColorBrush(Color.FromRgb(155, 50, 150)),
      new SolidColorBrush(Color.FromRgb(94, 196, 82)),
      new SolidColorBrush(Color.FromRgb(244, 147, 32)),
      new SolidColorBrush(Color.FromRgb(52, 197, 129)),
      new SolidColorBrush(Color.FromRgb(8, 84, 10))
    };
  }
}
