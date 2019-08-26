using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace SparkControls {
  public partial class IndiGraphSet : Window, WindowSetting {
    public IndiGraphSet() {
      InitializeComponent();
      listParams.ItemsSource = SparkControlsService.ListInputParams;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if ((Indicator != null) && (Indicator.DataParams.Count > 0)) {
        if (Indicator.CountDot == 0)
          Indicator.CountDot = 100;
        tbHistory.Text = Indicator.CountDot.ToString();
        foreach (global::Service.DataParam dataParam in Indicator.DataParams) {
          foreach (global::Service.DataParam checkedParam in listParams.Items) {
            if (dataParam.ID == checkedParam.ID)
              listParams.SelectedItems.Add(checkedParam);
          }
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        Indicator.DataParams.Clear();
        int indexColor = 0;
        foreach (global::Service.DataParam dataParam in listParams.SelectedItems) {
          if (dataParam.ColorLine == Brushes.White)
            dataParam.ColorLine = SparkControls.Helper.ListColor[indexColor];
          indexColor++;
          Indicator.DataParams.Add(dataParam);
          
        }
        if (tbHistory.Text == "")
          tbHistory.Text = "0";
        Indicator.CountDot = Convert.ToInt32(tbHistory.Text);
      }
      this.Close();
    }

    private void tbHistory_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      Regex regex = new Regex("[^0-9]+");
      e.Handled = regex.IsMatch(e.Text);
    }

    private void tbHistory_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.Space)
        e.Handled = true;
    }
  }
}
