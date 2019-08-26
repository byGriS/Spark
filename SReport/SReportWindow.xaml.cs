using System.Windows;
using System.Data.SQLite;
using System.ComponentModel;
using System.IO;
using System;
using System.Data;
using System.Collections.Generic;

namespace SReport {
  public partial class SReportWindow : Window {

    private SQLiteConnection dbConn = new SQLiteConnection();
    private SQLiteCommand sqlCmd = new SQLiteCommand();
    private CheckedParam selectedLength = null;
    private CheckedParam selectedCount = null;

    private string EnvPath = "";

    public string Field { get; set; }
    public string Bush { get; set; }
    public string Well { get; set; }
    public string NKTmm { get; set; }

    public SReportWindow() {
      InitializeComponent();
      this.EnvPath = Environment.CurrentDirectory + "\\";
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      DirectoryInfo folder = new DirectoryInfo(EnvPath);
      FileInfo[] files = folder.GetFiles("*.sqlite");
      string[] filesName = new string[files.Length];
      for (int i = 0; i < files.Length; i++)
        filesName[i] = Path.GetFileNameWithoutExtension(files[i].FullName);
      lbDataBases.ItemsSource = filesName;
    }

    private void lbDataBases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbDataBases.SelectedItem != null)
        btnConnect.IsEnabled = true;
      else
        btnConnect.IsEnabled = false;
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      if (lbDataBases.SelectedItem == null) {
        new Service.MessageView("Выберите базу данных", "", Service.MessageViewMode.Message).Show();
        return;
      }
      wb.NavigateToString("<html></html>");
      string dbFileName = EnvPath + lbDataBases.SelectedItem.ToString() + ".sqlite";
      try {
        if (dbConn.State != ConnectionState.Closed)
          dbConn.Close();
        dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
        dbConn.Open();
        sqlCmd.Connection = dbConn;
      } catch (Exception ex) {
        Service.Log.LogWrite(EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
      try {
        sqlCmd.CommandText = "Select name From sqlite_master where type='table' order by name;";
        SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(dataReader);
        List<CheckedParam> listLength = new List<CheckedParam>();
        List<CheckedParam> listCount = new List<CheckedParam>();
        try {
          foreach (DataRow row in dt.Rows) {
            if (row.ItemArray[0].ToString() == "sqlite_sequence" || row.ItemArray[0].ToString() == "works" || row.ItemArray[0].ToString().Split('|')[1] == "шт" )
              continue;
            listLength.Add(new CheckedParam {
              Title = row.ItemArray[0].ToString().Split('|')[0],
              Unit = row.ItemArray[0].ToString().Split('|')[1],
              IsChecked = false
            });
          }

          foreach (DataRow row in dt.Rows) {
            if (row.ItemArray[0].ToString() == "sqlite_sequence" || row.ItemArray[0].ToString() == "works" || row.ItemArray[0].ToString().Split('|')[1] != "шт")
              continue;
            listCount.Add(new CheckedParam {
              Title = row.ItemArray[0].ToString().Split('|')[0],
              Unit = row.ItemArray[0].ToString().Split('|')[1],
              IsChecked = false
            });
          }
        } catch (Exception ex) {
          new Service.MessageView("Нарушена структура базы данных", "Ошибка", Service.MessageViewMode.Error).Show();
          return;
        }

        if (listLength.Count == 0) {
          new Service.MessageView("В базе данные нет параметра длины трубы", "Ошибка", Service.MessageViewMode.Error).Show();
          return;
        }
        if (listCount.Count == 0) {
          new Service.MessageView("В базе данные нет параметра кол-ва труб", "Ошибка", Service.MessageViewMode.Error).Show();
          return;
        }

        lbLength.ItemsSource = listLength;
        lbCount.ItemsSource = listCount;
        btnRead.IsEnabled = true;
        selectedLength = null;
        selectedCount = null;
        FillingParamsData();

        lbLength.SelectedIndex = 0;
        lbCount.SelectedIndex = 0;
      } catch (Exception ex) {
        Service.Log.LogWrite(EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
    }

    private void Read_Click(object sender, RoutedEventArgs e) {
      Field = "_______________";
      Bush = "__________";
      Well = "__________";
      NKTmm = "_____";
      System.Threading.Thread window = null;
      readingWindow = new Service.MessageView("Считывание данных", "", Service.MessageViewMode.Message, false);
      window = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWindowReading));
      window.Start();

      wb.NavigateToString("<html></html>");
      selectedCount.Points = new List<PointDate>();
      selectedLength.Points = new List<PointDate>();

      sqlCmd.CommandText = "update '" + selectedLength.Title + "|" + selectedLength.Unit + "' set value = replace(value, ',','.')";
      sqlCmd.ExecuteNonQuery();
      sqlCmd.CommandText = "select * from '" + selectedLength.Title + "|" + selectedLength.Unit + "';";
      SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
      DataTable dt = new DataTable();
      dt.Load(dataReader);
      dataReader.Close();
      //bool initTitles = true;
      foreach (DataRow dr in dt.Rows) {
        selectedLength.Points.Add(new PointDate { X = Convert.ToDateTime(dr[2]), Y = Convert.ToDouble(dr[1]), id = Convert.ToInt32(dr[0]) });
        /*if (initTitles) {
          try {
            Field = dr[3].ToString() == "" ? "_______________" : dr[3].ToString();
            Bush = dr[4].ToString() == "" ? "________" : dr[4].ToString();
            Well = dr[5].ToString() == "" ? "________" : dr[5].ToString();
            NKTmm = dr[6].ToString() == "" ? "________" : dr[6].ToString();
          } catch {
            try {
              sqlCmd.CommandText = "select * from 'works' where id = " + dr[3].ToString();
              SQLiteDataReader dataReaderWorks = sqlCmd.ExecuteReader();
              DataTable dtWorks = new DataTable();
              dtWorks.Load(dataReaderWorks);
              dataReaderWorks.Close();
              Field = dtWorks.Rows[0][1].ToString() == "" ? "_______________" : dtWorks.Rows[0][1].ToString();
              Bush = dtWorks.Rows[0][2].ToString() == "" ? "________" : dtWorks.Rows[0][2].ToString();
              Well = dtWorks.Rows[0][3].ToString() == "" ? "________" : dtWorks.Rows[0][3].ToString();
              NKTmm = dtWorks.Rows[0][4].ToString() == "" ? "________" : dtWorks.Rows[0][4].ToString();
            } catch { }
          }
          initTitles = false;
        }*/
      }

      sqlCmd.CommandText = "update '" + selectedCount.Title + "|" + selectedCount.Unit + "' set value = replace(value, ',','.')";
      sqlCmd.ExecuteNonQuery();
      sqlCmd.CommandText = "select * from '" + selectedCount.Title + "|" + selectedCount.Unit + "';";
      dataReader = sqlCmd.ExecuteReader();
      dt = new DataTable();
      dt.Load(dataReader);
      foreach (DataRow dr in dt.Rows) {
        selectedCount.Points.Add(new PointDate { X = Convert.ToDateTime(dr[2]), Y = Convert.ToDouble(dr[1]), id = Convert.ToInt32(dr[0]) });
      }

      FillingParamsData();
      btnExport.IsEnabled = false;

      System.Threading.Thread.Sleep(100);
      readingWindow.Invoke(new Action(() => readingWindow.Close()));
    }

    private Service.MessageView readingWindow;
    private void ShowWindowReading() {
      try {
        readingWindow.ShowDialog();
      } catch { }
    }

    private void FillingParamsData() {
      if (selectedLength == null) {
        lStart.Content = "";
        lFinish.Content = "";
        btnDrawGraph.IsEnabled = false;
        btnExport.IsEnabled = false;
        dtpStart.IsEnabled = false;
        dtpFinish.IsEnabled = false;
        dtpStart.Value = null;
        dtpFinish.Value = null;
      } else {
        btnDrawGraph.IsEnabled = true;
        dtpStart.IsEnabled = true;
        dtpFinish.IsEnabled = true;
        DateTime start = DateTime.MaxValue;
        DateTime finish = DateTime.MinValue;
        if (start > selectedLength.Points[0].X)
          start = selectedLength.Points[0].X;
        if (finish < selectedLength.Points[selectedLength.Points.Count - 1].X)
          finish = selectedLength.Points[selectedLength.Points.Count - 1].X;
        lStart.Content = start.ToString("dd.MM.yyyy HH:mm:ss");
        lFinish.Content = finish.ToString("dd.MM.yyyy HH:mm:ss");
        dtpStart.Value = start;
        dtpFinish.Value = finish;
      }
    }

    private void lbLength_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbLength.SelectedItem != null)
        selectedLength = (CheckedParam)lbLength.SelectedItem;
    }

    private void lbCount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbCount.SelectedItem != null)
        selectedCount = (CheckedParam)lbCount.SelectedItem;
    }

    string result = "<html></html>";

    private void btnPreview_Click(object sender, RoutedEventArgs e) {




      List<double> resultValue = new List<double>();
      bool start = true;
      int curCount = 0;
      double curLength = 0;
      DateTime date = DateTime.Now;
      for (int i = 0; i < selectedCount.Points.Count; i++) {
        if (selectedCount.Points[i].X < dtpStart.Value.Value) {
          continue;
        }else {
          if (start) {
            curCount = (int)selectedCount.Points[i].Y;
            curLength = selectedLength.Points[i].Y;
            start = false;
            date = selectedCount.Points[i].X;

            sqlCmd.CommandText = "select * from '" + selectedLength.Title + "|" + selectedLength.Unit + "' where id = "+selectedCount.Points[i].id +";";
            SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dataReader);
            dataReader.Close();
            foreach (DataRow dr in dt.Rows) {
              try {
                Field = dr[3].ToString() == "" ? "_______________" : dr[3].ToString();
                Bush = dr[4].ToString() == "" ? "________" : dr[4].ToString();
                Well = dr[5].ToString() == "" ? "________" : dr[5].ToString();
                NKTmm = dr[6].ToString() == "" ? "________" : dr[6].ToString();
              } catch {
                try {
                  sqlCmd.CommandText = "select * from 'works' where id = " + dr[3].ToString();
                  SQLiteDataReader dataReaderWorks = sqlCmd.ExecuteReader();
                  DataTable dtWorks = new DataTable();
                  dtWorks.Load(dataReaderWorks);
                  dataReaderWorks.Close();
                  Field = dtWorks.Rows[0][1].ToString() == "" ? "_______________" : dtWorks.Rows[0][1].ToString();
                  Bush = dtWorks.Rows[0][2].ToString() == "" ? "________" : dtWorks.Rows[0][2].ToString();
                  Well = dtWorks.Rows[0][3].ToString() == "" ? "________" : dtWorks.Rows[0][3].ToString();
                  NKTmm = dtWorks.Rows[0][4].ToString() == "" ? "________" : dtWorks.Rows[0][4].ToString();
                } catch { }
              }
            }

          }
        }
        if (selectedCount.Points[i].X > dtpFinish.Value.Value)
          break;
        if (Math.Abs(selectedLength.Points[i].Y) < 5) {
          curCount = 0;
          curLength = 0;
        }
        if (Math.Abs((int)selectedCount.Points[i].Y) > Math.Abs(curCount)) {
          double calcLength = 0;
          calcLength = selectedLength.Points[i].Y - curLength;
          if (Math.Abs(calcLength) > 5) {
            resultValue.Add(Math.Abs(calcLength));
          }
          curCount = (int)selectedCount.Points[i].Y;
          curLength = selectedLength.Points[i].Y;
        }
      }

      int countColumns = 19;



      result = @"
