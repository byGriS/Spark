using System.Windows.Controls;
using System.Windows;
using Service;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System;
using System.Threading;

namespace SparkControls {
  public partial class IndiBoolean : UserControl, Indicator {

    public event Helper.SendData OnToggleData;

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiBoolean() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams[0];
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiBoolean; } }
    WindowSetting Indicator.IndiSetting {
      get {
        WindowSetting windowSetting = new IndiBooleanSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    public double MinValue { get; set;}
    public double MaxValue { get; set; }

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
        if (dataParams[0].Value == 0)
          ellipse.Fill = new SolidColorBrush(Colors.Red);
        else
          ellipse.Fill = new SolidColorBrush(Colors.Green);
      }));
    }

    public void UpdateBindingValue() {
      if (dataParams.Count > 0) {
        ((Service.DataParam)DataContext).OnChangeValue -= Param_OnChangeValue;
        DataContext = dataParams[0];
        dataParams[0].OnChangeValue += Param_OnChangeValue;
      }
    }

    private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
      if (dataParams.Count > 0) {
        if (new Service.MessageToggle().ShowDialog() == System.Windows.Forms.DialogResult.OK) {
          if (dataParams[0].Value == 0)
            OnToggleData?.Invoke(dataParams[0].ID, 1.0f);
          else
            OnToggleData?.Invoke(dataParams[0].ID, 0.0f);
        }
      }
    }
  }
}
