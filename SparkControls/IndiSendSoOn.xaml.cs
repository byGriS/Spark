using Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SparkControls {
  public partial class IndiSendSoOn : UserControl, Indicator {

    public event Helper.SendStart OnSendSoOn;

    public IndiSendSoOn() {
      InitializeComponent();
    }

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }

    public ObservableCollection<DataParam> DataParams { get { return new ObservableCollection<DataParam>(); } set { } }

    public WindowSetting IndiSetting {
      get {
        WindowSetting windowSetting = new IndiSendSoOnSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }



    public TypeIndicator TypeIndicator {
      get { return TypeIndicator.IndiSendSoOn; }
    }

    public void UpdateBindingValue() { }

    private Input input = null;
    public Input Input {
      get {
        return input;
      }
      set {
        input = value;
        if (input != null) {
          btn.IsEnabled = true;
        } else {
          btn.IsEnabled = false;
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Input == null) return;
      if (new Service.MessageQuestion("Отправить команду Пуск?", "Команда Пуск", MessageViewMode.Attention).ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        OnSendSoOn?.Invoke(((Service.InputCommon)this.Input).Title);
      }
    }
  }
}
