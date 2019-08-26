using Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SparkControls {
  public partial class IndiInputStatus : UserControl, Indicator, INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }

    private Timer timerData = new Timer();


    public IndiInputStatus() {
      InitializeComponent();
      this.DataContext = this;
      timerData.Elapsed += TimerData_Elapsed;
    }

    private void TimerData_Elapsed(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
        ellipse.Fill = new SolidColorBrush(Colors.Yellow);
        timerData.Stop();
      }));
    }

    public ObservableCollection<DataParam> DataParams { get { return new ObservableCollection<DataParam>(); } set { } }

    public WindowSetting IndiSetting {
      get {
        WindowSetting windowSetting = new IndiInputStatusSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    public TypeIndicator TypeIndicator {
      get { return TypeIndicator.IndiSendStart; }
    }

    public void UpdateBindingValue() {

    }

    public double TimerInterval {
      get { return timerData.Interval/1000.0; }
      set { timerData.Interval = value*1000.0; }
    }

    private Input input = null;
    public Input Input {
      get {
        return input;
      }
      set {
        input = value;
        if (input != null)
          IndiTitle = ((InputCommon)input).Title;
      }
    }

    private string indiTitle = "Состояние входа";
    public string IndiTitle {
      get {
        return indiTitle;
      }
      set {
        indiTitle = value;
        OnPropertyChanged("IndiTitle");
      }
    }


    public void InputDataUpdate() {
      this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
        timerData.Stop();
        ellipse.Fill = new SolidColorBrush(Colors.Green);
        timerData.Start();
      }));
    }
  }
}
