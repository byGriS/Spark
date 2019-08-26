using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace Spark {
  public partial class PipeCounterCorrectWindow : Window, INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private SQLiteConnection dbConn = new SQLiteConnection();
    private SQLiteCommand sqlCmd = new SQLiteCommand();
    private string selectedParam = "";

    private double valueParam = 0;
    public double ValueParam {
      get { return valueParam; }
      set {
        valueParam = value;
        OnPropertyChanged("ValueParam");
      }
    }

    public PipeCounterCorrectWindow() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      DirectoryInfo folder = new DirectoryInfo(Core.Work.EnvPath);
      FileInfo[] files = folder.GetFiles("*.sqlite");
      string[] filesName = new string[files.Length];
      for (int i = 0; i < files.Length; i++)
        filesName[i] = Path.GetFileNameWithoutExtension(files[i].FullName);
      lbDataBases.ItemsSource = filesName;
      this.DataContext = this;
    }

    private void lbDataBases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbDataBases.SelectedItem != null)
        btnConnect.IsEnabled = true;
      else
        btnConnect.IsEnabled = false;
    }

    private void ConnectDB_Click(object sender, RoutedEventArgs e) {
      if (lbDataBases.SelectedItem == null) {
        new Service.MessageView("Выберите базу данных", "", Service.MessageViewMode.Message).Show();
        return;
      }
      string dbFileName = Core.Work.EnvPath + lbDataBases.SelectedItem.ToString() + ".sqlite";
      try {
        if (dbConn.State != ConnectionState.Closed)
          dbConn.Close();
        dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
        dbConn.Open();
        sqlCmd.Connection = dbConn;
      } catch (Exception ex) {
        Service.Log.LogWrite(Core.Work.EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
      try {
        sqlCmd.CommandText = "Select name From sqlite_master where type='table' order by name;";
        SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(dataReader);

        List<string> list = new List<string>();
        //ObservableCollection<CheckedParam> list = new ObservableCollection<CheckedParam>();
        foreach (DataRow row in dt.Rows) {
          if (row.ItemArray[0].ToString() == "sqlite_sequence")
            continue;
          list.Add(row.ItemArray[0].ToString());
        }
        lbParams.ItemsSource = list;
        btnRead.IsEnabled = true;
      } catch (Exception ex) {
        Service.Log.LogWrite(Core.Work.EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
      btnInt.IsEnabled = false;
      btnDec.IsEnabled = false;
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      Close();
    }    

    private void Read_Click(object sender, RoutedEventArgs e) {
      if (lbParams.SelectedItem == null)
        return;
      btnInt.IsEnabled = true;
      btnDec.IsEnabled = true;
      selectedParam = lbParams.SelectedItem.ToString();
      sqlCmd.CommandText = "update '" + selectedParam + "' set value = replace(value, ',','.') WHERE id=(select id from '" + selectedParam + "' ORDER BY ID DESC LIMIT 1)";
      sqlCmd.ExecuteNonQuery();

      sqlCmd.CommandText =
                      "SELECT * FROM '" + selectedParam + "' ORDER BY ID DESC LIMIT 1";
      SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
      DataTable dt = new DataTable();
      dt.Load(dataReader);
      if ((DateTime.Now - Convert.ToDateTime(dt.Rows[0][2])).TotalSeconds < 1800)
        ValueParam = Convert.ToSingle(dt.Rows[0][1]);
      else
        new Service.MessageView("Последнее значение было более 30 минут назад", "", Service.MessageViewMode.Message).ShowDialog();
    }

    private void Decrement_Click(object sender, RoutedEventArgs e) {
      ValueParam--;
      sqlCmd.CommandText = "update '" + selectedParam + "' set value = " + ValueParam.ToString() + " WHERE id=(select id from '" + selectedParam + "' ORDER BY ID DESC LIMIT 1)";
      sqlCmd.ExecuteNonQuery();
    }

    private void Increment_Click(object sender, RoutedEventArgs e) {
      ValueParam++;
      sqlCmd.CommandText = "update '" + selectedParam + "' set value = "+ ValueParam.ToString()+" WHERE id=(select id from '" + selectedParam + "' ORDER BY ID DESC LIMIT 1)";
      sqlCmd.ExecuteNonQuery();
    }
  }
}
