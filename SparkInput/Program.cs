using System;
using System.Collections.ObjectModel;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using System.Data.SQLite;
using System.IO;
using System.Collections;
using System.Linq;
using System.Net.Sockets;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Modbus.Device;

namespace SparkInput {
  public class Program {

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
    private static ObservableCollection<PipeCounterHelper> PipeCounterHelpers = new ObservableCollection<PipeCounterHelper>();

    private static Timer TimerWorkModbus = null;
    private static int secondsTimerWorkModbus = 545;
    private static Timer ResetSerialPort = new Timer(5000);

    private static string dbFileName = "spark.sqlite";
    private static SQLiteConnection dbConn = new SQLiteConnection();
    //private static SQLiteCommand sqlCmd = new SQLiteCommand();

    private static string EnvPath = "";

    private static int NumWork { get; set; }

    static void Main(string[] args) {
      AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
        Environment.Exit(1); // завершаем процесс
      };
      EnvPath = args[0];
      

      new System.Threading.Thread(new System.Threading.ThreadStart(ReadInput)).Start();

      try {
        ServiceHost serviceHost = new ServiceHost(typeof(WCFService), new Uri("http://localhost:8000/sparkInput"));
        serviceHost.AddServiceEndpoint(typeof(Service.IContractIn), new BasicHttpBinding(), "");
        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
        behavior.HttpGetEnabled = true;
        serviceHost.Description.Behaviors.Add(behavior);
        serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
        serviceHost.Open();
      } catch {
        //Service.Log.LogShow(EnvPath, "Нет прав администратора", e.ToString(), "Ошибка", Service.MessageViewMode.Error);
      }

      ChannelFactory<Service.IContract> myChannelFactory = new ChannelFactory<Service.IContract>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/sparkMain"));
      wcfMain = myChannelFactory.CreateChannel();
      ((IClientChannel)wcfMain).OperationTimeout = new TimeSpan(0, 0, 10);

      ResetSerialPort.Elapsed += ResetSerialPort_Elapsed;
      Status = 1;

      while (true) {
        System.Threading.Thread.Sleep(250);
      }
    }

    public static void SendState() {
      wcfMain.ChangeState("input", status);
    }

    private static void ReadInput() {
      string[] dataPipeCounter = Service.WorkFile.Do(EnvPath + "pipeCounter.txt", Service.WorkFileMode.ReadAllText).ToString().Split('#');
      if (dataPipeCounter.Length == 4) {
        PipeCounterHelper.dempfer = Convert.ToInt32(dataPipeCounter[0]) / 100.0;
        PipeCounterHelper.lengthFrom = Convert.ToInt32(dataPipeCounter[1]);
        PipeCounterHelper.lengthTo = Convert.ToInt32(dataPipeCounter[2]);
        PipeCounterHelper.pauseTime = Convert.ToInt32(dataPipeCounter[3]);
      }
    }

