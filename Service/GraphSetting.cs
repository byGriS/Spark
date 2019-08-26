using OxyPlot;
using System.Collections.ObjectModel;

namespace Service {
  public class GraphSetting {
    public readonly ObservableCollection<int> ListSizeFont = new ObservableCollection<int> { 6, 8, 10, 12, 14, 16, 18, 20 };
    public readonly ObservableCollection<float> ListWidthLine = new ObservableCollection<float> { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 5.0f };

    private bool legendVis = true;
    public bool LegendVis { get { return legendVis; } set { legendVis = value; } }

    private LegendPosition legendPos = LegendPosition.TopRight;
    public LegendPosition LegendPos { get { return legendPos; } set { legendPos = value; } }

    private int legendSizeFont = 10;
    public int LegendSizeFont { get { return legendSizeFont; } set { legendSizeFont = value; } }

    private float lineWidth = 1.0f;
    public float LineWidth { get { return lineWidth; } set { lineWidth = value; } }

    private int axisSizeFont = 10;
    public int AxisSizeFont { get { return axisSizeFont; } set { axisSizeFont = value; } }

    public GraphSetting Clone() {
      GraphSetting gs = new GraphSetting();
      gs.LegendVis = this.LegendVis;
      gs.LegendSizeFont = this.LegendSizeFont;
      gs.LineWidth = this.LineWidth;
      gs.AxisSizeFont = this.AxisSizeFont;
      gs.LegendPos = this.LegendPos;
      return gs;
    }
  }
}
