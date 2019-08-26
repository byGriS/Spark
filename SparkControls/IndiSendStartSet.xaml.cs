using Service;
using System.Windows;

namespace SparkControls {
  public partial class IndiSendStartSet : Window, WindowSetting {
    public IndiSendStartSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputs;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (((IndiSendStart)Indicator).Input != null)
          cbParam.SelectedItem = ((IndiSendStart)Indicator).Input;
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (cbParam.SelectedItem != null) {
          ((IndiSendStart)Indicator).Input = (Input)cbParam.SelectedItem;
          Indicator.UpdateBindingValue();
        }
      }
      this.Close();
    }
  }
}
