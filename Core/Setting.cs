using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Core {
  public class Setting : INotifyPropertyChanged {
    public delegate void InputIsStart(bool value);
    public delegate void OutputIsStart(bool value);

    public event InputIsStart OnInputIsStart;
    public event OutputIsStart OnOutputIsStart;

    #region Params
    public int Version {
      get { return 10515; }
    }
    public int CountInput {
      get { return Inputs.Count; }
    }
    public int CountOutput {
      get { return Outputs.Count; }
    }
    public int CountWindow {
      get { return Windows.Count; }
    }

    private string title = "Бригада 1";
    public string Title {
      get { return title; }
      set {
        title = value;
        OnPropertyChanged("Title");
      }
    }

    public string Field { get; set; }
    public string Bush { get; set; }
    public string Well { get; set; }
    public int NKTmm { get; set; }
    public double LengthPlan { get; set; }
    public double SpeedPlan { get; set; }
    public double WaterPlan { get; set; }
    public int NumWork { get; set; }

    private bool inputIsStarted = false;
    public bool InputIsStarted {
      get { return inputIsStarted; }
      set {
        inputIsStarted = value;
        OnInputIsStart?.Invoke(inputIsStarted);
      }
    }

    private bool outputIsStarted = false;
    public bool OutputIsStarted {
      get { return outputIsStarted; }
      set {
        outputIsStarted = value;
        OnOutputIsStart?.Invoke(outputIsStarted);
      }
    }

    private ObservableCollection<Service.Input> inputs = new ObservableCollection<Service.Input>();
    public ObservableCollection<Service.Input> Inputs { get { return inputs; } set { inputs = value; } }
    public ObservableCollection<Service.Input> CloneInputs() {
      ObservableCollection<Service.Input> result = new ObservableCollection<Service.Input>();
      foreach (Service.Input input in this.Inputs) {
        result.Add(input.Clone());
      }
      return result;
    }

    public ObservableCollection<Service.DataParam> GetListInputParams() {
      ObservableCollection<Service.DataParam> list = new ObservableCollection<Service.DataParam>();
      foreach (Service.Input input in Inputs) {
        foreach (Service.DataParam param in ((Service.InputCommon)input).ListDataParams) {
          list.Add(param);
        }
      }
      return list;
    }
    public Service.DataParam GetParamByID(int ID) {
      ObservableCollection<Service.DataParam> list = this.GetListInputParams();
      foreach (Service.DataParam param in list)
        if (param.ID == ID)
          return param;
      return null;
    }

    private ObservableCollection<Service.Input> outputs = new ObservableCollection<Service.Input>();
    public ObservableCollection<Service.Input> Outputs { get { return outputs; } set { outputs = value; } }
    public ObservableCollection<Service.Input> CloneOutputs() {
      ObservableCollection<Service.Input> result = new ObservableCollection<Service.Input>();
      foreach (Service.Input input in this.Outputs) {
        result.Add(input.Clone());
      }
      return result;
    }

    private ObservableCollection<SparkControls.WindowIndicators> windows = new ObservableCollection<SparkControls.WindowIndicators>();
    public ObservableCollection<SparkControls.WindowIndicators> Windows {
      get { return windows; }
      set { windows = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    private bool isSendToServer = false;
    public bool IsSendToServer {
      get { return isSendToServer; }
      set { isSendToServer = value; }
    }

    #endregion

    #region Methods
    public void UpdateIndicators() {
      foreach (SparkControls.WindowIndicators window in Windows) 
        foreach (SparkControls.Indicator indicator in window.ListIndicators)
          for (int i = 0; i < indicator.DataParams.Count; i++) {
            bool loop = true;
            foreach (Service.InputCommon input in inputs) {
              if (loop) {
                foreach (Service.DataParam param in input.ListDataParams) {
                  if (param.ID == indicator.DataParams[i].ID) {
                    indicator.DataParams[i] = param;
                    indicator.UpdateBindingValue();
                    loop = false;
                    break;
                  }
                }
              } else {
                break;
              }
            }
          }
    }

    public void UpdateWorkListParams() {
      ObservableCollection<Service.DataParam> list = new ObservableCollection<Service.DataParam>();
      foreach(Service.InputCommon input in inputs) {
        foreach(Service.DataParam param in input.ListDataParams) {
          list.Add(param);
        }
      }
      Work.dataParam = list;
    }

    public void UpdateBindPramUnits(SettingUnits settingUnits) {
      ObservableCollection<Service.DataParam> list = this.GetListInputParams();
      foreach (Service.DataParam dataParam in list) {
        bool setType = false;
        bool setUnit = false;
        for (int i = 0; i < settingUnits.ParamsTypes.Count; i++) {
          if (setType) break;
          if (dataParam.ParamType.Title == settingUnits.ParamsTypes[i].Title) {
            dataParam.ParamType = settingUnits.ParamsTypes[i];
            setType = true;
            for (int j = 0; j < settingUnits.ParamsTypes[i].ListUnits.Count; j++) {
              if (setUnit) break;
              if (dataParam.ParamUnit.Title == settingUnits.ParamsTypes[i].ListUnits[j].Title) {
                dataParam.ParamUnit = settingUnits.ParamsTypes[i].ListUnits[j];
                setUnit = true;
              }
            }
          }
        }
        if (!setType) {
          dataParam.ParamType = new Service.ParamType { Title = "" };
        }
        if (!setUnit) {
          dataParam.ParamUnit = new Service.ParamUnit { Title = "" };
        }
      }
    }

    public void UpdateOutputsReferences() {
      foreach(Service.Input output in Outputs) {
        for(int i = 0; i < ((Service.InputCommon)output).ListDataParamsOut.Count;) { 
          Service.DataParam param = GetParamByID(((Service.InputCommon)output).ListDataParamsOut[i].ID);
          if (param == null)
            ((Service.InputCommon)output).ListDataParamsOut.RemoveAt(i);
          else {
            ((Service.InputCommon)output).ListDataParamsOut[i].Title = param.Title;
            i++;
          }
        }
      }
      
    }

    public Service.Input GetInputByTitle(string title) {
      foreach(Service.Input input in this.Inputs)
        if (((Service.InputCommon)input).Title == title) {
          return input;
        }
      return null;
    }

    #endregion
  }
}
