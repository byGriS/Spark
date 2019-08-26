using System.Windows;

namespace SparkControls {
  public partial class IndiManyParamSet : Window, WindowSetting {
    public IndiManyParamSet() {
      InitializeComponent();
      listParams.ItemsSource = SparkControlsService.ListInputParams;
    }

    public Indicator Indicator { get; set; }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      if ((Indicator != null) && (Indicator.DataParams.Count> 0)){
        foreach(global::Service.DataParam dataParam in Indicator.DataParams) {
          foreach(global::Service.DataParam checkedParam in listParams.Items) {
            if (dataParam.ID == checkedParam.ID)
              listParams.SelectedItems.Add(checkedParam);
          }
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (Indicator != null) {
        Indicator.DataParams.Clear();
        foreach (global::Service.DataParam dataParam in listParams.SelectedItems) {
          Indicator.DataParams.Add(dataParam);
        }
      }
      this.Close();
    }
  }
}
