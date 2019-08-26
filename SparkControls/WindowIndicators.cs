using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SparkControls {
  public class WindowIndicators : Canvas {
    public string Title { get; set; }

    private ObservableCollection<Indicator> listIndicator = new ObservableCollection<Indicator>();
    public ObservableCollection<Indicator> ListIndicators { get { return listIndicator; } }
  }
}
