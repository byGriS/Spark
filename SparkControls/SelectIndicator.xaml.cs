using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;

namespace SparkControls {
  public partial class SelectIndicator : Window {

    private Indicator indicator;
    public Indicator Indicator {
      get { return indicator; }
      set {indicator = value;}
    }

    public SelectIndicator() {
      InitializeComponent();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      Indicator.Size = new Size(200, 200);
      this.DialogResult = true;
      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.DialogResult = false;
      this.Close();
    }

    private void Indi_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
      Indicator selectedIndi = sender as Indicator;
      foreach (Indicator indi in listIndicators.Children) {
        ((UserControl)indi).BorderBrush = Brushes.LightGray;
      }
        ((UserControl)selectedIndi).BorderBrush = Brushes.Blue;
      btnOk.IsEnabled = true;
      Indicator = (Indicator)Activator.CreateInstance(selectedIndi.GetType());
    }
  }
}
