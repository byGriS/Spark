using Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SparkControls {
  public partial class IndiManyParam : UserControl, Indicator {

    public int CountDot { get; set; }
    public Point Location { get; set; }
    public Size Size { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    private ObservableCollection<DataParam> dataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> DataParams { get { return dataParams; } set { dataParams = value; } }

    public IndiManyParam() {
      InitializeComponent();
      DataParams.CollectionChanged += DataParams_CollectionChanged;
      if (dataParams.Count > 0)
        DataContext = dataParams;
      else
        DataContext = null;
    }

    public TypeIndicator TypeIndicator { get { return TypeIndicator.IndiManyParam; } }
    WindowSetting Indicator.IndiSetting {
      get {
        WindowSetting windowSetting = new IndiManyParamSet();
        windowSetting.Indicator = this;
        return windowSetting;
      }
    }

    private void DataParams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
        DataContext = dataParams;
      }
    }

    public void UpdateBindingValue() {
    }

    private void ShowParams_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
      DataGrid Dg = sender as DataGrid;
      if ((Dg != null) && (e.AddedCells.Count > 0)) {
        Dg.SelectedCells.Clear();
      }
    }
  }
}
