using Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Threading;

namespace SparkControls {
  public partial class IndiGraph : UserControl, Indicator {

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public PlotModel Model { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiGraph() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams;
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiGraph; } }
    WindowSetting Indicator.IndiSetting {
      get {
        WindowSetting windowSetting = new IndiGraphSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    private void DataParams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
        foreach (DataParam dataParam in dataParams) {
          dataParam.OnChangeValue -= Param_OnChangeValue;
          dataParam.OnChangeValue += Param_OnChangeValue;
        }
        DataContext = dataParams;
        InitGraph();
        InitGraphSetting();
      }
    }

    private void InitGraph() {
      Model = new PlotModel();

      Model.Background = OxyColors.Black;
      DateTimeAxis axisX = new DateTimeAxis() {
        Title = "Время",
        Position = AxisPosition.Bottom,
        StringFormat = "HH:mm:ss"
      };
      axisX.MajorGridlineStyle = LineStyle.Solid;
      axisX.MajorGridlineThickness = 1;
      Model.Axes.Add(axisX);

      int pos = 0;
      foreach (DataParam param in DataParams) {
        LinearAxis axisY = new LinearAxis() {
          Title = param.Title + ", " + param.ParamUnit.Title,
          Key = param.Title,
          Position = AxisPosition.Left,
          PositionTier = pos++
        };
        if (pos == 1) {
          axisY.AxisChanged += AxeY_AxisChanged;
          axisY.MajorGridlineStyle = LineStyle.Solid;
          axisY.MajorGridlineThickness = 1;
        }
        Model.Axes.Add(axisY);
        LineSeries series = new LineSeries() {
          Title = param.Title,
          YAxisKey = param.Title
        };
        Model.Series.Add(series);
      }

      var controller = new PlotController();
      controller.UnbindAll();
      controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
      controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, PlotCommands.ZoomRectangle);
      controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Alt, PlotCommands.PointsOnlyTrack);
      controller.BindMouseWheel(PlotCommands.ZoomWheel);
      controller.BindKeyDown(OxyKey.R, PlotCommands.Reset);
      plotter.Controller = controller;

      plotter.Model = Model;
    }

    private void AxeY_AxisChanged(object sender, AxisChangedEventArgs e) {
      Axis ax = sender as Axis;
      if (e.ChangeType == AxisChangeTypes.Pan) {
        double delta = ax.ActualMaximum - ax.ActualMinimum;
        foreach (Axis axis in plotter.ActualModel.Axes) {
          if ((axis.GetType() != typeof(DateTimeAxis)) && (axis != ax)) {
            axis.Pan(e.DeltaMaximum * (axis.ActualMaximum - axis.ActualMinimum) / delta * axis.Scale * -1);
          }
        }
      }
      if (e.ChangeType == AxisChangeTypes.Zoom) {
        double delta = (ax.ActualMaximum - e.DeltaMaximum) - (ax.ActualMinimum - e.DeltaMinimum);
        foreach (Axis axis in plotter.ActualModel.Axes) {
          if ((axis.GetType() != typeof(DateTimeAxis)) && (axis != ax)) {
            double z = e.DeltaMaximum * (axis.ActualMaximum - axis.ActualMinimum) / delta;
            double x = e.DeltaMinimum * (axis.ActualMaximum - axis.ActualMinimum) / delta;
            axis.Zoom(axis.ActualMinimum + x, axis.ActualMaximum + z);
          }
        }
      }
    }

    private void InitGraphSetting() {
      Model.IsLegendVisible = graphSetting.LegendVis;
      Model.LegendPosition = graphSetting.LegendPos;
      Model.LegendFontSize = graphSetting.LegendSizeFont;
      int indexDataParam = 0;
      foreach (LineSeries series in Model.Series) {
        series.StrokeThickness = graphSetting.LineWidth;
        series.Color = OxyColor.FromRgb(dataParams[indexDataParam].ColorLine.Color.R, dataParams[indexDataParam].ColorLine.Color.G, dataParams[indexDataParam].ColorLine.Color.B);
        indexDataParam++;
      }
      indexDataParam = 0;
      for (int i = 0; i < Model.Axes.Count; i++) {
        Model.Axes[i].TitleFontSize = graphSetting.AxisSizeFont;
        if (!Model.Axes[i].IsHorizontal())
          if (indexDataParam < DataParams.Count) {
            if (DataParams[indexDataParam++].IsRight)
              Model.Axes[i].Position = AxisPosition.Right;
            else
              Model.Axes[i].Position = AxisPosition.Left;
          }
      }
      plotter.InvalidatePlot(false);
    }

    private void Param_OnChangeValue(DataParam dataParam) {
      this.Dispatcher.Invoke(new ThreadStart(delegate {
        foreach (LineSeries line in plotter.Model.Series) {
        if (line.Title == dataParam.Title) {
          if (line.Points.Count > CountDot)
            line.Points.RemoveAt(0);
          line.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), dataParam.Value));
          plotter.InvalidatePlot(true);
        }
      }
      }));
    }

    public void UpdateBindingValue() {
      if (DataContext != null) {
        foreach (DataParam param in (ObservableCollection<DataParam>)DataContext) {
          param.OnChangeValue -= Param_OnChangeValue;
        }
        foreach (DataParam dataParam in dataParams) {
          dataParam.OnChangeValue -= Param_OnChangeValue;
          dataParam.OnChangeValue += Param_OnChangeValue;
        }
        DataContext = dataParams;
      }
    }

    private GraphSetting graphSetting = new GraphSetting();
    public GraphSetting GraphSetting {
      get { return graphSetting; }
      set {
        graphSetting = value;
        InitGraphSetting();
      }
    }
  }
}