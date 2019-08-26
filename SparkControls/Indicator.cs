using Service;
using System.Collections.ObjectModel;
using System.Windows;

namespace SparkControls {
  public interface Indicator {
    Point Location { get; set; }
    Size Size { get; set; }
    int CountDot { get; set; }

    double MinValue { get; set; }
    double MaxValue { get; set; }

    ObservableCollection<DataParam> DataParams { get; set; }

    TypeIndicator TypeIndicator { get; }
    WindowSetting IndiSetting { get; }

    void UpdateBindingValue();
  }

  public enum TypeIndicator {
    IndiDigital,
    IndiManyParam,
    IndiColumn,
    IndiArrow,
    IndiGraph,
    IndiBoolean,
    IndiToggle,
    IndiSendStart,
    IndiSendSoOn,
    IndiInputStatus
  }
}
