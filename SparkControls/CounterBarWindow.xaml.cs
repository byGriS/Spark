using System.ComponentModel;
using System.Windows;

namespace SparkControls {
  public partial class CounterBarWindow : Window, INotifyPropertyChanged {
    public CounterBarWindow() {
      InitializeComponent();
      DataContext = this;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private string value = "0";
    public string Value {
      get { return value; }
      set { this.value = value;
        OnPropertyChanged("Value");
      }
    }
  }
}