    public static void UpdateInputs(string[] data, string titleDB) {
      StopPorts();
      dbFileName = EnvPath + titleDB + ".sqlite";
      if (!File.Exists(dbFileName))
        SQLiteConnection.CreateFile(dbFileName);
      try {
        dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
        dbConn.Open();
        //sqlCmd.Connection = dbConn;
        SQLiteCommand sqlCmd = new SQLiteCommand(dbConn);
        sqlCmd.CommandText = "select numwork from 'works' where id = (select count(*) from works);";
        NumWork = Convert.ToInt32(sqlCmd.ExecuteScalar());
      } catch (Exception e) {
        Service.Log.LogWrite(EnvPath, "Ошибка подключения к базе данных", e.ToString());
      }
      Inputs = new ObservableCollection<InputM>();
      for (int i = 0; i < data.Length; i++) {
        JObject json = JObject.Parse(data[i]);
        if (json.Property("PortName") != null) {
          Service.InputSerial input = JsonConvert.DeserializeObject<Service.InputSerial>(data[i]);
          for (int j = 0; j < input.ListDataParams.Count; j++) {
            if ((input.ListDataParams[j].ParamType.Title == "Длина трубы") && (j + 1 < input.ListDataParams.Count) && (input.ListDataParams[j + 1].ParamType.Title == "Кол-во труб")) {
              PipeCounterHelpers.Add(new PipeCounterHelper { IndexCounter = j + 1 });
              j++;
            }
          }
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
        if (!((Service.InputCommon)inputM.Input).IsUsed)
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
          if (((Service.InputCommon)inputM.Input).InputType == Service.InputType.COMText) {
            port.DataReceived += Port_DataReceived;
          } else {
            Modbus.Device.IModbusSerialMaster serialMaster = Modbus.Device.ModbusSerialMaster.CreateRtu(port);
            serialMaster.Transport.Retries = 3;
            port.ReadTimeout = 3000;
            port.WriteTimeout = 3000;
            serialMaster.Transport.WaitToRetryMilliseconds = 3000;
            inputM.MasterSerial = serialMaster;
          }
          try {
            port.Open();
          } catch {
            Status = 3;
            new Service.MessageView("Не удается подключиться к " + port.PortName + " порту", "Ошибка", Service.MessageViewMode.Error);
          }
          ResetSerialPort.Start();
        } else {
          TcpClient client = new TcpClient();
          client.BeginConnect(((Service.InputEthernet)inputM.Input).IPSlave, ((Service.InputEthernet)inputM.Input).Port, null, null);
          inputM.MasterTCP = Modbus.Device.ModbusIpMaster.CreateIp(client);
          inputM.MasterTCP.Transport.Retries = 1;
          inputM.MasterTCP.Transport.WaitToRetryMilliseconds = 900;
        }
      }
      if (TimerWorkModbus == null) {
        TimerWorkModbus = new Timer(1000);
        TimerWorkModbus.Enabled = true;
        TimerWorkModbus.Elapsed += TimerWorkModbus_Elapsed;
        //TimerWorkModbus.AutoReset = false;
      }
      Status = 2;
    }

    private static void ResetSerialPort_Elapsed(object sender, ElapsedEventArgs e) {
      for (int i = 0; i < SerialPorts.Count; i++) {
        if (SerialPorts[i].IsOpen == false)
          try {
            SerialPorts[i].Open();
          } catch { }
      }
    }

