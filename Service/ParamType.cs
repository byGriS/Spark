using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Service {
  public class ParamType : INotifyPropertyChanged {
    public string Title { get; set; }
    public ObservableCollection<ParamUnit> ListUnits = new ObservableCollection<ParamUnit>();

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public ParamType Clone() {
      ParamType pt = new ParamType() {
        Title = this.Title
      };
      foreach(ParamUnit pu in this.ListUnits) {
        pt.ListUnits.Add(pu.Clone());
      }
      return pt;
    }

    public override bool Equals(object obj) {
      if (obj == null || !(obj is ParamType))
        return false;
      return (((ParamType)obj).Title == this.Title);
    }
  }
}
