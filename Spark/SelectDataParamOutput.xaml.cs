using System.Collections.ObjectModel;
using System.Windows;

namespace Spark {
  public partial class SelectDataParamOutput : Window {

    private Service.DataParam addedParam;

    public SelectDataParamOutput(ObservableCollection<Service.DataParam> dataParams, Service.DataParam addedParam) {
      InitializeComponent();
      DataContext = dataParams;
      this.addedParam = addedParam;
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      this.DialogResult = true;
      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.DialogResult = false;
      this.Close();
    }

    private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbParams.SelectedItem == null) {
        btnOk.IsEnabled = false;
        addedParam = null;
        return;
      } else {
        btnOk.IsEnabled = true;
        addedParam.ID = ((Service.DataParam)lbParams.SelectedItem).ID;
        addedParam.Title = ((Service.DataParam)lbParams.SelectedItem).Title;
        addedParam.ParamUnit =  new Service.ParamUnit { Title = ((Service.DataParam)lbParams.SelectedItem).ParamUnit.Title };
      }
    }
  }
}
