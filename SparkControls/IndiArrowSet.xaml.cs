using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SparkControls {
  public partial class IndiArrowSet : Window, WindowSetting {
    public IndiArrowSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputParams;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (Indicator.DataParams.Count > 0)
          cbParam.SelectedItem = Indicator.DataParams[0];
        tbCountDot.Text = Indicator.CountDot.ToString();
        tbMin.Text = ((IndiArrow)Indicator).MinValue.ToString();
        tbMax.Text = ((IndiArrow)Indicator).MaxValue.ToString();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (tbCountDot.Text == "")
          tbCountDot.Text = "0";
        Indicator.CountDot = Convert.ToInt32(tbCountDot.Text);
        if (tbMin.Text == "")
          tbMin.Text = "0";
        ((IndiArrow)Indicator).MinValue = Convert.ToDouble(tbMin.Text);
        if (tbMax.Text == "")
          tbMax.Text = "0";
        ((IndiArrow)Indicator).MaxValue = Convert.ToDouble(tbMax.Text);
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
