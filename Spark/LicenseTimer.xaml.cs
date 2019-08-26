using System.ComponentModel;
using System.Windows;

namespace Spark {
  public partial class LicenseTimer : Window, INotifyPropertyChanged {

    private bool cancelClose = true;

    public LicenseTimer() {
      InitializeComponent();
      DataContext = this;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      e.Cancel = cancelClose;
    }

    public void CloseTimer() {
      cancelClose = false;
      this.Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      cancelClose = true;
    }

    private int counter = 60;
    public int Counter {
      get { return counter; }
      set {
        counter = value;
        OnPropertyChanged("Counter");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}