    private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
      SerialPort sp = (SerialPort)sender;
      string data = "";
      try {
        if (!sp.IsOpen)
          return;
        data = sp.ReadLine();
      } catch {
        if (!sp.IsOpen)
          return;
        data = sp.ReadExisting();
        data += "\n";
      }
      try {
        Service.InputSerial input = GetInputByCOMPort(sp.PortName);
        string[] splittedData = data.Split(new string[] { input.SymbolSplitter }, StringSplitOptions.None);
        if (splittedData.Length < 2)
          return;

        int startIndexData = 0;
        DateTime dateTime;
        if (DateTime.TryParse(splittedData[0], out dateTime))
          startIndexData = 1;

        int startIndexInput = 0;
        if (input.ListDataParams[0].ParamType.Title == "Время")
          startIndexInput = 1;

        for (; startIndexData < splittedData.Length; startIndexData++) {
          if (startIndexInput >= input.ListDataParams.Count)
            break;
          input.ListDataParams[startIndexInput].Value = Convert.ToSingle(splittedData[startIndexData].Replace('.', ','));
          startIndexInput++;
          if (startIndexInput >= input.ListDataParams.Count)
            break;
          if (input.ListDataParams[startIndexInput].ParamType.Title == "Кол-во труб") {
            startIndexInput++;
          }
        }
        CalcCountPipe(input);
        WriteToBD(input);
        WCFSendValues(input);
        Status = 2;
      } catch (Exception ex) {
        Service.Log.LogWrite(EnvPath, ex.Message, ex.ToString());
      }
    }

    private static Service.InputSerial GetInputByCOMPort(string portName) {
      foreach (InputM inputM in Inputs)
        if (inputM.Input.GetType() == typeof(Service.InputSerial))
          if (((Service.InputSerial)inputM.Input).PortName == portName)
            return (Service.InputSerial)inputM.Input;
      return null;
    }


    private static void TimerWorkModbus_Elapsed(object sender, ElapsedEventArgs e) {
      secondsTimerWorkModbus++;
      foreach (InputM inputM in Inputs) {
        try {
          if (((Service.InputCommon)inputM.Input).InputType != Service.InputType.COMText) {
            if (!((Service.InputCommon)inputM.Input).IsUsed)
              continue;
            for (int i = 0; i < ((Service.InputCommon)inputM.Input).ListDataParams.Count;) {
              if (((Service.InputCommon)inputM.Input).ListDataParams[i].ParamType.Title == "Уровень(A114)") {
                if (secondsTimerWorkModbus >= 600) {
                  secondsTimerWorkModbus = 0;
                  ((Service.InputCommon)inputM.Input).ListDataParams[i].Value = ReadAutonA114(
                      inputM.MasterSerial,
                      ((Service.InputCommon)inputM.Input).ListDataParams[i].Address,
                      (byte)((Service.InputCommon)inputM.Input).ListDataParams[i].SlaveID
                      );
                }
                i++;
                continue;
              }
              byte slaveId = Convert.ToByte(((Service.InputCommon)inputM.Input).ListDataParams[i].SlaveID);
              string[] addressSplit = (((Service.InputCommon)inputM.Input).ListDataParams[i].Address.ToString()).Replace('.', ',').Split(',');
              ushort startAddress = (ushort)(Convert.ToInt16(addressSplit[0]));
              ushort numRegisters = 0;
              int countRegister = 1;
              switch (((Service.InputCommon)inputM.Input).ListDataParams[i].Type) {
                case Service.ModbusType.BIT:
                case Service.ModbusType.WORD:
                  numRegisters = 1;
                  break;
                case Service.ModbusType.DWORD:
                case Service.ModbusType.FLOAT:
                  numRegisters = 2;
                  break;
              }

              #region определение кол-во читаемых регистров
              for (int j = i + 1; j < ((Service.InputCommon)inputM.Input).ListDataParams.Count && j > -1; j++) {
                if ((((Service.InputCommon)inputM.Input).ListDataParams[j].SlaveID == ((Service.InputCommon)inputM.Input).ListDataParams[i].SlaveID) &&
                  (((Service.InputCommon)inputM.Input).ListDataParams[j].Command == ((Service.InputCommon)inputM.Input).ListDataParams[i].Command) &&
                  (((Service.InputCommon)inputM.Input).ListDataParams[j].Type == ((Service.InputCommon)inputM.Input).ListDataParams[i].Type)) {
                  switch (((Service.InputCommon)inputM.Input).ListDataParams[i].Type) {
                    case Service.ModbusType.BIT:
                    case Service.ModbusType.WORD:
                      if (((Service.InputCommon)inputM.Input).ListDataParams[j].Address - ((Service.InputCommon)inputM.Input).ListDataParams[i].Address == 1)
                        countRegister++;
                      else
                        j = int.MaxValue;
                      break;
                    case Service.ModbusType.DWORD:
                    case Service.ModbusType.FLOAT:
                      if (((Service.InputCommon)inputM.Input).ListDataParams[j].Address - ((Service.InputCommon)inputM.Input).ListDataParams[i].Address == 2)
                        countRegister++;
                      else
                        j = int.MaxValue;
                      break;
                  }
                } else
                  break;
              }
              //i += countRegister - 1;
              numRegisters = (ushort)(numRegisters * countRegister);
              #endregion
              Status = 1;
              #region чтение данных
              ushort[] registers = null;
              bool[] coils = null;
              if (((Service.InputCommon)inputM.Input).ListDataParams[i].Command == Service.ModbusCommandInput.ReadCoil) {
                if (((Service.InputCommon)inputM.Input).InputType == Service.InputType.COMModbus) {
                  try {
                    coils = inputM.MasterSerial.ReadCoils(slaveId, startAddress, numRegisters);
                  } catch { }
                } else {
                  try {
                    coils = inputM.MasterTCP.ReadCoils(slaveId, startAddress, numRegisters);
                  } catch { }
                }
              }
              if (((Service.InputCommon)inputM.Input).ListDataParams[i].Command == Service.ModbusCommandInput.ReadHolding) {
                if (((Service.InputCommon)inputM.Input).InputType == Service.InputType.COMModbus) {
                  try {
                    registers = inputM.MasterSerial.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
                  } catch { }
                } else {
                  try {
                    registers = inputM.MasterTCP.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
                  } catch { }
                }
              }
              if ((registers == null) && (coils == null)) {
                Status = 3;
                //Service.Log.LogWrite("Registers == null", "Registers == null");
                continue;
              }
              #endregion
              Status = 2;
              #region преобразование прочитанных регистров
              switch (((Service.InputCommon)inputM.Input).ListDataParams[i].Type) {
                case Service.ModbusType.BIT:
                  if (registers != null) {
                    for (int r = 0; r < registers.Length; r++) {
                      BitArray b = new BitArray(new byte[] { BitConverter.GetBytes(registers[r])[0], BitConverter.GetBytes(registers[r])[1] });
                      int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
                      int numBit = 0;
                      try { numBit = Convert.ToInt16(addressSplit[1]); } catch { }
                      ((Service.InputCommon)inputM.Input).ListDataParams[i++].Value = bits[numBit];
                    }
                  } else {
                    for (int c = 0; c < coils.Length; c++) {
                      ((Service.InputCommon)inputM.Input).ListDataParams[i++].Value = Convert.ToSingle(coils[c]);
                    }
                  }
                  break;
                case Service.ModbusType.WORD:
                  for (int r = 0; r < registers.Length; r++) {
                    Int16 int16value = BitConverter.ToInt16(new byte[] {
                    BitConverter.GetBytes(registers[r])[0],
                    BitConverter.GetBytes(registers[r])[1] },
                      0);
                    ((Service.InputCommon)inputM.Input).ListDataParams[i++].Value = int16value;
                  }
                  break;
                case Service.ModbusType.DWORD:
                  for (int r = 0; r < registers.Length; r += 2) {
                    int intValue = BitConverter.ToInt32(new byte[] {
                    BitConverter.GetBytes(registers[r])[0],
                    BitConverter.GetBytes(registers[r])[1],
                    BitConverter.GetBytes(registers[r+1])[0],
                    BitConverter.GetBytes(registers[r+1])[1]},
                      0);
                    ((Service.InputCommon)inputM.Input).ListDataParams[i++].Value = intValue;
                  }
                  break;
                case Service.ModbusType.FLOAT:
                  for (int r = 0; r < registers.Length; r += 2) {
                    float floatData = BitConverter.ToSingle(new byte[] {
                    BitConverter.GetBytes(registers[r])[0],
                    BitConverter.GetBytes(registers[r])[1],
                    BitConverter.GetBytes(registers[r+1])[0],
                    BitConverter.GetBytes(registers[r+1])[1]},
                      0);
                    ((Service.InputCommon)inputM.Input).ListDataParams[i++].Value = floatData;
                  }
                  break;
              }
              #endregion
            }
            WriteToBD((Service.InputCommon)inputM.Input);
            WCFSendValues((Service.InputCommon)inputM.Input);
          }
        } catch (Exception ex) {
          Service.Log.LogWrite(EnvPath, ex.Message, ex.ToString());
        }
      }
    }

    private static void WriteToBD(Service.InputCommon input) {
      foreach (Service.DataParam param in input.ListDataParams) {
        try {
          SQLiteCommand sqlCmd = new SQLiteCommand(dbConn);
          sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS '" + param.Title + "|" + param.ParamUnit.Title + "' (id INTEGER PRIMARY KEY AUTOINCREMENT, value real, dt text, numwork int )";
          sqlCmd.ExecuteNonQuery();
          sqlCmd.CommandText = "INSERT INTO '" + param.Title + "|" + param.ParamUnit.Title + "' ('value', 'dt', 'numwork') values ('" +
                  param.Value + "' , '" +
                  DateTime.Now + "', '" +
                  NumWork + "')";
          sqlCmd.ExecuteNonQuery();
        } catch {

        }
      }
    }

    private static void WCFSendValues(Service.InputCommon input) {
      Service.PairDataParam[] pairs = new Service.PairDataParam[input.ListDataParams.Count];
      for (int i = 0; i < input.ListDataParams.Count; i++) {
        pairs[i].ID = input.ListDataParams[i].ID;
        pairs[i].Value = input.ListDataParams[i].Value;
      }
      wcfMain.SetValue(pairs);
    }

    public static void SendData(int idDataParam, float newValue) {
      foreach (InputM inputM in Inputs)
        foreach (Service.DataParam dataParam in ((Service.InputCommon)inputM.Input).ListDataParams) {
          if (dataParam.ID == idDataParam) {
            byte slaveId = Convert.ToByte(dataParam.SlaveID);
            ushort startAddress = (ushort)(Convert.ToInt16(dataParam.Address));

            if (dataParam.Type == Service.ModbusType.BIT) {
              if (dataParam.Command == Service.ModbusCommandInput.ReadCoil) {
                if (inputM.MasterTCP != null) {
                  inputM.MasterTCP.WriteSingleCoil(startAddress, Convert.ToBoolean(newValue));
                } else {
                  inputM.MasterSerial.WriteSingleCoil(slaveId, startAddress, Convert.ToBoolean(newValue));
                }
              } else {
                string[] addressSplit = dataParam.Address.ToString().Replace('.', ',').Split(',');
                startAddress = (ushort)(Convert.ToInt16(addressSplit[0]));
                int addCoil = (Convert.ToInt32(addressSplit[1]));
                ushort[] registers;
                if (inputM.MasterTCP != null) {
                  registers = inputM.MasterTCP.ReadHoldingRegisters(slaveId, startAddress, 1);
                } else {
                  registers = inputM.MasterSerial.ReadHoldingRegisters(slaveId, startAddress, 1);
                }
                BitArray b = new BitArray(new byte[] { BitConverter.GetBytes(registers[0])[0], BitConverter.GetBytes(registers[0])[1] });
                int[] bits = b.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
                bits[addCoil] = (bits[addCoil] == 0) ? 1 : 0;
                string stringTemp = ConvertBtoS(bits);
                short result = Convert.ToInt16(stringTemp, 2);
                if (inputM.MasterTCP != null) {
                  inputM.MasterTCP.WriteSingleRegister(startAddress, (ushort)result);
                } else {
                  inputM.MasterSerial.WriteSingleRegister(slaveId, startAddress, (ushort)result);
                }
              }
            } else {
              int countReg = 1;
              if (dataParam.Type != Service.ModbusType.WORD)
                countReg = 2;

              byte[] data;
              if (countReg == 1) {
                data = BitConverter.GetBytes(Convert.ToUInt16(newValue));
              } else {
                if (dataParam.Type == Service.ModbusType.DWORD) {
                  data = BitConverter.GetBytes(Convert.ToUInt32(newValue));
                } else {
                  data = BitConverter.GetBytes(newValue);
                }
              }
              ushort[] regs = new ushort[countReg];
              regs[0] = BitConverter.ToUInt16(data, 0);
              if (countReg == 2)
                regs[1] = BitConverter.ToUInt16(data, 2);
              if (inputM.MasterTCP != null) {
                inputM.MasterTCP.WriteMultipleRegisters(slaveId, startAddress, regs);
              } else {
                inputM.MasterSerial.WriteMultipleRegisters(slaveId, startAddress, regs);
              }
              return;
            }
          }
        }
    }

    public static void SendStart(string titleInput) {
      foreach(SerialPort port in SerialPorts) {
        if (GetInputByCOMPort(port.PortName).Title == titleInput) {
          port.Write("start\r");
          Service.InputCommon input = GetInputByCOMPort(port.PortName);
          foreach(Service.DataParam param in input.ListDataParams) {
            if (param.ParamType.Title == "Кол-во труб") {
              param.Value = 0;
              PipeCounterHelpers[0].Reset();
              

            }
          }
          WCFSendValues(input);
          WriteToBD(input);
        }
      }

    }

    public static void SendSoOn(string titleInput) {
      foreach (SerialPort port in SerialPorts) {
        if (GetInputByCOMPort(port.PortName).Title == titleInput) {
          port.Write("so1on 1\r");
          System.Threading.Thread.Sleep(1000);
          port.Write("so2on 1\r");
        }
      }
    }

    private static string ConvertBtoS(int[] input) {
      string result = "";
      for (int i = input.Length - 1; i > -1; i--)
        result += input[i].ToString();
      return result;
    }

    private static void CalcCountPipe(Service.InputSerial input) {
      foreach (PipeCounterHelper pipeCounterHelper in PipeCounterHelpers) {
        try {
          if (pipeCounterHelper.IsInitCounterPipe == false) {
            SQLiteCommand sqlCmd = new SQLiteCommand(dbConn);
            sqlCmd.CommandText = "select count(*) from sqlite_master where name='" +
              input.ListDataParams[pipeCounterHelper.IndexCounter].Title +
              "|" +
              input.ListDataParams[pipeCounterHelper.IndexCounter].ParamUnit.Title + "';";
            if (Convert.ToInt32(sqlCmd.ExecuteScalar()) == 0)
              return;
            sqlCmd.CommandText =
              "SELECT * FROM '" +
              input.ListDataParams[pipeCounterHelper.IndexCounter].Title +
              "|" +
              input.ListDataParams[pipeCounterHelper.IndexCounter].ParamUnit.Title +
              "' ORDER BY ID DESC LIMIT 1";
            SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dataReader);
            input.ListDataParams[pipeCounterHelper.IndexCounter].Value = Convert.ToSingle(dt.Rows[0][1]);
            pipeCounterHelper.CountPipe = input.ListDataParams[pipeCounterHelper.IndexCounter].Value;
            pipeCounterHelper.DateTimeCounterPipe = Convert.ToDateTime(dt.Rows[0][2]);
            double subTime = DateTime.Now.Subtract(pipeCounterHelper.DateTimeCounterPipe).TotalSeconds;
            if (subTime > 1800) {
              pipeCounterHelper.Reset();
              input.ListDataParams[pipeCounterHelper.IndexCounter].Value = 0;
            }
            pipeCounterHelper.IsInitCounterPipe = true;
            //pipeCounterHelper.CalcPipe(input.ListDataParams[pipeCounterHelper.IndexCounter - 1].Value);
          } else {
            pipeCounterHelper.CalcPipe(input.ListDataParams[pipeCounterHelper.IndexCounter - 1].Value);
            input.ListDataParams[pipeCounterHelper.IndexCounter].Value = pipeCounterHelper.CountPipe;
          }
        } catch (Exception ex) {
          Service.Log.LogWrite(EnvPath, ex.Message, ex.ToString());
        }
      }

      return;
    }


    #region Auton
    private static float ReadAutonA114(Modbus.Device.IModbusSerialMaster master, double serialDevice, byte IDAddress) {
      //return 10f;
      string deviceName = "A114*." +
        ((serialDevice < 1000) ?
        "0" + ((int)serialDevice).ToString() :
        ((int)serialDevice).ToString());
      float level = -1;
      for (int i = 0; i < 3; i++) {
        try {
          AutonDeviceDisconnect(master, IDAddress);
          System.Threading.Thread.Sleep(5000);
          AutonDeviceConnect(master, deviceName, IDAddress);
          Authenticate(master, IDAddress);
          CurrentStateCheck(master, IDAddress);
          SettingAuton(master, IDAddress);
          level = (float)SingleMeasure(master, IDAddress);
          AutonDeviceDisconnect(master, IDAddress);
          break;
        } catch {
          System.Threading.Thread.Sleep(5000);
        }
      }

      return level;
    }

    private static int AutonDeviceConnect(IModbusSerialMaster master, string deviceName, byte IDAddress) {
      ushort[] stateM = new ushort[1];
      stateM = master.ReadHoldingRegisters(IDAddress, (ushort)4113, 1);
      short state = BitConverter.ToInt16(new byte[] {
                    BitConverter.GetBytes(stateM[0])[0],
                    BitConverter.GetBytes(stateM[0])[1] },
                      0);
      if (state > 0) {
        return 1;
      }

      byte[] deviceNameBytes = Encoding.ASCII.GetBytes(deviceName);
      ushort[] deviceNameUShort = new ushort[8];
      int j = 0;
      for (int i = 0; i < deviceNameBytes.Length; i++) {
        deviceNameUShort[j++] = BitConverter.ToUInt16(new byte[] {
        deviceNameBytes[i],
        deviceNameBytes[++i] }, 0);
      }
      master.WriteMultipleRegisters(IDAddress, (ushort)4112, deviceNameUShort);
      System.Threading.Thread.Sleep(1000);
      for (int i = 0; i < 30; i++) {
        stateM = master.ReadHoldingRegisters(IDAddress, (ushort)4113, 1);
        state = BitConverter.ToInt16(new byte[] {
                    BitConverter.GetBytes(stateM[0])[0],
                    BitConverter.GetBytes(stateM[0])[1] }, 0);
        if (state == 0)
          break;
        System.Threading.Thread.Sleep(1000);
      }
      return 0;
    }

    private static int Authenticate(IModbusSerialMaster master, byte IDAddress) {
      string pass = "28866";
      byte[] passBytes = Encoding.ASCII.GetBytes(pass);
      ushort[] passUShort = new ushort[8];
      int j = 0;
      for (int i = 0; i < passBytes.Length; i++) {
        passUShort[j++] = BitConverter.ToUInt16(new byte[] {
        passBytes[i],
        (i + 1 < passBytes.Length) ? passBytes[++i] : (byte)0 }, 0);
      }
      master.WriteMultipleRegisters(IDAddress, (ushort)1, passUShort);
      return 0;
    }

    private static int CurrentStateCheck(IModbusSerialMaster master, byte IDAddress) {
      ushort[] result = new ushort[1];
      result = master.ReadHoldingRegisters(IDAddress, (ushort)32, 1);
      short stateConnect = BitConverter.ToInt16(new byte[] {
                    BitConverter.GetBytes(result[0])[0],
                    BitConverter.GetBytes(result[0])[1] },
                      0);
      if (stateConnect > 0) {
        master.WriteSingleRegister(IDAddress, (ushort)32, 0);
      }
      System.Threading.Thread.Sleep(250);
      return 0;
    }

    private static int SettingAuton(IModbusSerialMaster master, byte IDAddress) {
      int date = portGetCurrentDate();
      int j = 0;
      byte[] arr = BitConverter.GetBytes(date);
      ushort[] nameDevice = new ushort[2];
      for (int i = 0; i < arr.Length; i++) {
        nameDevice[j++] = BitConverter.ToUInt16(new byte[] {
        arr[i],
        arr[++i] }, 0);
      }
      master.WriteMultipleRegisters(IDAddress, (ushort)35, new ushort[] { nameDevice[0], nameDevice[1] });
      master.WriteMultipleRegisters(IDAddress, (ushort)64, new ushort[] { 0, 5, nameDevice[0], nameDevice[1] });
      master.WriteMultipleRegisters(IDAddress, (ushort)69, new ushort[] { 5, 1, 0, 0, 0, 52224, 52428, 52428, 52428, 52428, 52428, 52428, 52428, 2, 999, 20 });
      master.WriteMultipleRegisters(IDAddress, (ushort)67, new ushort[] { 32, 0 });
      return 0;
    }

    private static double SingleMeasure(IModbusSerialMaster master, byte IDAddress) {
      master.WriteMultipleRegisters(IDAddress, (ushort)32, new ushort[] { 100 });
      bool measuring = true;
      while (measuring) {
        ushort[] stateRead = new ushort[1];
        stateRead = master.ReadHoldingRegisters(IDAddress, (ushort)32, 1);
        short stateConnect = BitConverter.ToInt16(new byte[] {
                    BitConverter.GetBytes(stateRead[0])[0],
                    BitConverter.GetBytes(stateRead[0])[1] },
                        0);
        if (stateConnect <= 0)
          break;
        System.Threading.Thread.Sleep(1000);
      }
      ushort[] result = new ushort[3];
      result = master.ReadHoldingRegisters(IDAddress, (ushort)58, 6);
      float responseDuration = BitConverter.ToSingle(new byte[] {
                    BitConverter.GetBytes(result[2])[0],
                    BitConverter.GetBytes(result[2])[1],
                    BitConverter.GetBytes(result[2+1])[0],
                    BitConverter.GetBytes(result[2+1])[1]},
                     0);
      return responseDuration * 301.0 / 2.0;
    }

    private static int AutonDeviceDisconnect(IModbusSerialMaster master, byte IDAddress) {
      ushort[] nameDevice = { 52224, 52428, 52428, 52428, 52428, 52428, 52428, 52428 };
      master.WriteMultipleRegisters(IDAddress, (ushort)4112, nameDevice);
      return 0;
    }

    private static int portGetCurrentDate() {
      int[] DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
      DateTime sys_time;
      int DEFAULT_YEAR = 1980;
      int daysInYear = 0;
      int i;
      int leapDays;

      // Получаем текущую дату-время
      sys_time = DateTime.Now;

      // Вычисляем какой по счету день в году
      for (i = 1; i < sys_time.Month; i++)
        daysInYear += DaysInMonth[i - 1];

      // Добавляем число в месяце
      daysInYear += sys_time.Day;

      // Вычисляем кол-во високосных дней прошедших от DEFAULT_YEAR
      leapDays = (sys_time.Year - DEFAULT_YEAR) / 4 +
        ((sys_time.Year - DEFAULT_YEAR) % 4 == 0 && sys_time.Month <= 2 ? 0 : 1);

      // Вычисляем сколько времени прошло от DEFAULT_YEAR
      return (int)(((int)(sys_time.Year - DEFAULT_YEAR)) * 31536000L +
        (int)(leapDays) * 86400L +
        (int)(daysInYear - 1) * 86400L + (int)(sys_time.Hour) * 3600L +
        (int)(sys_time.Minute) * 60L + (int)(sys_time.Second));
    }
    #endregion
  }
}
