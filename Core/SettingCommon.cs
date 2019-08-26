using System.ComponentModel;
using System.Windows.Media;

namespace Core {
  public class SettingCommon : INotifyPropertyChanged {

    private string passAdmin = "";
    public string PassAdmin {
      get { return passAdmin; }
      set {
        passAdmin = value;
        OnPropertyChanged("PassAdmin");
      }
    }

    public Service.GraphSetting graphSetting = new Service.GraphSetting();

    public bool IsWindowMode { get; set; }

    public bool IsAutorunWin { get; set; }

    public bool IsRunModules { get; set; }

    private string pathConfig = "";
    public string PathConfig {
      get { return pathConfig; }
      set {
        pathConfig = value;
        OnPropertyChanged("PathConfig");
      }
    }

    public Color MinColor = Color.FromRgb(136, 91, 0);
    public Color MaxColor = Color.FromRgb(140, 0, 0);

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public bool changed { get; set; }

    public SettingCommon Clone() {
      SettingCommon sc = new SettingCommon();
      sc.PassAdmin = this.PassAdmin;
      sc.graphSetting = this.graphSetting.Clone();
      sc.IsWindowMode = this.IsWindowMode;
      sc.IsAutorunWin = this.IsAutorunWin;
      sc.IsRunModules = this.IsRunModules;
      sc.PathConfig = this.PathConfig;
      sc.MinColor = Color.FromRgb(this.MinColor.R, this.MinColor.G, this.MinColor.B);
      sc.MaxColor = Color.FromRgb(this.MaxColor.R, this.MaxColor.G, this.MaxColor.B);
      return sc;
    }
  }
}
