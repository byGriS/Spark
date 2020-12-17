using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Media;

namespace Core {
  public class Deserialize {
    public static Setting Setting(string data, SettingUnits settingUnits) {
      Setting setting = new Setting();
      string[] lines = data.Split('\n');
      int indexLine = 0;
      int version = lines[indexLine] != "" ? Convert.ToInt32(lines[indexLine]) : 0;
      indexLine++;
      int countInput = Convert.ToInt32(lines[indexLine++]);
      int countOutput = Convert.ToInt32(lines[indexLine++]);
      int countWindow = Convert.ToInt32(lines[indexLine++]);
      setting.Title = lines[indexLine++];
      if (version >= 10509) {
        setting.Field = lines[indexLine++];
        setting.Bush = lines[indexLine++];
        setting.Well = lines[indexLine++];
      }
      if (version >= 10511) {
        setting.NKTmm = Convert.ToInt32(lines[indexLine++]);
      }
      if (version >= 10515) {
        setting.LengthPlan = Convert.ToDouble(lines[indexLine++]);
        setting.SpeedPlan = Convert.ToDouble(lines[indexLine++]);
        setting.WaterPlan = Convert.ToDouble(lines[indexLine++]);
      }

      #region Input
      for (int i = 0; i < countInput; i++) {
        string[] dataInput = lines[indexLine++].Split(new string[] { "#," }, StringSplitOptions.None);

        Service.Input input = (Service.Input)(Activator.CreateInstance("Service", "Service." + dataInput[0]).Unwrap());
        ((Service.InputCommon)input).Title = dataInput[1];
        ((Service.InputCommon)input).IsUsed = Convert.ToBoolean(dataInput[2]);
        ((Service.InputCommon)input).InputType = (Service.InputType)Enum.Parse(typeof(Service.InputType), dataInput[3]);
        if (input.GetType() == typeof(Service.InputSerial)) {
          ((Service.InputSerial)input).PortName = dataInput[4];
          ((Service.InputSerial)input).BaudRate = Convert.ToInt32(dataInput[5]);
          ((Service.InputSerial)input).DataBits = Convert.ToInt32(dataInput[6]);
          ((Service.InputSerial)input).Parity = (Parity)Enum.Parse(typeof(Parity), dataInput[7]);
          ((Service.InputSerial)input).StopBits = (StopBits)Enum.Parse(typeof(StopBits), dataInput[8]);
          ((Service.InputSerial)input).Handshake = (Handshake)Enum.Parse(typeof(Handshake), dataInput[9]);
          ((Service.InputSerial)input).ReadBufferSize = Convert.ToInt32(dataInput[10]);
          ((Service.InputSerial)input).ReadTimeout = Convert.ToInt32(dataInput[11]);
          ((Service.InputSerial)input).Dtr = Convert.ToBoolean(dataInput[12]);
          ((Service.InputSerial)input).Rts = Convert.ToBoolean(dataInput[13]);
          ((Service.InputSerial)input).SymbolSplitter = dataInput[14];
          ((Service.InputCommon)input).IDSlave = Convert.ToInt32(dataInput[15]);
        } else {
          ((Service.InputEthernet)input).IPSlave = dataInput[4];
          ((Service.InputEthernet)input).Port = Convert.ToInt32(dataInput[5]);
          ((Service.InputCommon)input).IDSlave = Convert.ToInt32(dataInput[6]);
        }
        int countParam = Convert.ToInt32(lines[indexLine++]);
        for (int j = 0; j < countParam; j++) {
          string[] dataParam = lines[indexLine++].Split(new string[] { "#," }, StringSplitOptions.None);
          Service.DataParam param = new Service.DataParam() {
            ID = Convert.ToInt32(dataParam[0]),
            Title = dataParam[1],
            ParamType = settingUnits.GetParamTypeByTitle(dataParam[2]),
            SlaveID = Convert.ToInt32(dataParam[4]),
            Address = Convert.ToDouble(dataParam[5]),
            Type = (Service.ModbusType)Enum.Parse(typeof(Service.ModbusType), dataParam[6]),
            Command = (Service.ModbusCommandInput)Enum.Parse(typeof(Service.ModbusCommandInput), dataParam[7]),
            AlarmMin = Convert.ToSingle(dataParam[8]),
            AlarmMax = Convert.ToSingle(dataParam[9]),
            IsRight = Convert.ToBoolean(dataParam[10]),
            ColorLine = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dataParam[11]))
          };
          param.ParamUnit = settingUnits.GetUnitByTitle(param.ParamType, dataParam[3]);
          ((Service.InputCommon)input).ListDataParams.Add(param);
        }
        setting.Inputs.Add(input);
      }
      #endregion

