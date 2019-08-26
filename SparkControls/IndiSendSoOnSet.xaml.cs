using Service;
using System.Windows;

namespace SparkControls {
  public partial class IndiSendSoOnSet : Window, WindowSetting {
    public IndiSendSoOnSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputs;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (((IndiSendSoOn)Indicator).Input != null)
          cbParam.SelectedItem = ((IndiSendSoOn)Indicator).Input;
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (cbParam.SelectedItem != null) {
          ((IndiSendSoOn)Indicator).Input = (Input)cbParam.SelectedItem;
          Indicator.UpdateBindingValue();
        }
      }
      this.Close();
    }
  }
}
