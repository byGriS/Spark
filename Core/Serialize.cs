using System;

namespace Core {
  public class Serialize {
    public static string Setting(Setting setting) {
      string result = "";
      result += setting.Version + "\n";
      result += setting.CountInput + "\n";
      result += setting.CountOutput + "\n";
      result += setting.CountWindow + "\n";
      result += setting.Title + "\n";
      result += setting.Field + "\n";
      result += setting.Bush + "\n";
      result += setting.Well + "\n";
      result += setting.NKTmm + "\n";
      result += setting.LengthPlan.ToString() + "\n";
      result += setting.SpeedPlan.ToString() + "\n";
      result += setting.WaterPlan.ToString() + "\n";
      #region inputs
      foreach (Service.InputCommon input in setting.Inputs) {
        result += input.GetType().Name + "#,";
        result += input.Title + "#,";
        result += input.IsUsed + "#,";
        result += input.InputType + "#,";
        if (input.GetType() == typeof(Service.InputSerial)) {
          result += ((Service.InputSerial)input).PortName + "#,";
          result += ((Service.InputSerial)input).BaudRate + "#,";
          result += ((Service.InputSerial)input).DataBits + "#,";
          result += ((Service.InputSerial)input).Parity + "#,";
          result += ((Service.InputSerial)input).StopBits + "#,";
          result += ((Service.InputSerial)input).Handshake + "#,";
          result += ((Service.InputSerial)input).ReadBufferSize + "#,";
          result += ((Service.InputSerial)input).ReadTimeout + "#,";
          result += ((Service.InputSerial)input).Dtr + "#,";
          result += ((Service.InputSerial)input).Rts + "#,";
          result += ((Service.InputSerial)input).SymbolSplitter + "#,";
        } else {
          result += ((Service.InputEthernet)input).IPSlave + "#,";
          result += ((Service.InputEthernet)input).Port + "#,";
        }
        result += input.IDSlave + "#,";
        result += "\n";
        result += input.ListDataParams.Count;
        result += "\n";
        foreach(Service.DataParam param in input.ListDataParams) {
          result += param.ID + "#,";
          result += param.Title + "#,";
          result += param.ParamType.Title + "#,";
          result += param.ParamUnit.Title + "#,";
          result += param.SlaveID + "#,";
          result += param.Address + "#,";
          result += param.Type + "#,";
          result += param.Command + "#,";
          result += param.AlarmMin + "#,";
          result += param.AlarmMax + "#,";
          result += param.IsRight + "#,";
          result += param.ColorLine.ToString() + "#,";
          result += "\n";
        }
      }
      #endregion

      #region outputs
      foreach (Service.InputCommon input in setting.Outputs) {
        result += input.GetType().Name + "#,";
        result += input.Title + "#,";
        result += input.IsUsed + "#,";
        result += input.InputType + "#,";
        if (input.GetType() == typeof(Service.InputSerial)) {
          result += ((Service.InputSerial)input).PortName + "#,";
          result += ((Service.InputSerial)input).BaudRate + "#,";
          result += ((Service.InputSerial)input).DataBits + "#,";
          result += ((Service.InputSerial)input).Parity + "#,";
          result += ((Service.InputSerial)input).StopBits + "#,";
          result += ((Service.InputSerial)input).Handshake + "#,";
          result += ((Service.InputSerial)input).ReadBufferSize + "#,";
          result += ((Service.InputSerial)input).ReadTimeout + "#,";
          result += ((Service.InputSerial)input).Dtr + "#,";
          result += ((Service.InputSerial)input).Rts + "#,";
          result += ((Service.InputSerial)input).SymbolSplitter + "#,";
        } else {
          result += ((Service.InputEthernet)input).IPSlave + "#,";
          result += ((Service.InputEthernet)input).Port + "#,";
        }
        result += input.IDSlave + "#,";
        result += "\n";
        result += input.ListDataParamsOut.Count;
        result += "\n";
        foreach (Service.DataParamOutput param in input.ListDataParamsOut) {
          result += param.ID + "#,";
          result += param.Title + "#,";
          result += param.Address + "#,";
          result += param.ParamUnitTitle + "#,";
          result += "\n";
        }
      }
      #endregion

      #region Windows
      foreach (SparkControls.WindowIndicators window in setting.Windows) {
        result += window.Title;
        result += "\n";
        result += window.ListIndicators.Count;
        result += "\n";
        foreach(SparkControls.Indicator indicator in window.ListIndicators) {
          result += indicator.DataParams.Count + "#,";
          result += indicator.GetType() + "#,";
          result += indicator.Size.Width + "#,";
          result += indicator.Size.Height + "#,";
          result += indicator.Location.X + "#,";
          result += indicator.Location.Y + "#,";
          result += indicator.CountDot + "#,";
          if (indicator.GetType() == typeof(SparkControls.IndiGraph)) {
            result += ((SparkControls.IndiGraph)indicator).GraphSetting.AxisSizeFont + "#,";
            result += ((SparkControls.IndiGraph)indicator).GraphSetting.LegendSizeFont + "#,";
            result += ((SparkControls.IndiGraph)indicator).GraphSetting.LegendVis + "#,";
            result += ((SparkControls.IndiGraph)indicator).GraphSetting.LineWidth + "#,";
            result += ((SparkControls.IndiGraph)indicator).GraphSetting.LegendPos.ToString() + "#,";
          }
          result += indicator.MinValue + "#,";
          result += indicator.MaxValue + "#,";
          if (indicator.GetType() == typeof(SparkControls.IndiSendStart)) {
            if (((SparkControls.IndiSendStart)indicator).Input == null)
              result += "#,";
            else
              result += ((Service.InputCommon)((SparkControls.IndiSendStart)indicator).Input).Title + "#,";
          }
          if (indicator.GetType() == typeof(SparkControls.IndiSendSoOn)) {
            if (((SparkControls.IndiSendSoOn)indicator).Input == null)
              result += "#,";
            else
              result += ((Service.InputCommon)((SparkControls.IndiSendSoOn)indicator).Input).Title + "#,";
          }
          if (indicator.GetType() == typeof(SparkControls.IndiInputStatus)) {
            result += ((SparkControls.IndiInputStatus)indicator).TimerInterval + "#,";
            if (((SparkControls.IndiInputStatus)indicator).Input == null)
              result += "#,";
            else
              result += ((Service.InputCommon)((SparkControls.IndiInputStatus)indicator).Input).Title + "#,";
          }
          result += "\n";
          foreach(Service.DataParam dataParam in indicator.DataParams) {
            result += dataParam.ID;
            result += "\n";
          }
        }
      }
      #endregion

      return result;
    }

