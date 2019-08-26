using OxyPlot;
using Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Spark {
  public partial class SettingGraphsWindow : Window {

    private Service.GraphSetting graphSetting = new Service.GraphSetting();
    private Service.GraphSetting graphSettingReturn = null;
    private SparkControls.IndiGraph indi = null;
    private Brush backUp = null;
    private ObservableCollection<DataParamGraphSetting> paramsList = new ObservableCollection<DataParamGraphSetting>();
    private ObservableCollection<CheckedParam> selectedParams = null;


    public SettingGraphsWindow(SparkControls.IndiGraph indi) {
      this.indi = indi;
      InitializeComponent();
      graphSetting = indi.GraphSetting.Clone();
      paramsList = new ObservableCollection<DataParamGraphSetting>();
      foreach(DataParam dataParam in indi.DataParams) {
        paramsList.Add(new DataParamGraphSetting {
          ID = dataParam.ID,
          Title = dataParam.Title,
          ColorLine = dataParam.ColorLine,
          IsRight = dataParam.IsRight
        });
      }
      graphParamsSet.ItemsSource = paramsList;
      InitGraphSets();
    }

    public SettingGraphsWindow(GraphSetting graphSetting) {
      InitializeComponent();
      this.graphSettingReturn = graphSetting;
      this.graphSetting = graphSetting.Clone();
      graphParamsSet.Visibility = Visibility.Collapsed;
      this.Height = 275;
      this.Title = "Настройки для графиков";
      InitGraphSets();
    }

    public SettingGraphsWindow(GraphSetting graphSetting, ObservableCollection<CheckedParam> selectedParams) {
      InitializeComponent();
      this.selectedParams = selectedParams;
      this.graphSettingReturn = graphSetting;
      this.graphSetting = graphSetting.Clone();
      paramsList = new ObservableCollection<DataParamGraphSetting>();
      foreach (CheckedParam selectedParam in selectedParams) {
        paramsList.Add(new DataParamGraphSetting {
          Title = selectedParam.Title,
          ColorLine = selectedParam.ColorLine,
          IsRight = selectedParam.IsRight,
          MinY = selectedParam.MinY,
          MaxY = selectedParam.MaxY
        });
      }
      graphParamsSet.ItemsSource = paramsList;
      InitGraphSets();
    }

    private void InitGraphSets() {
      backUp = bTL.Background;
      cbLegendSizeFont.ItemsSource = graphSetting.ListSizeFont;
      cbLegendSizeFont.SelectedValue = graphSetting.LegendSizeFont;
      cbAxisSizeFont.ItemsSource = graphSetting.ListSizeFont;
      cbAxisSizeFont.SelectedValue = graphSetting.AxisSizeFont;
      cbLineWidth.ItemsSource = graphSetting.ListWidthLine;
      cbLineWidth.SelectedValue = graphSetting.LineWidth;
      DataContext = graphSetting;
      switch (graphSetting.LegendPos) {
        case OxyPlot.LegendPosition.TopLeft:
          bTL.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.TopCenter:
          bTC.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.TopRight:
          bTR.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.LeftMiddle:
          bML.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.RightMiddle:
          bMR.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.BottomLeft:
          bBL.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.BottomCenter:
          bBC.Background = new SolidColorBrush(Colors.Red);
          break;
        case OxyPlot.LegendPosition.BottomRight:
          bBR.Background = new SolidColorBrush(Colors.Red);
          break;
      }
    }

    private void Pos_Click(object sender, RoutedEventArgs e) {
      Button b = (Button)sender;
      string nameB = b.Name;
      foreach(Button btn in gridPosLegend.Children) {
        btn.Background = backUp;
      }
      b.Background = new SolidColorBrush(Colors.Red);
      b.Focusable = false;
      switch (nameB) {
        case "bTL":
          graphSetting.LegendPos = OxyPlot.LegendPosition.TopLeft;
          break;
        case "bTC":
          graphSetting.LegendPos = OxyPlot.LegendPosition.TopCenter;
          break;
        case "bTR":
          graphSetting.LegendPos = OxyPlot.LegendPosition.TopRight;
          break;
        case "bML":
          graphSetting.LegendPos = OxyPlot.LegendPosition.LeftMiddle;
          break;
        case "bMR":
          graphSetting.LegendPos = OxyPlot.LegendPosition.RightMiddle;
          break;
        case "bBL":
          graphSetting.LegendPos = OxyPlot.LegendPosition.BottomLeft;
          break;
        case "bBC":
          graphSetting.LegendPos = OxyPlot.LegendPosition.BottomCenter;
          break;
        case "bBR":
          graphSetting.LegendPos = OxyPlot.LegendPosition.BottomRight;
          break;
      }
    }

    private void ColorChange_Click(object sender, RoutedEventArgs e) {
      if (graphParamsSet.SelectedItem == null)
        return;
      Button b = (Button)sender;
      System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
      cd.Color = System.Drawing.Color.FromArgb(((Color)b.Background.GetValue(SolidColorBrush.ColorProperty)).R,
         ((Color)b.Background.GetValue(SolidColorBrush.ColorProperty)).G,
         ((Color)b.Background.GetValue(SolidColorBrush.ColorProperty)).B);
      if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        DataParamGraphSetting dataParam = (DataParamGraphSetting)graphParamsSet.SelectedItem;
        dataParam.ColorLine = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
        b.Background = dataParam.ColorLine;
      }
    }

    private void chRight_Click(object sender, RoutedEventArgs e) {
      if (graphParamsSet.SelectedItem == null)
        return;
      CheckBox cb = (CheckBox)sender;
      ((DataParamGraphSetting)graphParamsSet.SelectedItem).IsRight = cb.IsChecked.Value;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void Apply_Click(object sender, RoutedEventArgs e) {
      if (indi != null) {
        for (int i = 0; i < indi.DataParams.Count; i++) {
          indi.DataParams[i].ColorLine = paramsList[i].ColorLine;
          indi.DataParams[i].IsRight = paramsList[i].IsRight;
        }
        indi.GraphSetting = graphSetting.Clone();
      }
      if (selectedParams != null) {
        for (int i = 0; i < paramsList.Count; i++) {
          selectedParams[i].ColorLine = paramsList[i].ColorLine;
          selectedParams[i].IsRight = paramsList[i].IsRight;
          selectedParams[i].MinY = paramsList[i].MinY;
          selectedParams[i].MaxY = paramsList[i].MaxY;
        }
      }
      if (graphSettingReturn != null)
        WriteGraphSettingReturn();
      Close();
    }

    private void WriteGraphSettingReturn() {
      graphSettingReturn.AxisSizeFont = graphSetting.AxisSizeFont;
      graphSettingReturn.LegendPos = graphSetting.LegendPos;
      graphSettingReturn.LegendSizeFont = graphSetting.LegendSizeFont;
      graphSettingReturn.LegendVis = graphSetting.LegendVis;
      graphSettingReturn.LineWidth = graphSetting.LineWidth;
    }
  }

  public class DataParamGraphSetting {
    public int ID { get; set; }
    public string Title { get; set; }
    public SolidColorBrush ColorLine { get; set; }
    public bool IsRight { get; set; }
    public double MinY { get; set; }
    public double MaxY { get; set; }
  }
}
