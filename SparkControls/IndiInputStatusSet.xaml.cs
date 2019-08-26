using Service;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SparkControls {
  public partial class IndiInputStatusSet : Window, WindowSetting {
    public IndiInputStatusSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputs;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (((IndiInputStatus)Indicator).Input != null) {
          cbParam.SelectedItem = ((IndiInputStatus)Indicator).Input;
          tbCountDot.Text = ((IndiInputStatus)Indicator).TimerInterval.ToString();
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (tbCountDot.Text == "" || Convert.ToInt32(tbCountDot.Text) < 1)
          tbCountDot.Text = "1";
        ((IndiInputStatus)Indicator).TimerInterval = Convert.ToInt32(tbCountDot.Text);
        if (cbParam.SelectedItem != null) {
          ((IndiInputStatus)Indicator).Input = (Input)cbParam.SelectedItem;
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
