using System.ComponentModel;

namespace PipeCounter {
  public class Setting : INotifyPropertyChanged {

    private double pipeLengthFrom = 7;
    public double PipeLengthFrom {
      get { return pipeLengthFrom; }
      set {
        pipeLengthFrom = value;
        NotifyPropertyChanged("PipeLengthFrom");
      }
    }

    private double pipeLengthTo = 12;
    public double PipeLengthTo {
      get { return pipeLengthTo; }
      set {
        pipeLengthTo = value;
        NotifyPropertyChanged("PipeLengthTo");
      }
    }

    private int pipeTime = 40;
    public int PipeTime {
      get { return pipeTime; }
      set {
        pipeTime = value;
        NotifyPropertyChanged("PipeTime");
      }
    }

    private int countPipe = 0;
    public int CountPipe {
      get { return countPipe; }
      set {
        countPipe = value;
        NotifyPropertyChanged("CountPipe");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
