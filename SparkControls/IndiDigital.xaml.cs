using System.Windows.Controls;
using Service;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Text;
using System;
using System.Threading;

namespace SparkControls {
  public partial class IndiDigital : UserControl, Indicator {

    public event Helper.SendData OnSendData;

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiDigital() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams[0];
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiDigital; } }
    WindowSetting Indicator.IndiSetting {
      get {
        WindowSetting windowSetting = new IndiDigitalSet();
        windowSetting.Indicator = this;
        return windowSetting;
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
      }));
    }

    public void UpdateBindingValue() {
      if (dataParams.Count > 0) {
        ((DataParam)DataContext).OnChangeValue -= Param_OnChangeValue;
        DataContext = dataParams[0];
        dataParams[0].OnChangeValue += Param_OnChangeValue;
      }
      Binding bind = new Binding("Value");
      bind.Source = this.DataContext;
      bind.Converter = new StringFormatConverter();
      bind.ConverterParameter = CountDot;
      this.lValue.SetBinding(Label.ContentProperty, bind);
    }

    private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
      if (dataParams.Count > 0) {
        StringBuilder newValue = new StringBuilder();
        if (new Service.MessageInput("Введите новое значение", dataParams[0].Title, newValue, true).ShowDialog() == System.Windows.Forms.DialogResult.OK) {
          OnSendData?.Invoke(dataParams[0].ID, Convert.ToSingle(newValue.ToString().Replace('.',',')));
        }
      }
    }
  }
}
