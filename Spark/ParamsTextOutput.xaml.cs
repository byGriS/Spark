using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Spark {
  public partial class ParamsTextOutput : UserControl {

    public OutputSettingWindow OutputSettingWindow { get; set; }
    public SparkWindow SparkWindow { get; set; }

    public ParamsTextOutput() {
      InitializeComponent();
    }

    private ObservableCollection<Service.DataParamOutput> listDataParams = null;
    public ObservableCollection<Service.DataParamOutput> ListDataParams {
      get { return listDataParams; }
      set {
        listDataParams = value;
        DataContext = listDataParams;
        bInsertParam.Visibility = Visibility.Hidden;
        bRemoveParam.Visibility = Visibility.Hidden;
      }
    }

    private Service.DataParamOutput selectedParam = null;
    private Service.DataParamOutput SelectedParam {
      get { return selectedParam; }
      set {
        selectedParam = value;
        if (value == null) {
          bInsertParam.Visibility = Visibility.Hidden;
          bRemoveParam.Visibility = Visibility.Hidden;
        } else {
          bInsertParam.Visibility = Visibility.Visible;
          bRemoveParam.Visibility = Visibility.Visible;
        }
      }
    }

    private void AddParam_Click(object sender, RoutedEventArgs e) {
      Service.DataParam addedParam = new Service.DataParam();
      if (new SelectDataParamOutput(SparkWindow.setting.GetListInputParams(), addedParam).ShowDialog() == true) {
        Service.DataParamOutput addParam = new Service.DataParamOutput() {
          ID = addedParam.ID,
          Title = addedParam.Title,
          ParamUnitTitle = addedParam.ParamUnit.Title
        };
        AddParam(ListDataParams.Count, addParam);
      }
    }

    private void InsertParam_Click(object sender, RoutedEventArgs e) {
      if (SelectedParam == null)
        return;
      int index = ListDataParams.IndexOf(SelectedParam);
      Service.DataParam addedParam = new Service.DataParam();
      if (new SelectDataParamOutput(SparkWindow.setting.GetListInputParams(), addedParam).ShowDialog() == true) {
        Service.DataParamOutput addParam = new Service.DataParamOutput() {
          ID = addedParam.ID,
          Title = addedParam.Title,
          ParamUnitTitle = addedParam.ParamUnit.Title
        };
        AddParam(index, addParam);
      }
    }

    private void AddParam(int index, Service.DataParamOutput addedParam) {
      addedParam.Address = index * 2;
      for(int i = index; i < ListDataParams.Count; i++) {
        ListDataParams[i].Address += 2;
      }
      ListDataParams.Insert(index, addedParam);
    }

    private void RemoveParam_Click(object sender, RoutedEventArgs e) {
      if (SelectedParam == null)
        return;
      int index = ListDataParams.IndexOf(SelectedParam);
      ListDataParams.Remove(SelectedParam);
      for (int i = index; i < ListDataParams.Count; i++) {
        ListDataParams[i].Address -= 2;
      }
      SelectedParam = null;
    }

    private void ParamsSerialTextOut_CurrentCellChanged(object sender, System.EventArgs e) {
      DataGrid dataGrid = (DataGrid)sender;
      if (dataGrid.CurrentItem != null) {
        SelectedParam = (Service.DataParamOutput)((DataGrid)sender).CurrentItem;
      }
    }
  }
}
