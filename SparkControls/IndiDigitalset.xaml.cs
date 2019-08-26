using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SparkControls {
  public partial class IndiDigitalSet : Window, WindowSetting {

    public Indicator Indicator { get; set; }
    
    public IndiDigitalSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputParams;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (Indicator.DataParams.Count > 0)
        cbParam.SelectedItem = Indicator.DataParams[0];
        tbCountDot.Text = Indicator.CountDot.ToString();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (tbCountDot.Text == "")
          tbCountDot.Text = "0";
        Indicator.CountDot = Convert.ToInt32(tbCountDot.Text);
        if (cbParam.SelectedItem != null) {
          Indicator.DataParams.Clear();
          Indicator.DataParams.Add((global::Service.DataParam)cbParam.SelectedItem);
          Indicator.UpdateBindingValue();
        }
      }
      this.Close();
    }

    private void tbCountDot_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      Regex regex = new Regex("[^0-9]+");
      e.Handled = regex.IsMatch(e.Text);
    }

    private void tbCountDot_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.Space)
        e.Handled = true;
    }
  }
}