    public static string Units(SettingUnits settingUnits) {
      string result = "";
      foreach(Service.ParamType type in settingUnits.ParamsTypes) {
        result += type.Title + "#:";
        foreach (Service.ParamUnit unit in type.ListUnits)
          result += unit.Title + "#;";
        result += "\n";
      }
      return result;
    }

    public static string SettingCommon(SettingCommon settingCommon) {
      string result = "";
      int bias = new Random().Next(2, 9);
      result += bias.ToString();
      foreach(char c in settingCommon.PassAdmin) {
        result += ((char)(c + bias)).ToString();
      }
      result += "\n";
      result += settingCommon.graphSetting.AxisSizeFont.ToString() + "\n";
      result += settingCommon.graphSetting.LegendPos.ToString() + "\n";
      result += settingCommon.graphSetting.LegendSizeFont.ToString() + "\n";
      result += settingCommon.graphSetting.LegendVis.ToString() + "\n";
      result += settingCommon.graphSetting.LineWidth.ToString() + "\n";
      result += settingCommon.IsWindowMode + "\n";
      result += settingCommon.IsAutorunWin + "\n";
      result += settingCommon.IsRunModules + "\n";
      result += settingCommon.PathConfig + "\n";
      result += settingCommon.MaxColor.ToString() + "\n";
      result += settingCommon.MinColor.ToString() + "\n";
      result += settingCommon.Dark.ToString() + "\n";
      return result;
    }
  }
}
