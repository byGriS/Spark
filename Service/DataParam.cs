using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Service {
  public class DataParam : INotifyPropertyChanged {
    public delegate bool ChangeTitle(DataParam dataParam, string oldTitle, string newTitle);
    public delegate void ChangeValue(DataParam dataParam);
    public event ChangeTitle OnChangeTitle;
    public event ChangeValue OnChangeValue;

    public int ID { get; set; }

    private string title = "";
    public string Title {
      get { return title; }
      set {
        if (title != value)
          if ((OnChangeTitle?.Invoke(this, title, value) ?? true))
            title = value;
        OnPropertyChanged("Title");
      }
    }

    private ParamType paramType = new ParamType();
    public ParamType ParamType {
      get { return paramType; }
      set {
        paramType = value;
        OnPropertyChanged("ParamType");
      }
    }

    private ParamUnit paramUnit = new ParamUnit();
    public ParamUnit ParamUnit {
      get { return paramUnit; }
      set {
        paramUnit = value;
        OnPropertyChanged("ParamUnit");
      }
    }

    public int SlaveID { get; set; }
    public double Address { get; set; }
    public ModbusType Type { get; set; }
    public ModbusCommandInput Command { get; set; }

    private ObservableCollection<ParamType> listParamsTypes = new ObservableCollection<ParamType>();
    public ObservableCollection<ParamType> ListParamsTypes {
      get { return listParamsTypes; }
      set {
        listParamsTypes = value;
        OnPropertyChanged("ListParamsTypes");
      }
    }

    private ObservableCollection<ParamUnit> listParamsUnits = new ObservableCollection<ParamUnit>();
    public ObservableCollection<ParamUnit> ListParamsUnits {
      get { return listParamsUnits; }
      set {
        listParamsUnits = value;
        OnPropertyChanged("ListParamsUnits");
      }
    }

    private float value = 0;
    public float Value {
      get { return value; }
      set {
        this.value = value;
        OnPropertyChanged("Value");
        OnChangeValue?.Invoke(this);
      }
    }

    private float alarmMin = 0;
    public float AlarmMin {
      get { return alarmMin; }
      set {
        this.alarmMin = value;
        OnPropertyChanged("AlarmMin");
        OnChangeValue?.Invoke(this);
      }
    }

    private float alarmMax = 0;
    public float AlarmMax {
      get { return alarmMax; }
      set {
        this.alarmMax = value;
        OnPropertyChanged("AlarmMax");
        OnChangeValue?.Invoke(this);
      }
    }

    private SolidColorBrush colorLine = Brushes.Black;
    public SolidColorBrush ColorLine { get { return colorLine; }set { colorLine = value; } }

    public bool IsRight { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public DataParam Clone() {
      DataParam dp = new DataParam() {
        ID = this.ID,
        Title = this.Title,
        SlaveID = this.SlaveID,
        Address = this.Address,
        Command = this.Command,
        Type = this.Type,
        AlarmMin = this.AlarmMin,
        AlarmMax = this.AlarmMax,
        ColorLine = this.ColorLine,
        IsRight = this.IsRight
      };
      dp.ParamType = this.ParamType.Clone();
      if (this.ParamUnit != null)
        dp.ParamUnit = this.ParamUnit.Clone();
      return dp;
    }
  }
}