<!DOCTYPE html>
<html lang='ru'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <meta http-equiv='X-UA-Compatible' content='ie=edge'>
  <title>Document</title>
  <style>
    table{
      width: 100%;
      padding: 0 25px;
      border-collapse: collapse;
      font-size:  0.8em;
    }
    .header{
      font-weight: bold;
      text-align: center;
    }
    h4, h3, h2{
      text-align: center;
      margin: 5px;
      padding: 0;
    }
    td, th{
      border: 1px solid black;
      text-align: center;
      width: 4.76%;
    }
    .br{
      height: 10px;
    }
  </style>
</head>
<body>
  <div class='header'>м. " + Field + " к. " + Bush + " скв. " + Well + "<br/>"+date.ToString("dd.MM.yyyy")+ @"</div>
  <h3>Мера НКТ "+ NKTmm+" мм</h3>";

      int indexColumn = 1;
      double totalLength = 0;
      int index = 0;
      for (int table = 0; table < 2; table++) {
        double[] columnLength = new double[countColumns];
        result += @"<table cellspacing='0'>";
        result += @"
<tr>
  <td></td>";
        for (int column = 0; column < countColumns; column++) {
          result += @"<th>" + indexColumn++ * 10 + @"</th>";
          columnLength[column] = 0;
        }
        result += @"
</tr>";
        for(int line = 0; line< 10; line++) {
          result += @"<tr>";
          result += @"<td>" + (line+1) + @"</td>";
          for (int column = 0; column < countColumns; column++) {
            string value = (index + column * 10 < resultValue.Count ? resultValue[index + column * 10].ToString("0.00") : " ");
            result += @"<td> " + value + @"</td>";
            columnLength[column] += (index + column * 10 < resultValue.Count ? resultValue[index + column * 10] : 0);
          }
          result += @"</tr>";
          index++;
        }

        result += @"
<tr>
  <td></td>";
        for (int column = 0; column < countColumns; column++) {
          result += @"<th>" + columnLength[column].ToString("0.00") + @"</th>";
        }
        result += @"
</tr>";
        result += @"
<tr>
  <td></td>";
        for (int column = 0; column < countColumns; column++) {
          totalLength += columnLength[column];
          result += @"<th>" + totalLength.ToString("0.00") + @"</th>";
          columnLength[column] = 0;
        }
        result += @"
</tr>
";
        index += countColumns * 9;
        result += @"<div><p></p></div></table>";
      }
      result += @"
  <div class='br'></div><div class='br'></div>
  <hr size = '0px' color='black' width='50%'>
  <div class='br'></div>
  <hr size = '0px' color='black' width='50%'>
  <div class='br'></div>
  <hr size = '0px' color='black' width='50%'>
  <div class='br'></div>
  <hr size = '0px' color='black' width='50%'>
  <div class='br'></div><div class='br'></div>
  <hr size = '0px' color='black' width='50%'>
  <div class='br'></div>
  <hr size = '0px' color='black' width='50%'>";
      result += @"
</body>
</html>
";
      result = result.Replace("\r\n", "");
      wb.NavigateToString(result);
      btnExport.IsEnabled = true;
    }

    private void btnExport_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
      sfd.Filter = "Report file (*.html)|*.html";
      sfd.Title = "Сохранить";
      FileInfo savePath = null;
      if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        savePath = new FileInfo(sfd.FileName);
      }
      if (savePath == null) return;
      Service.WorkFile.Do(savePath.FullName, Service.WorkFileMode.WriteNew, result);
    }
  }

  public class CheckedParam : INotifyPropertyChanged {

    private string title = "";
    public string Title {
      get { return title; }
      set {
        title = value;
        OnPropertyChanged("Title");
      }
    }

    private string unit = "";
    public string Unit {
      get { return unit; }
      set {
        unit = value;
        OnPropertyChanged("Unit");
      }
    }

    private bool isChecked = false;
    public bool IsChecked {
      get { return isChecked; }
      set {
        isChecked = value;
        OnPropertyChanged("IsChecked");
      }
    }

    public List<PointDate> Points = new List<PointDate>();

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }

  public struct PointDate {
    public int id;
    public DateTime X;
    public double Y;
  }
}
