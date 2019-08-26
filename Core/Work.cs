using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using System.ServiceModel;
using System;
using System.Timers;
using System.Net;
using System.IO;
using Service;

namespace Core {
  public class Work {
    private Setting setting = null;
    public static Setting _setting = null;
    private bool restart = false;

    public void CloseApp() {
      CloseInput();
      CloseOutput();
    }

    private void SetSetting(Setting settingIn) {
      setting = settingIn;
      _setting = settingIn;
    }

    #region Start/Stop input
    private void StartInput() {
      CloseInput();
      if (!restart) {
        SendWork();
      }
      Process input = new Process();
      input.StartInfo.FileName = Core.Work.EnvPath + "SparkInput.exe";
      input.StartInfo.Arguments = "\"" + Core.Work.EnvPath;
      input.Start();
    }

    public void StopInput() {
      Work.OnChangedState("input", 0);
      CloseInput();
    }
    private void CloseInput() {
      string[] checkName = new string[] { "SparkInput", "SparkInput.exe" };
      for (int i = 0; i < checkName.Length; i++) {
        Process[] result = Process.GetProcessesByName(checkName[i]);
        if (result.Length > 0) {
          foreach (Process arg in result) {
            try {
              arg.Kill();
            } catch { }
          }
        }
      }
    }
    #endregion

    #region Start/Stop output
    private void StartOutput() {
      CloseOutput();
      Process output = new Process();
      output.StartInfo.FileName = Core.Work.EnvPath + "SparkOutput.exe";
      output.StartInfo.Arguments = Core.Work.EnvPath;
      output.Start();
    }
    public void StopOutput() {
      CloseOutput();
    }
    private void CloseOutput() {
      string[] checkName = new string[] { "SparkOutput", "SparkOutput.exe" };
      for (int i = 0; i < checkName.Length; i++) {
        Process[] result = Process.GetProcessesByName(checkName[i]);
        if (result.Length > 0) {
          foreach (Process arg in result)
            arg.Kill();
        }
      }
    }
    #endregion

    #region Inputs Methods

    public void UpdateInput(Setting setting, bool restart) {
      this.restart = restart;
      SetSetting(setting);
      CloseInput();
      StartInput();
      ChannelFactory<Service.IContractIn> myChannelFactory = new ChannelFactory<Service.IContractIn>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkInput"));
      Service.IContractIn wcfInput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfInput).OperationTimeout = new TimeSpan(0, 1, 10);

      string[] data = new string[setting.Inputs.Count];
      for (int i = 0; i < setting.Inputs.Count; i++) {
        data[i] = JsonConvert.SerializeObject(setting.Inputs[i]);
      }
      int countConnect = 0;
      while (true) {
        try {
          countConnect++;
          wcfInput.UpdateInputs(data, setting.Title);
          break;
        } catch (Exception ex) {
          System.Threading.Thread.Sleep(500);
          if (countConnect > 5) {
            Service.Log.LogShow(EnvPath, "Не удается подключиться к модулю входов", ex.ToString(), "Ошибка", Service.MessageViewMode.Error);
            break;
          }
        }
      }
    }

