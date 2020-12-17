using System.Windows;
using System.Windows.Controls;
using Service;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Threading;

namespace SparkControls {
  public partial class IndiColumn : UserControl, Indicator {

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiColumn() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams[0];
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiColumn; } }
    public WindowSetting IndiSetting {
      get {
        WindowSetting windowSetting = new IndiColumnSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    private double minValue = 0.0;
    public double MinValue {
      get { return minValue; }
      set {
        minValue = value;
        lScaleMin.Content = minValue.ToString();
        lScaleMid.Content = (maxValue + minValue) / 2.0;
      }
    }

    private double maxValue = 100.0;
    public double MaxValue {
      get { return maxValue; }
      set {
        maxValue = value;
        lScaleMax.Content = maxValue.ToString();
        lScaleMid.Content = (maxValue + minValue) / 2.0;
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
        canvasColumn.Children.Clear();
        Rectangle rect = new Rectangle();
        rect.Width = canvasColumn.ActualWidth;
        double resultHeight = (dataParams[0].Value-MinValue) * canvasColumn.ActualHeight / (MaxValue-MinValue);
        
        rect.Height = (resultHeight >= 0) ? resultHeight : 0;
        if (rect.Height > canvasColumn.ActualHeight)
          rect.Height = canvasColumn.ActualHeight + 10;
        rect.Fill = new SolidColorBrush(Colors.Blue);
        this.lValue.Foreground = (Brush)((Setter)(((Style)FindResource("LabelStyle")).Setters[0])).Value;

        if ((dataParams[0].AlarmMax != 0) || (dataParams[0].AlarmMin != 0)) {
          if (dataParams[0].Value < dataParams[0].AlarmMin) {
            rect.Fill = new SolidColorBrush(Helper.MinColor);
            this.lValue.Foreground = new SolidColorBrush(Helper.MinColor);
          }
          if (dataParams[0].Value > dataParams[0].AlarmMax) {
            rect.Fill = new SolidColorBrush(Helper.MaxColor);
            this.lValue.Foreground = new SolidColorBrush(Helper.MaxColor);
          }
        }

        canvasColumn.Children.Add(rect);
        Canvas.SetTop(rect, canvasColumn.ActualHeight - rect.Height);
        Canvas.SetLeft(rect, 0);
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
  }
}