      #region Outputs
      for (int i = 0; i < countOutput; i++) {
        string[] dataInput = lines[indexLine++].Split(new string[] { "#," }, StringSplitOptions.None);
        Service.Input input = (Service.Input)(Activator.CreateInstance("Service", "Service." + dataInput[0]).Unwrap());
        ((Service.InputCommon)input).Title = dataInput[1];
        ((Service.InputCommon)input).IsUsed = Convert.ToBoolean(dataInput[2]);
        ((Service.InputCommon)input).InputType = (Service.InputType)Enum.Parse(typeof(Service.InputType), dataInput[3]);
        if (input.GetType() == typeof(Service.InputSerial)) {
          ((Service.InputSerial)input).PortName = dataInput[4];
          ((Service.InputSerial)input).BaudRate = Convert.ToInt32(dataInput[5]);
          ((Service.InputSerial)input).DataBits = Convert.ToInt32(dataInput[6]);
          ((Service.InputSerial)input).Parity = (Parity)Enum.Parse(typeof(Parity), dataInput[7]);
          ((Service.InputSerial)input).StopBits = (StopBits)Enum.Parse(typeof(StopBits), dataInput[8]);
          ((Service.InputSerial)input).Handshake = (Handshake)Enum.Parse(typeof(Handshake), dataInput[9]);
          ((Service.InputSerial)input).ReadBufferSize = Convert.ToInt32(dataInput[10]);
          ((Service.InputSerial)input).ReadTimeout = Convert.ToInt32(dataInput[11]);
          ((Service.InputSerial)input).Dtr = Convert.ToBoolean(dataInput[12]);
          ((Service.InputSerial)input).Rts = Convert.ToBoolean(dataInput[13]);
          ((Service.InputSerial)input).SymbolSplitter = dataInput[14];
          ((Service.InputCommon)input).IDSlave = Convert.ToInt32(dataInput[15]);
        } else {
          ((Service.InputEthernet)input).IPSlave = dataInput[4];
          ((Service.InputEthernet)input).Port = Convert.ToInt32(dataInput[5]);
          ((Service.InputCommon)input).IDSlave = Convert.ToInt32(dataInput[6]);
        }
        int countParam = Convert.ToInt32(lines[indexLine++]);
        for (int j = 0; j < countParam; j++) {
          string[] dataParam = lines[indexLine++].Split(new string[] { "#," }, StringSplitOptions.None);
          Service.DataParamOutput param = new Service.DataParamOutput() {
            ID = Convert.ToInt32(dataParam[0]),
            Title = dataParam[1],
            Address = Convert.ToDouble(dataParam[2]),
          };
          try {
            param.ParamUnitTitle = dataParam[3];
          } catch { }

          ((Service.InputCommon)input).ListDataParamsOut.Add(param);
        }
        setting.Outputs.Add(input);
      }
      #endregion

      #region Windows
      for (int i = 0; i < countWindow; i++) {
        SparkControls.WindowIndicators window = new SparkControls.WindowIndicators();
        window.Title = lines[indexLine++];
        int countIndi = Convert.ToInt32(lines[indexLine++]);
        for (int j = 0; j < countIndi; j++) {
          string[] dataIndi = lines[indexLine++].Split(new string[] { "#," }, StringSplitOptions.None);
          int countDataParam = Convert.ToInt32(dataIndi[0]);
          SparkControls.Indicator indi = (SparkControls.Indicator)(Activator.CreateInstance("SparkControls", dataIndi[1]).Unwrap());
          indi.Size = new System.Windows.Size(Convert.ToDouble(dataIndi[2]), Convert.ToDouble(dataIndi[3]));
          indi.Location = new System.Windows.Point(Convert.ToDouble(dataIndi[4]), Convert.ToDouble(dataIndi[5]));
          indi.CountDot = Convert.ToInt32(dataIndi[6]);
          if (indi.GetType() == typeof(SparkControls.IndiGraph)) {
            ((SparkControls.IndiGraph)indi).GraphSetting.AxisSizeFont = Convert.ToInt32(dataIndi[7]);
            ((SparkControls.IndiGraph)indi).GraphSetting.LegendSizeFont = Convert.ToInt32(dataIndi[8]);
            ((SparkControls.IndiGraph)indi).GraphSetting.LegendVis = Convert.ToBoolean(dataIndi[9]);
            ((SparkControls.IndiGraph)indi).GraphSetting.LineWidth = Convert.ToInt32(dataIndi[10]);
            ((SparkControls.IndiGraph)indi).GraphSetting.LegendPos = (OxyPlot.LegendPosition)Enum.Parse(typeof(OxyPlot.LegendPosition), dataIndi[11]);
            if (dataIndi.Length > 13) {
              indi.MinValue = Convert.ToDouble(dataIndi[12]);
              indi.MaxValue = Convert.ToDouble(dataIndi[13]);
            }
          } else {
            if (dataIndi.Length > 8) {
              indi.MinValue = Convert.ToDouble(dataIndi[7]);
              indi.MaxValue = Convert.ToDouble(dataIndi[8]);
            }
          }
          if (indi.GetType() == typeof(SparkControls.IndiSendStart)) {
            ((SparkControls.IndiSendStart)indi).Input = setting.GetInputByTitle(dataIndi[9]);
          }
          if (indi.GetType() == typeof(SparkControls.IndiSendSoOn)) {
            ((SparkControls.IndiSendSoOn)indi).Input = setting.GetInputByTitle(dataIndi[9]);
          }
          if (indi.GetType() == typeof(SparkControls.IndiInputStatus)) {
            ((SparkControls.IndiInputStatus)indi).TimerInterval = Convert.ToInt32(dataIndi[9]);
            ((SparkControls.IndiInputStatus)indi).Input = setting.GetInputByTitle(dataIndi[10]);
          }
          for (int p = 0; p < countDataParam; p++) {
            Service.DataParam param = setting.GetParamByID(Convert.ToInt32(lines[indexLine++]));
            if (param != null)
              indi.DataParams.Add(param);
          }
          window.ListIndicators.Add(indi);
        }
        setting.Windows.Add(window);
      }
      #endregion

