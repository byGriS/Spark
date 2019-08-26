using Core;
using Newtonsoft.Json;
using SparkControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Windows;

namespace Spark {
  public partial class ArchiveExportToServerWindow : Window {

    private WorkSpark selectedWork = null;
    private ObservableCollection<CheckedParam> selectedParam = new ObservableCollection<CheckedParam>();

    public ArchiveExportToServerWindow(string db, SQLiteCommand sqlCmd,ObservableCollection<WorkSpark> works, ObservableCollection<CheckedParam> selectedParam) {
      InitializeComponent();
      this.selectedParam = selectedParam;
      foreach (WorkSpark ws in works) {
        try {
          sqlCmd.CommandText = "select * from 'works' where numwork = '" + ws.id + "'";
          SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
          DataTable dt = new DataTable();
          dt.Load(dataReader);
          ws.numwork = ws.id;
          ws.team = db;
          ws.field = dt.Rows[0][1].ToString();
          ws.bush = dt.Rows[0][2].ToString();
          ws.well = dt.Rows[0][3].ToString();
          ws.nktmm = dt.Rows[0][4].ToString();
          ws.lengthplan = dt.Rows[0][5].ToString();
          ws.speedplan = dt.Rows[0][6].ToString();
          ws.waterplan = dt.Rows[0][7].ToString();
          ws.endwork = ws.startwork;
        } catch {
          sqlCmd.CommandText = "select * from 'works' where id = '" + ws.id + "'";
          SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
          DataTable dt = new DataTable();
          dt.Load(dataReader);
          ws.team = db;
          ws.field = dt.Rows[0][1].ToString();
          ws.bush = dt.Rows[0][2].ToString();
          ws.well = dt.Rows[0][3].ToString();
          ws.nktmm = dt.Rows[0][4].ToString();
          ws.lengthplan = dt.Rows[0][5].ToString();
          ws.speedplan = dt.Rows[0][6].ToString();
          ws.waterplan = dt.Rows[0][7].ToString();
          ws.endwork = ws.startwork;
          ws.numwork = (Math.Abs(unchecked((int)(DateTime.Parse(ws.startwork).Ticks)))).ToString();
        }
      }
      if (works.Count == 0) {
        new Service.MessageView("Нет данных для выгрузки", "Внимание", Service.MessageViewMode.Attention).ShowDialog();
        this.IsEnabled = false;
      }
      DataContext = works;
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      if (lbWorks.SelectedItem == null) {
        return;
      }
      selectedWork = (WorkSpark)lbWorks.SelectedItem;

      string work = JsonConvert.SerializeObject(selectedWork, Formatting.Indented);
      SendWork(work);
      
      DataSparkArchive ds = new DataSparkArchive();
      ds.numWork = selectedWork.id;
      int amountPackage = 0;
      int[] indexs = new int[selectedParam.Count];
      DateTime start = Convert.ToDateTime(selectedWork.startwork);
      bool loop = true;
      CounterBarWindow cbw = new CounterBarWindow();
      cbw.Value = "0";
      cbw.Show();

      for (int i = 0; i < selectedParam.Count; i++) {
        while (true) {
          if (selectedParam[i].Points[indexs[i]].X >= start)
            break;
          indexs[i]++;
        }
      }

      while (loop) {
        if (start > DateTime.Now) {
          break;
        }
        List<ParamSpark> dataList = new List<ParamSpark>();
        for (int i = 0; i < selectedParam.Count; i++) {
          if (indexs[i] >= selectedParam[i].Points.Count) {
            loop = false;
            break;
          }
          while (true) {
            if (indexs[i] >= selectedParam[i].Points.Count) {
              break;
            }
            if (selectedParam[i].Points[indexs[i]].X < start) {
              indexs[i]++;
            } else {
              break;
            }
          }
          if (indexs[i] >= selectedParam[i].Points.Count) {
            continue;
          }
          if (selectedParam[i].Points[indexs[i]].NumWork.ToString() != ds.numWork) {
            loop = false;
            break;
          }
          if (Math.Abs((selectedParam[i].Points[indexs[i]].X - start).TotalSeconds) < 5) {
            dataList.Add(new ParamSpark {
              paramTitle = selectedParam[i].Title,
              paramValue = Convert.ToSingle(selectedParam[i].Points[indexs[i]].Y),
              timestamp = start.ToString("yyyy-MM-dd HH:mm:ss")
            });
          }
        }
        
        if (!loop)
          break;
        if (dataList.Count != 0) {
          ds.dataParam.Add(dataList);
          amountPackage++;
        }
        start = start.AddSeconds(5);
        if (amountPackage > 29) {
          ds.numWork = selectedWork.numwork;
          string data2 = JsonConvert.SerializeObject(ds, Formatting.Indented);
          ds.numWork = selectedWork.id;
          SendExportData(data2);
          amountPackage = 0;
          ds.dataParam.Clear();
          cbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => cbw.Value = (Convert.ToInt32(cbw.Value) + 30).ToString()));
        }
      }
      cbw.Close();
      ds.numWork = selectedWork.numwork;
      string data = JsonConvert.SerializeObject(ds, Formatting.Indented);
      SendExportData(data);
      ds.dataParam.Clear();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void SendExportData(string data) {
      //string url = "http://192.168.1.2/api/setarchive.php";
      string url = "http://fh7929y8.bget.ru/spark/api/setarchive.php";
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);  //make request         
      request.ContentType = "application/json";
      request.Method = "POST";
      request.Timeout = 1000;
      request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
      try {
        using (StreamWriter writer = new StreamWriter(request.GetRequestStream())) {
          writer.Write(data.ToString());
        }
        WebResponse response = request.GetResponse();
        string jsonData = String.Empty;
        using (var reader = new StreamReader(response.GetResponseStream())) {
          jsonData = reader.ReadToEnd();
        }
        response.Close();
        //OnChangedStateWeb?.Invoke("Сервер работает", 1);
      } catch (Exception e) {
        //OnChangedStateWeb?.Invoke("Сервер не отвечает", 0);
      }
    }

    private bool SendWork(string data) {
      //string url = "http://192.168.1.2/api/setworkarchive.php";
      string url = "http://fh7929y8.bget.ru/spark/api/setworkarchive.php";
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);  //make request         
      request.ContentType = "application/json";
      request.Method = "POST";
      request.Timeout = 2800;
      request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
      try {
        using (StreamWriter writer = new StreamWriter(request.GetRequestStream())) {
          writer.Write(data.ToString());
        }
        WebResponse response = request.GetResponse();
        string jsonData = String.Empty;
        using (var reader = new StreamReader(response.GetResponseStream())) {
          jsonData = reader.ReadToEnd();
        }
        response.Close();
        return true;
      } catch (Exception e) {
        return false;
      }
    }
  }
}
