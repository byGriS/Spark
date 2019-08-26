using Service;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SparkControls {
  public partial class IndiArrow : UserControl, Indicator {

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiArrow() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams[0];
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiArrow; } }
    public WindowSetting IndiSetting {
      get {
        WindowSetting windowSetting = new IndiArrowSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    private double minValue = 0.0;
    public double MinValue {
      get { return minValue; }
      set { minValue = value;
        lScaleMin.Content = minValue.ToString();
      }
    }

    private double maxValue = 100.0;
    public double MaxValue {
      get { return maxValue; }
      set {
        maxValue = value;
        lScaleMax.Content = maxValue.ToString();
      }
    }

    private void DataParams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
        if (dataParams.Count > 0)
          dataParams[0].OnChangeValue -= Param_OnChangeValue;
        DataContext = dataParams[0];
        dataParams[0].OnChangeValue += Param_OnChangeValue;
      }
    }

    private void Param_OnChangeValue(DataParam dataParam) {
      this.Dispatcher.Invoke(new ThreadStart(delegate {
        if ((dataParams[0].AlarmMax != 0) || (dataParams[0].AlarmMin != 0)) {
        this.lValue.Foreground = new SolidColorBrush(Helper.Text);
        if (dataParams[0].Value < dataParams[0].AlarmMin) {
          this.lValue.Foreground = new SolidColorBrush(Helper.MinColor);
        }
        if (dataParams[0].Value > dataParams[0].AlarmMax) {
          this.lValue.Foreground = new SolidColorBrush(Helper.MaxColor);
        }
      }

      path2.Fill = new SolidColorBrush(Colors.Green);
      this.lValue.Foreground = new SolidColorBrush(Helper.Text);
      if (dataParams[0].Value < dataParams[0].AlarmMin) {
        path2.Fill = new SolidColorBrush(Helper.MinColor);
        this.lValue.Foreground = new SolidColorBrush(Helper.MinColor);
      }
      if (dataParams[0].Value > dataParams[0].AlarmMax) {
        path2.Fill = new SolidColorBrush(Helper.MaxColor);
        this.lValue.Foreground = new SolidColorBrush(Helper.MaxColor);
      }

      double angle = -180;
      if ((MaxValue > 0) && (dataParams[0] != null)) {
        double division = 180 / MaxValue;
        if (dataParams[0].Value < MaxValue)
          angle = -180 + dataParams[0].Value * division;
        else
          angle = 0;
        RotateTransform rotate = new RotateTransform(angle);
        if (pathGeom2 != null)
          pathGeom2.Transform = rotate;
      }
      }));
    }

    public void UpdateBindingValue() {
      if (dataParams.Count > 0) {
        ((Service.DataParam)DataContext).OnChangeValue -= Param_OnChangeValue;
        DataContext = dataParams[0];
        dataParams[0].OnChangeValue += Param_OnChangeValue;
      }
      Binding bind = new Binding("Value");
      bind.Source = this.DataContext;
      bind.Converter = new StringFormatConverter();
      bind.ConverterParameter = CountDot;
      this.lValue.SetBinding(Label.ContentProperty, bind);
    }
    
    PathGeometry pathGeom2;
    Path path2 = new Path();

    private void Draw() {
      canvas.Children.Clear();
      Path path = new Path();
      double maxValue = (canvas.ActualWidth > canvas.ActualHeight) ? canvas.ActualWidth : canvas.ActualHeight;
      path.Stroke = new SolidColorBrush(Helper.Text);
      path.StrokeThickness = 1;
      CombinedGeometry combinedG = new CombinedGeometry();
      combinedG.GeometryCombineMode = GeometryCombineMode.Exclude;
      GeometryGroup gGroup = new GeometryGroup();
      gGroup.FillRule = FillRule.EvenOdd;
      gGroup.Children.Add(new EllipseGeometry() {
        RadiusX = canvas.ActualWidth / 2,
        RadiusY = canvas.ActualHeight
      });
      gGroup.Children.Add(new EllipseGeometry() {
        RadiusX = 0,
        RadiusY = 0
      });
      combinedG.Geometry1 = gGroup;
      GeometryGroup gGroup2 = new GeometryGroup();
      gGroup2.FillRule = FillRule.Nonzero;
      PathGeometry pathGeom = new PathGeometry();
      PathFigure pathFig = new PathFigure();
      pathFig.IsClosed = true;
      pathFig.IsFilled = true;
      pathFig.StartPoint = new Point(-100, 0);
      pathFig.Segments.Add(new LineSegment() { Point = new Point(maxValue, 0) });
      pathFig.Segments.Add(new LineSegment() { Point = new Point(maxValue, maxValue) });
      pathFig.Segments.Add(new LineSegment() { Point = new Point(0 - maxValue, maxValue) });
      pathFig.Segments.Add(new LineSegment() { Point = new Point(0 - maxValue, 0) });
      pathGeom.Figures.Add(pathFig);
      gGroup2.Children.Add(pathGeom);
      combinedG.Geometry2 = gGroup2;
      path.Data = combinedG;
      TranslateTransform ttPath = new TranslateTransform() { X = canvas.ActualWidth / 2, Y = canvas.ActualHeight };
      path.RenderTransform = ttPath;
      canvas.Children.Add(path);

      path2.Data = path.Data.Clone();
      path2.Stroke = new SolidColorBrush(Colors.Black);
      path2.StrokeThickness = 1;
      path2.Fill = new SolidColorBrush(Colors.Green);
      TranslateTransform ttPath2 = new TranslateTransform() { X = canvas.ActualWidth / 2, Y = canvas.ActualHeight };
      path2.RenderTransform = ttPath2;
      pathGeom2 = (PathGeometry)((GeometryGroup)((CombinedGeometry)path2.Data).Geometry2).Children[0].Clone();
      double angle = -180;
      if ((MaxValue > 0) && (dataParams.Count > 0)) {
        double division = 180 / MaxValue;
        if (dataParams[0].Value < MaxValue)
          angle = -180 + dataParams[0].Value * division;
        RotateTransform rotate = new RotateTransform(angle);
        if (pathGeom2 != null)
          pathGeom2.Transform = rotate;
      }
      RotateTransform rotateVolume = new RotateTransform() { Angle = angle };
      pathGeom2.Transform = rotateVolume;
      ((GeometryGroup)((CombinedGeometry)path2.Data).Geometry2).Children.Add(pathGeom2);
      canvas.Children.Add(path2);

    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
      Draw();
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
      Draw();
    }
  }
}
