using System.Windows;

namespace SparkControls {
  public partial class IndiBooleanSet : Window, WindowSetting{

    public Indicator Indicator { get; set; }

    public IndiBooleanSet() {
      InitializeComponent();
      cbParam.ItemsSource = SparkControlsService.ListInputParams;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if ((Indicator != null) && (Indicator.DataParams.Count > 0)) {
        cbParam.SelectedItem = Indicator.DataParams[0];
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        if (cbParam.SelectedItem != null) {
          Indicator.DataParams.Clear();
          Indicator.DataParams.Add((global::Service.DataParam)cbParam.SelectedItem);
          Indicator.UpdateBindingValue();
        }
      }
      this.Close();
    }
  }
}
