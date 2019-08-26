using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Timers;
using Service;
using Modbus.Data;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace SparkOutput {
  class Program {

    private static int status = 0;
    public static int Status {
      get { return status; }
      set {
        status = value;
        new System.Threading.Thread(new System.Threading.ThreadStart(SendState)).Start();
      }
    }

    //1 - запущен, не работает
    //2 = запущен, работает 
    //3 = запущен, ошибка

    private static Service.IContract wcfMain;
    private static ObservableCollection<InputM> Inputs = new ObservableCollection<InputM>();
    private static ObservableCollection<SerialPort> SerialPorts = new ObservableCollection<SerialPort>();

    private static Timer ResetSerialPort = new Timer(5000);
    //private static Timer UpdateDataTimer = new Timer(1000);

    private static string EnvPath = "";

    static void Main(string[] args) {
      EnvPath = args[0];

      try {
        ServiceHost serviceHost = new ServiceHost(typeof(WCFService), new Uri("http://localhost:8000/sparkOutput"));
        serviceHost.AddServiceEndpoint(typeof(Service.IContractOut), new BasicHttpBinding(), "");
        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
        behavior.HttpGetEnabled = true;
        serviceHost.Description.Behaviors.Add(behavior);
        serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
        serviceHost.Open();
      } catch (Exception e) {
        Service.Log.LogShow(EnvPath, "Нет прав администратора", e.ToString(), "Ошибка", Service.MessageViewMode.Error);
        return;
      }

      ChannelFactory<Service.IContract> myChannelFactory = new ChannelFactory<Service.IContract>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkMain"));
      wcfMain = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfMain).OperationTimeout = new TimeSpan(0, 0, 10);

      Status = 1;
      ResetSerialPort.Elapsed += ResetSerialPort_Elapsed;
      //UpdateDataTimer.Elapsed += UpdateDataTimer_Elapsed;


      //new System.Threading.Thread(new System.Threading.ThreadStart(ReadOutput)).Start();

      while (true) {
        System.Threading.Thread.Sleep(250);
      }
    }

    /*private static void UpdateDataTimer_Elapsed(object sender, ElapsedEventArgs e) {
      foreach(InputM input in Inputs) {
        PairDataParam[] data = new PairDataParam[input.Input.ListDataParamsOut.Count];
          for(int i=0;i<input.Input.ListDataParamsOut.Count;i++) {
          sqlCmd.CommandText =
              "SELECT * FROM '" + input.Input.ListDataParamsOut[i].Title + "|" + input.Input.ListDataParamsOut[i].ParamUnitTitle + "' ORDER BY ID DESC LIMIT 1";
          SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
          DataTable dt = new DataTable();
          dt.Load(dataReader);
          data[i].ID = input.Input.ListDataParamsOut[i].ID;
          data[i].Value = Convert.ToSingle(dt.Rows[0][1]);
        }
        UpdateData(input.Input.Title, data);
      }
    }*/

    public static void SendState() {
      //Service.WorkFile.Do(EnvPath + "outputState.dat", Service.WorkFileMode.WriteNew, status.ToString());
      wcfMain.ChangeState("output", status);
    }

   /* private static void ReadOutput() {
      int countOutput = 0;
      while (true) {
        if (File.Exists(EnvPath + "toOutput" + countOutput + ".dat")) {
          countOutput++;
        } else {
          break;
        }
      }
      string[] data = new string[countOutput];
      for (int i = 0; i < countOutput; i++) {
        data[i] = Service.WorkFile.Do(EnvPath + "toOutput" + i.ToString() + ".dat", Service.WorkFileMode.ReadAllText).ToString();
        Service.WorkFile.DeleteFile(EnvPath + "toOutput" + i.ToString() + ".dat");
      }
      titleDB = Service.WorkFile.Do(EnvPath + "outputTitle.dat", Service.WorkFileMode.ReadAllText).ToString();
      Service.WorkFile.DeleteFile(EnvPath + "outputTitle.dat");
      dbConn = new SQLiteConnection("Data Source=" + EnvPath + titleDB + ".sqlite;Version=3;");
      dbConn.Open();
      sqlCmd.Connection = dbConn;
      UpdateOutputs(data);
    }*/

    public static void UpdateOutputs(string[] data) {
      StopPorts();
      Inputs = new ObservableCollection<InputM>();
      for (int i = 0; i < data.Length; i++) {
        JObject json = JObject.Parse(data[i]);
        if (json.Property("PortName") != null) {
          Service.InputSerial input = JsonConvert.DeserializeObject<Service.InputSerial>(data[i]);
          Inputs.Add(new InputM { Input = input });
        } else {
          Service.InputEthernet input = JsonConvert.DeserializeObject<Service.InputEthernet>(data[i]);
          Inputs.Add(new InputM { Input = input });
        }
      }
      StartPorts();
    }

    public static void StopPorts() {
      Status = 1;
      foreach (SerialPort ports in SerialPorts) {
        if (ports.IsOpen) {
          ports.Close();
          ports.Dispose();
        }
      }
      SerialPorts = new ObservableCollection<SerialPort>();
    }

    private static void StartPorts() {
      foreach (InputM inputM in Inputs) {
        if (!(inputM.Input).IsUsed)
          continue;
        if (inputM.Input.GetType() == typeof(Service.InputSerial)) {
          SerialPort port = new SerialPort() {
            PortName = ((Service.InputSerial)inputM.Input).PortName,
            BaudRate = ((Service.InputSerial)inputM.Input).BaudRate,
            Parity = ((Service.InputSerial)inputM.Input).Parity,
            StopBits = ((Service.InputSerial)inputM.Input).StopBits,
            Handshake = ((Service.InputSerial)inputM.Input).Handshake,
            ReadBufferSize = ((Service.InputSerial)inputM.Input).ReadBufferSize,
            ReadTimeout = ((Service.InputSerial)inputM.Input).ReadTimeout,
            DtrEnable = ((Service.InputSerial)inputM.Input).Dtr,
            RtsEnable = ((Service.InputSerial)inputM.Input).Rts
          };
          SerialPorts.Add(port);
          try {
            port.Open();
          }catch {
            new Service.MessageView("Не удается подключиться к " + port.PortName + " порту", "Ошибка", MessageViewMode.Error);
          }
          if (inputM.Input.InputType == Service.InputType.COMModbus) {
            Modbus.Device.ModbusSerialSlave slaveSerial = Modbus.Device.ModbusSerialSlave.CreateRtu((byte)inputM.Input.IDSlave, port);
            inputM.SlaveSerial = slaveSerial;
            inputM.SlaveSerial.DataStore = DataStoreFactory.CreateDefaultDataStore();
            System.Threading.Thread slaveThread = new System.Threading.Thread(new System.Threading.ThreadStart(inputM.SlaveSerial.Listen));
            slaveThread.Start();
          }
          ResetSerialPort.Start();
        } else {
          int port = 502;
          IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
          IPAddress[] addr = ipEntry.AddressList;
          TcpListener tcpListener = new TcpListener(addr[0], port);
          inputM.SlaveTCP = Modbus.Device.ModbusTcpSlave.CreateTcp((byte)inputM.Input.IDSlave, tcpListener);
          inputM.SlaveTCP.DataStore = DataStoreFactory.CreateDefaultDataStore();
          System.Threading.Thread slaveThread = new System.Threading.Thread(new System.Threading.ThreadStart(inputM.SlaveTCP.Listen));
          slaveThread.Start();
        }
      }
      Status = 2;
      //UpdateDataTimer.Start();
    }

    private static void ResetSerialPort_Elapsed(object sender, ElapsedEventArgs e) {
      for (int i = 0; i < SerialPorts.Count; i++) {
        if (SerialPorts[i].IsOpen == false)
          try {
            SerialPorts[i].Open();
          } catch { }
      }
    }

    private static InputM GetInputByTiile(string title) {
      foreach (InputM input in Inputs)
        if (input.Input.Title == title)
          return input;
      return null;
    }

    private static SerialPort GetSerialPortByPortName(string name) {
      foreach (SerialPort port in SerialPorts)
        if (port.PortName == name)
          return port;
      return null;
    }

    public static void UpdateData(string titleOutput, PairDataParam[] values) {
      InputM inputM = GetInputByTiile(titleOutput);
      if (inputM == null)
        return;
      if (inputM.Input.InputType == InputType.COMText) {
        SendDataSerial(GetSerialPortByPortName(((Service.InputSerial)inputM.Input).PortName), values, ((Service.InputSerial)inputM.Input).SymbolSplitter);
      }else {
        UpdateModbusDataStore(inputM, values);
      }

    }

    private static void SendDataSerial(SerialPort serial, PairDataParam[] values, string splitter) {
      if (serial == null)
        return;
      if (!serial.IsOpen)
        return;
      string data = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
      data += splitter;
      for(int i = 0; i < values.Length; i++) {
        data += values[i].Value.ToString();
        data += splitter;
      }
      serial.WriteLine(data);
    }

    private static void UpdateModbusDataStore(InputM inputM, PairDataParam[] values) {
      for (int i = 0; i < values.Length; i++) {
        ushort[] uintData = new ushort[2];
        float[] floatData = new float[] { values[i].Value };
        Buffer.BlockCopy(floatData, 0, uintData, 0, 4);
        if (inputM.Input.InputType == InputType.COMModbus) {
          inputM.SlaveSerial.DataStore.HoldingRegisters[i * 2 + 1] = uintData[0];
          inputM.SlaveSerial.DataStore.HoldingRegisters[i * 2 + 2] = uintData[1];
        } else {
          inputM.SlaveTCP.DataStore.HoldingRegisters[i * 2 +1] = uintData[0];
          inputM.SlaveTCP.DataStore.HoldingRegisters[i * 2 + 2] = uintData[1];
        }
      }
    }

  }
}
