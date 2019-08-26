using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Spark {
  public partial class ParamsTextInput : UserControl {

    public InputSettingWindow InputSettingWindow { get; set; }

    private ObservableCollection<Service.DataParam> listDataParams = null;
    public ObservableCollection<Service.DataParam> ListDataParams {
      get { return listDataParams; }
      set {
        listDataParams = value;
        foreach (Service.DataParam dp in listDataParams) {
          dp.ListParamsTypes = SparkWindow.settingUnits.ParamsTypes;
          dp.ListParamsUnits = dp.ParamType.ListUnits;
          dp.OnChangeTitle -= Dp_OnChangeTitle;
          dp.OnChangeTitle += Dp_OnChangeTitle;
        }
        DataContext = listDataParams;
        bInsertParam.Visibility = System.Windows.Visibility.Hidden;
        bRemoveParam.Visibility = System.Windows.Visibility.Hidden;
      }
    }

    private bool Dp_OnChangeTitle(Service.DataParam dataParam, string oldTitle, string newTitle) {
      bool error = false;
      if (SparkWindow == null)
        return false;
      foreach (Service.Input input in InputSettingWindow.inputs) {
        if (error == true)
          break;
        foreach (Service.DataParam param in ((Service.InputCommon)input).ListDataParams) {
          if (param.Title == newTitle) {
            error = true;
            break;
          }
        }
      }
      if (error) {
        new Service.MessageView("Имя параметра: " + newTitle + " уже занято!", "Ошибка", Service.MessageViewMode.Error).ShowDialog();
        dataParam.Title = oldTitle;
      }
      return !error;
    }

    public SparkWindow SparkWindow { get; set; }

    private Service.DataParam selectedParam = null;
    private Service.DataParam SelectedParam {
      get { return selectedParam; }
      set {
        selectedParam = value;
        if (value == null) {
          bInsertParam.Visibility = System.Windows.Visibility.Hidden;
          bRemoveParam.Visibility = System.Windows.Visibility.Hidden;
        } else {
          bInsertParam.Visibility = System.Windows.Visibility.Visible;
          bRemoveParam.Visibility = System.Windows.Visibility.Visible;
        }
      }
    }

    public ParamsTextInput() {
      InitializeComponent();
    }

    private void AddParam_Click(object sender, System.Windows.RoutedEventArgs e) {
      if ((SparkWindow.settingUnits.ParamsTypes.Count == 0) || (SparkWindow.settingUnits.ParamsTypes[0].ListUnits.Count == 0)) {
        Service.Log.LogShow(Core.Work.EnvPath, "Отсутствует список единиц измерений\nСоставьте список: Настройки-Ед.изм.", "Not paramsTypes and Units", "Ошибка", Service.MessageViewMode.Error);
      }
      AddParam(ListDataParams.Count);
    }

    private void InsertParam_Click(object sender, System.Windows.RoutedEventArgs e) {
      if (SelectedParam == null)
        return;
      int index = ListDataParams.IndexOf(SelectedParam);
      AddParam(index);
    }

    private void AddParam(int index) {
      ListDataParams.Insert(index, new Service.DataParam() {
        ID = new Random().Next(0, int.MaxValue),
        SlaveID = 1,
        ListParamsTypes = SparkWindow.settingUnits.ParamsTypes,
        ListParamsUnits = SparkWindow.settingUnits.ParamsTypes[0].ListUnits,
        Command = Service.ModbusCommandInput.ReadHolding,
        Type = Service.ModbusType.WORD,
        ParamType = SparkWindow.settingUnits.ParamsTypes[0],
        ParamUnit = SparkWindow.settingUnits.ParamsTypes[0].ListUnits[0]
      });
      ListDataParams[index].OnChangeTitle += Dp_OnChangeTitle;
    }

    private void RemoveParam_Click(object sender, System.Windows.RoutedEventArgs e) {
      if (SelectedParam == null)
        return;
      ListDataParams.Remove(SelectedParam);
      SelectedParam = null;
    }

    private void DataGrid_CurrentCellChanged(object sender, System.EventArgs e) {
      DataGrid dataGrid = (DataGrid)sender;
      if (dataGrid.CurrentItem != null) {
        SelectedParam = (Service.DataParam)((DataGrid)sender).CurrentItem;
      }
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      ComboBox cb = (ComboBox)sender;
      if (cb.SelectedItem != null) {
        Service.DataParam param = (Service.DataParam)cb.DataContext;
        param.ListParamsUnits = ((Service.ParamType)cb.SelectedItem).ListUnits;
        if ((param.ParamUnit == null) && (param.ListParamsUnits.Count > 0))
          param.ParamUnit = param.ListParamsUnits[0];
      }
    }
  }
}