    public void SendDataToInput(int idDataParam, float newValue) {
      ChannelFactory<Service.IContractIn> myChannelFactory = new ChannelFactory<Service.IContractIn>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkInput"));
      Service.IContractIn wcfInput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfInput).OperationTimeout = new TimeSpan(0, 0, 3);
      try {
        wcfInput.SendData(idDataParam, newValue);
      } catch { }
    }

    public void SendStartToInput(string inputTitle) {
      ChannelFactory<Service.IContractIn> myChannelFactory = new ChannelFactory<Service.IContractIn>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkInput"));
      Service.IContractIn wcfInput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfInput).OperationTimeout = new TimeSpan(0, 0, 3);
      try {
        wcfInput.SendStart(inputTitle);
      } catch { }
    }

    public void SendSoOnToInput(string inputTitle) {
      ChannelFactory<Service.IContractIn> myChannelFactory = new ChannelFactory<Service.IContractIn>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkInput"));
      Service.IContractIn wcfInput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfInput).OperationTimeout = new TimeSpan(0, 0, 3);
      try {
        wcfInput.SendSoOn(inputTitle);
      } catch { }
    }

    #endregion

    #region Outputs Methods

    public void UpdateOutput(Setting setting) {
      SetSetting(setting);
      CloseOutput();
      StartOutput();
      ChannelFactory<Service.IContractOut> myChannelFactory = new ChannelFactory<Service.IContractOut>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkOutput"));
      Service.IContractOut wcfInput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfInput).OperationTimeout = new TimeSpan(0, 1, 10);

      string[] data = new string[setting.Outputs.Count];
      for (int i = 0; i < setting.Outputs.Count; i++) {
        data[i] = JsonConvert.SerializeObject(setting.Outputs[i]);
      }
      int countConnect = 0;
      while (true) {
        try {
          countConnect++;
          wcfInput.UpdateOutputs(data);
          break;
        } catch (Exception ex) {
          System.Threading.Thread.Sleep(500);
          if (countConnect > 5) {
            Service.Log.LogShow(EnvPath, "Не удается подключиться к модулю выходов", ex.ToString(), "Ошибка", Service.MessageViewMode.Error);
            break;
          }
        }
      }
    }

    private Timer timerUpdateData = new Timer(1000);

    public void StartUpdateOutput() {
      timerUpdateData.Elapsed -= TimerUpdateData_Elapsed;
      timerUpdateData.Elapsed += TimerUpdateData_Elapsed;
      timerUpdateData.Enabled = true;
      timerUpdateData.Start();
    }

    public void StopUpdateOutput() {
      timerUpdateData.Enabled = false;
      timerUpdateData.Stop();
    }

    private void TimerUpdateData_Elapsed(object sender, ElapsedEventArgs e) {
      ChannelFactory<Service.IContractOut> myChannelFactory = new ChannelFactory<Service.IContractOut>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkOutput"));
      Service.IContractOut wcfOutput = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfOutput).OperationTimeout = new TimeSpan(0, 0, 3);
      foreach (Service.InputCommon output in setting.Outputs) {
        Service.PairDataParam[] pairs = new Service.PairDataParam[output.ListDataParamsOut.Count];
        for (int i = 0; i < output.ListDataParamsOut.Count; i++) {
          pairs[i] = new Service.PairDataParam {
            ID = output.ListDataParamsOut[i].ID,
            Value = setting.GetParamByID(output.ListDataParamsOut[i].ID).Value
          };
        }
        try {
          wcfOutput.UpdateDate(output.Title, pairs);
        } catch { }
      }
    }
    #endregion

    #region static 

    public delegate void ChangedState(string source, int state);
    public static event ChangedState OnChangedState;
    public delegate void ChangedStateWeb(string title, int state);
    public static event ChangedStateWeb OnChangedStateWeb;
    public static ObservableCollection<Service.DataParam> dataParam = null;
    public delegate void UpdateInputData(string inputTitle);
    public static event UpdateInputData OnUpdateInputData;


    public static void UpdateValues(Service.PairDataParam[] values) {
      if (dataParam == null)
        return;
      try {
        foreach (Service.PairDataParam pair in values) {
          foreach (Service.InputCommon input in _setting.Inputs) {
            foreach (DataParam param in input.ListDataParams) {
              if (pair.ID == param.ID) {
                param.Value = pair.Value;
                OnUpdateInputData?.Invoke(input.Title);
                break;
              }
            }
          }
        }
      } catch { }
    }

    private static DateTime timelapse = DateTime.Now;

    public static void SendDataToWebServer(Service.PairDataParam[] values) {
      //if (((TimeSpan)(DateTime.Now - timelapse)).TotalSeconds < 3) return;
      //timelapse = DateTime.Now;
      if (_setting == null) return;
      InputCommon input = null;
      if (values.Length > 0) {
        foreach (InputCommon arg in _setting.Inputs) {
          foreach(DataParam param in arg.ListDataParams) {
            if (values[0].ID == param.ID) {
              input = arg;
              break;
            }
          }
        }
      }
      if (input == null) return;
      DateTime now = DateTime.Now;
      if ((now - input.LastSendWebServer).TotalSeconds < 5) return;

      input.LastSendWebServer = now;
      
      DataSpark ds = new DataSpark();
      if (_setting == null) return;
      ds.numWork = _setting.NumWork.ToString();
      foreach (Service.PairDataParam pair in values) {
        foreach (Service.DataParam param in dataParam) {
          if (pair.ID == param.ID) {
            ds.dataParam.Add(new ParamSpark {
              paramTitle = param.Title,
              paramValue = pair.Value,
              timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            break;
          }
        }
      }
      string data = JsonConvert.SerializeObject(ds, Formatting.Indented);

      System.Threading.Thread threadSendData = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendData));
      threadSendData.Start(data);
      
    }

    public static void SendWork() {
      if (_setting == null) return;
      if (!_setting.IsSendToServer) {
        OnChangedStateWeb?.Invoke("Сервер ожидает", 2);
        return;
      }

      //string url = "http://192.168.1.2/api/setwork.php";
      string url = "http://fh7929y8.bget.ru/spark/api/setwork.php";

      WorkSpark ws = new WorkSpark();
      if (_setting == null) return;
      ws.id = _setting.NumWork.ToString();
      ws.team = _setting.Title;
      ws.field = _setting.Field;
      ws.bush = _setting.Bush;
      ws.well = _setting.Well;
      ws.nktmm = _setting.NKTmm.ToString();
      ws.lengthplan = _setting.LengthPlan.ToString();
      ws.speedplan = _setting.SpeedPlan.ToString();
      ws.waterplan = _setting.WaterPlan.ToString();
      ws.startwork = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      ws.endwork = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      string data = JsonConvert.SerializeObject(ws, Formatting.Indented);

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
        OnChangedStateWeb?.Invoke("Сервер работает", 1);
      } catch (Exception e) {
        OnChangedStateWeb?.Invoke("Сервер не отвечает", 0);
      }
    }

    public static void SendData(object data) {
      if (!_setting.IsSendToServer) {
        OnChangedStateWeb?.Invoke("Сервер ожидает", 2);
        return;
      }
      //string url = "http://192.168.1.2/api/setvalue.php";
      string url = "http://fh7929y8.bget.ru/spark/api/setvalue.php";
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
          if (jsonData != "") {
            SendWork();
          }
        }
        response.Close();
        OnChangedStateWeb?.Invoke("Сервер работает", 1);
      } catch (Exception e) {
        if (e.Message == "Запрос был прерван: Запрос отменен." || e.Message == "Удаленный сервер возвратил ошибку: (500) Внутренняя ошибка сервера.")
          SendWork();
        else
          OnChangedStateWeb?.Invoke("Сервер не отвечает", 0);
      }
    }

    public static bool CheckWebServer() {
      //string url = "http://192.168.1.2/api/test.php";
      string url = "http://fh7929y8.bget.ru/spark/api/test.php";
      HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);  //make request         
      request.Timeout = 2800;
      request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
      try {
        WebResponse response = request.GetResponse();
        string jsonData = String.Empty;
        using (var reader = new StreamReader(response.GetResponseStream())) {
          jsonData = reader.ReadToEnd();
        }
        response.Close();
        return jsonData == "1";
      } catch (Exception e) {
        OnChangedStateWeb?.Invoke("Сервер не отвечает", 0);
        return false;
      }
    }

    public static void ChangeState(string source, int state) {
      OnChangedState?.Invoke(source, state);
    }

    public static string EnvPath = "";

    #endregion
  }
}