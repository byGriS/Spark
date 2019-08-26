using System;
using System.Windows;
using System.IO;

namespace Spark {
  public partial class UnitSetWindow : Window {

    Core.SettingUnits settingUnits = null;

    public UnitSetWindow() {
      InitializeComponent();
    }

    private Service.ParamType selectedParam = null;
    private Service.ParamType SelectedParam {
      get { return selectedParam; }
      set {
        selectedParam = value;
        bInsertUnit.Visibility = Visibility.Hidden;
        bRemoveUnit.Visibility = Visibility.Hidden;
        if (value == null) {
          bInsertParam.Visibility = Visibility.Hidden;
          bRemoveParam.Visibility = Visibility.Hidden;
          bAddUnit.Visibility = Visibility.Hidden;
          dgUnit.ItemsSource = null;
        } else {
          bInsertParam.Visibility = Visibility.Visible;
          bRemoveParam.Visibility = Visibility.Visible;
          bAddUnit.Visibility = Visibility.Visible;
          if (((settingUnits.ParamsTypes[0] == selectedParam) && (selectedParam.Title == "Длина трубы")) ||
             ((settingUnits.ParamsTypes[1] == selectedParam) && (selectedParam.Title == "Кол-во труб")) ||
             ((settingUnits.ParamsTypes[2] == selectedParam) && (selectedParam.Title == "Уровень(A114)"))) {
            bInsertParam.Visibility = Visibility.Hidden;
            bRemoveParam.Visibility = Visibility.Hidden;
            bAddUnit.Visibility = Visibility.Hidden;
            bInsertUnit.Visibility = Visibility.Hidden;
            bRemoveUnit.Visibility = Visibility.Hidden;
          }
          dgUnit.ItemsSource = SelectedParam.ListUnits;
        }
      }
    }

    private Service.ParamUnit selectedUnit = null;
    private Service.ParamUnit SelectedUnit {
      get { return selectedUnit;}
      set {
        selectedUnit = value;
        if (value == null) {
          bInsertUnit.Visibility = Visibility.Hidden;
          bRemoveUnit.Visibility = Visibility.Hidden;
        }else {
          bInsertUnit.Visibility = Visibility.Visible;
          bRemoveUnit.Visibility = Visibility.Visible;
          if (((settingUnits.ParamsTypes[0] == selectedParam) && (selectedParam.Title == "Длина трубы")) || 
            ((settingUnits.ParamsTypes[1] == selectedParam) && (selectedParam.Title == "Кол-во труб")) ||
             ((settingUnits.ParamsTypes[2] == selectedParam) && (selectedParam.Title == "Уровень(A114)"))) {
            bInsertUnit.Visibility = Visibility.Hidden;
            bRemoveUnit.Visibility = Visibility.Hidden;
          }
        }
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      settingUnits = ((SparkWindow)this.Owner).settingUnits.Clone();
      DataContext = settingUnits.ParamsTypes;
    }

    private void Export_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
      sfd.Filter = "Params file (*.dat)|*.dat";
      sfd.Title = "Сохранить";
      FileInfo savePath = null;
      if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        savePath = new FileInfo(sfd.FileName);
      }
      if (savePath == null)
        return;
      string result = Core.Serialize.Units(settingUnits);
      Service.WorkFile.Do(savePath.FullName, Service.WorkFileMode.WriteNew, result);
    }

    private void Import_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
      ofd.Filter = "Params file (*.dat)|*.dat";
      ofd.Title = "Открыть";
      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        string input = (string)Service.WorkFile.Do(ofd.FileName, Service.WorkFileMode.ReadAllText);
        settingUnits = Core.Deserialize.Units(input);
        SelectedUnit = null;
        SelectedParam = null;
        DataContext = settingUnits.ParamsTypes;
      }     
    }

    private void dgParam_CurrentCellChanged(object sender, EventArgs e) {
      if (dgParam.CurrentItem != null) {
        SelectedParam = (Service.ParamType)dgParam.CurrentItem;
      }
    }

    private void dgUnit_CurrentCellChanged(object sender, EventArgs e) {
      if (dgUnit.CurrentItem != null) {
        SelectedUnit = (Service.ParamUnit)dgUnit.CurrentItem;
      }
    }

    private void AddParam_Click(object sender, RoutedEventArgs e) {
      settingUnits.ParamsTypes.Add(new Service.ParamType() { });
    }

    private void InsertParam_Click(object sender, RoutedEventArgs e) {
      if (SelectedParam == null) return;
      int index = settingUnits.ParamsTypes.IndexOf(SelectedParam);
      settingUnits.ParamsTypes.Insert(index, new Service.ParamType());
    }

    private void RemoveParam_Click(object sender, RoutedEventArgs e) {
      if (SelectedParam == null) return;
      if ((settingUnits.ParamsTypes[0] == SelectedParam) && (SelectedParam.Title == "Длина трубы")) {
        new Service.MessageView("Нельзя удалить параметр \"Длина трубы\"", "Внимание", Service.MessageViewMode.Attention).ShowDialog();
        return;
      }
      if ((settingUnits.ParamsTypes[1] == SelectedParam) && (SelectedParam.Title == "Кол-во труб")) {
        new Service.MessageView("Нельзя удалить параметр \"Кол-во труб\"", "Внимание", Service.MessageViewMode.Attention).ShowDialog();
        return;
      }
      if ((settingUnits.ParamsTypes[2] == SelectedParam) && (SelectedParam.Title == "Уровень(A114)")) {
        new Service.MessageView("Нельзя удалить параметр \"Уровень(А114)\"", "Внимание", Service.MessageViewMode.Attention).ShowDialog();
        return;
      }
      settingUnits.ParamsTypes.Remove(SelectedParam);
      SelectedParam = null;
      SelectedUnit = null;
    }

    private void AddUnit_Click(object sender, RoutedEventArgs e) {
      selectedParam.ListUnits.Add(new Service.ParamUnit() {  Title = "" });
      dgUnit.Items.Refresh();
    }

    private void InsertUnit_Click(object sender, RoutedEventArgs e) {
      if (SelectedUnit == null)
        return;
      int index = SelectedParam.ListUnits.IndexOf(SelectedUnit);
      SelectedParam.ListUnits.Insert(index, new Service.ParamUnit());
      dgUnit.Items.Refresh();
    }

    private void RemoveUnit_Click(object sender, RoutedEventArgs e) {
      if (SelectedUnit == null)
        return;
      SelectedParam.ListUnits.Remove(SelectedUnit);
      SelectedUnit = null;
      dgUnit.Items.Refresh();
    }


    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      ((SparkWindow)this.Owner).settingUnits = settingUnits.Clone();
      string result = Core.Serialize.Units(settingUnits);
      Service.WorkFile.Do(Core.Work.EnvPath + "params.dat", Service.WorkFileMode.WriteNew, result);
      this.Close();
    }

    private void dgParam_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e) {
      if (((settingUnits.ParamsTypes[0] == selectedParam) && (selectedParam.Title == "Длина трубы")) ||
           ((settingUnits.ParamsTypes[1] == selectedParam) && (selectedParam.Title == "Кол-во труб")) ||
           ((settingUnits.ParamsTypes[2] == selectedParam) && (selectedParam.Title == "Уровень(A114)"))) {
        e.Cancel = true;
      }      
    }
  }
}
