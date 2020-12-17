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
      set {
        minValue = value;
        //lScaleMin.Content = minValue.ToString();
      }
    }

    private double maxValue = 100.0;
    public double MaxValue {
      get { return maxValue; }
      set {
        maxValue = value;
        //lScaleMax.Content = maxValue.ToString();
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
        double angle = -135;
        if ((MaxValue > 0) && (dataParams[0] != null)) {
          double division = 270 / (MaxValue - MinValue);
          if (Math.Abs(dataParams[0].Value) < MaxValue)
            angle = -135 + (Math.Abs(dataParams[0].Value) - MinValue) * division;
          else
            angle = 0;
          if (angle < -135) angle = -135;
          if (angle > 135) angle = 135;
          RotateTransform rotate = new RotateTransform(angle) { CenterX = 73, CenterY = 59 };
          TranslateTransform toCenter = new TranslateTransform(73, 59);
          TransformGroup transformGroup = new TransformGroup();
          transformGroup.Children.Add(toCenter);
          transformGroup.Children.Add(rotate);
          arrow.RenderTransform = transformGroup;
        }
      }));
    }

    public void UpdateBindingValue() {
      if (dataParams.Count > 0) {
        ((Service.DataParam)DataContext).OnChangeValue -= Param_OnChangeValue;
        DataContext = dataParams[0];
        dataParams[0].OnChangeValue += Param_OnChangeValue;
        

        double valueSector = (MaxValue - MinValue) / 6.0;

        if (((Service.DataParam)DataContext).AlarmMin > MinValue && ((Service.DataParam)DataContext).AlarmMin < MaxValue) {
          int countSectorsMin = (int)Math.Ceiling(((Service.DataParam)DataContext).AlarmMin / valueSector);
          for (int i = 0; i < countSectorsMin; i++) {
            if (i == countSectorsMin - 1) {
              double angle = 270.0 * ((Service.DataParam)DataContext).AlarmMin / (MaxValue - MinValue) + 135.0;
              double x = 120.0 * Math.Cos(angle * Math.PI / 180.0);
              double y = 120.0 * Math.Sin(angle * Math.PI / 180.0);
              minZone.Segments.Add(new LineSegment { Point = new Point(x, y) });
            } else {
              if (i == 0)
                minZone.Segments.Add(new LineSegment { Point = new Point(-120, 0) });
              if (i == 1)
                minZone.Segments.Add(new LineSegment { Point = new Point(-120, -120) });
              if (i == 2)
                minZone.Segments.Add(new LineSegment { Point = new Point(0, -120) });
              if (i == 3)
                minZone.Segments.Add(new LineSegment { Point = new Point(120, -120) });
              if (i == 4)
                minZone.Segments.Add(new LineSegment { Point = new Point(120, 0) });
            }
          }
        }else {
          minZone.Segments.Clear();
          minZone.Segments.Add(new LineSegment { Point = new Point(-106, 106) });
        }

        if (((Service.DataParam)DataContext).AlarmMax < MaxValue  && ((Service.DataParam)DataContext).AlarmMax > MinValue) {
          int countSectorMax = 6 - (int)Math.Ceiling(((Service.DataParam)DataContext).AlarmMax / valueSector);
          for (int i = 0; i < countSectorMax; i++) {
            if (i == countSectorMax - 1) {
              double angle = -1.0 * 270.0 * (MaxValue - ((Service.DataParam)DataContext).AlarmMax) / (MaxValue - MinValue) + 45.0;
              double x = 120.0 * Math.Cos(angle * Math.PI / 180.0);
              double y = 120.0 * Math.Sin(angle * Math.PI / 180.0);
              maxZone.Segments.Add(new LineSegment { Point = new Point(x, y) });
            } else {
              if (i == 0)
                maxZone.Segments.Add(new LineSegment { Point = new Point(120, 0) });
              if (i == 1)
                maxZone.Segments.Add(new LineSegment { Point = new Point(120, -120) });
              if (i == 2)
                maxZone.Segments.Add(new LineSegment { Point = new Point(0, -120) });
              if (i == 3)
                maxZone.Segments.Add(new LineSegment { Point = new Point(-120, -120) });
              if (i == 4)
                maxZone.Segments.Add(new LineSegment { Point = new Point(-120, 0) });
            }
          }
        } else {
          maxZone.Segments.Clear();
          maxZone.Segments.Add(new LineSegment { Point = new Point(106, 106) });
        }
        l0.Content = MinValue;
        l100.Content = MaxValue;
        l25.Content = (MaxValue - MinValue) / 4;
        l50.Content = (MaxValue - MinValue) / 4 * 2;
        l75.Content = (MaxValue - MinValue) / 4 * 3;
      }
      Binding bind = new Binding("Value");
      bind.Source = this.DataContext;
      bind.Converter = new StringFormatConverter();
      bind.ConverterParameter = CountDot;
      //this.lValue.SetBinding(Label.ContentProperty, bind);

    }

    PathGeometry pathGeom2;
    Path path2 = new Path();

    private void Draw() {
      /*canvas.Children.Clear();
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
      //canvas.Children.Add(path);

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
      ((GeometryGroup)((CombinedGeometry)path2.Data).Geometry2).Children.Add(pathGeom2);*/
     // canvas.Children.Add(path2);

    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
      //Draw();
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
      //Draw();
    }
  }
}