      return setting;
    }

    public static SettingUnits Units(string data) {
      SettingUnits settingUnits = new SettingUnits();
      string[] lines = data.Split('\n');
      for (int i = 0; i < lines.Length; i++) {
        if (lines[i].Length < 1) break;
        string[] line = lines[i].Split(new string[] { "#:" }, StringSplitOptions.None);
        string[] dateType = line[0].Split(new string[] { "#," }, StringSplitOptions.None);
        settingUnits.ParamsTypes.Add(new Service.ParamType { Title = dateType[0] });
        string[] units = line[1].Split(new string[] { "#;" }, StringSplitOptions.None);
        for (int j = 0; j < units.Length; j++) {
          if (units[j].Length < 1) break;
          string[] dataUnit = units[j].Split(new string[] { "#," }, StringSplitOptions.None);
          settingUnits.ParamsTypes[i].ListUnits.Add(new Service.ParamUnit { Title = dataUnit[0] });
        }
      }
      if (settingUnits.ParamsTypes.Count == 0) {
        settingUnits.ParamsTypes.Add(new Service.ParamType {
          Title = "Кол-во труб"
        });
        settingUnits.ParamsTypes[0].ListUnits = new ObservableCollection<Service.ParamUnit>();
        settingUnits.ParamsTypes[0].ListUnits.Add(new Service.ParamUnit {
          Title = "шт"
        });
      }
      bool add = true;
      foreach (Service.ParamType type in settingUnits.ParamsTypes) {
        if (type.Title == "Кол-во труб") {
          add = false;
          break;
        }
      }
      if (add) {
        settingUnits.ParamsTypes.Insert(0, new Service.ParamType {
          Title = "Кол-во труб"
        });
        settingUnits.ParamsTypes[0].ListUnits = new ObservableCollection<Service.ParamUnit>();
        settingUnits.ParamsTypes[0].ListUnits.Add(new Service.ParamUnit {
          Title = "шт"
        });
      }

      return settingUnits;
    }

    public static SettingCommon SettingCommon(string data) {
      SettingCommon settingCommon = new Core.SettingCommon();
      string[] lines = data.Split('\n');
      int bias = Convert.ToInt32(lines[0][0]);
      for (int i = 1; i < lines[0].Length; i++) {
        settingCommon.PassAdmin += ((char)(lines[0][i] - bias)).ToString();
      }
      int indexLine = 1;
      settingCommon.graphSetting.AxisSizeFont = Convert.ToInt32(lines[indexLine++]);
      settingCommon.graphSetting.LegendPos = (OxyPlot.LegendPosition)Enum.Parse(typeof(OxyPlot.LegendPosition), lines[indexLine++]);
      settingCommon.graphSetting.LegendSizeFont = Convert.ToInt32(lines[indexLine++]);
      settingCommon.graphSetting.LegendVis = Convert.ToBoolean(lines[indexLine++]);
      settingCommon.graphSetting.LineWidth = Convert.ToInt32(lines[indexLine++]);
      settingCommon.IsWindowMode = Convert.ToBoolean(lines[indexLine++]);
      settingCommon.IsAutorunWin = Convert.ToBoolean(lines[indexLine++]);
      settingCommon.IsRunModules = Convert.ToBoolean(lines[indexLine++]);
      settingCommon.PathConfig = lines[indexLine++];
      settingCommon.MaxColor = (Color)ColorConverter.ConvertFromString(lines[indexLine++]);
      settingCommon.MinColor = (Color)ColorConverter.ConvertFromString(lines[indexLine++]);
      try {
        settingCommon.Dark = Convert.ToBoolean(lines[indexLine++]);
      } catch { }
      return settingCommon;
    }
  }
}
