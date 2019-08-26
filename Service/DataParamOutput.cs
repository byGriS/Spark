using System.ComponentModel;

namespace Service {
  public class DataParamOutput : INotifyPropertyChanged {
    public int ID { get; set; }

    private string title = "";
    public string Title {
      get { return title; }
      set {
        title = value;
        OnPropertyChanged("Title");
      }
    }

    private double address = 0;
    public double Address {
      get { return address; }
      set {
        address = value;
        OnPropertyChanged("Address");
      }
    }

    private string paramUnitTitle = "";
    public string ParamUnitTitle {
      get { return paramUnitTitle; }
      set {
        paramUnitTitle = value;
        OnPropertyChanged("ParamUnitTitle");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public DataParamOutput Clone() {
      DataParamOutput dp = new DataParamOutput() {
        ID = this.ID,
        Title = this.Title,
        Address = this.Address,
        ParamUnitTitle = this.ParamUnitTitle
      };
      return dp;
    }
  }
}
