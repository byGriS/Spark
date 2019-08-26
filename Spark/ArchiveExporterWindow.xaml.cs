using System;
using System.IO;
using System.Windows;

namespace Spark {
  public partial class ArchiveExporterWindow : Window {

    private ArchiveExportData archiveExportData = null;

    public ArchiveExporterWindow(ArchiveExportData archiveExportData) {
      InitializeComponent();
      this.archiveExportData = archiveExportData;
      tbPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      dtpStart.Value = archiveExportData.Start;
      dtpFinish.Value = archiveExportData.Finish;
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      archiveExportData.PathFolder = tbPath.Text;
      archiveExportData.Start = dtpStart.Value.Value;
      archiveExportData.Finish = dtpFinish.Value.Value;
      switch (cbInterval.SelectedIndex) {
        case 0:
          archiveExportData.Interval = 60;
          break;
        case 1:
          archiveExportData.Interval = 300;
          break;
        case 2:
          archiveExportData.Interval = 1800;
          break;
        case 3:
          archiveExportData.Interval = 3600;
          break;
        case 4:
          archiveExportData.Interval = 21600;
          break;
        case 5:
          archiveExportData.Interval = 86400;
          break;
      }
      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      archiveExportData.PathFolder = "";
      this.Close();
    }

    private void FolderClick(object sender, RoutedEventArgs e) {
      System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
      if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        tbPath.Text = fbd.SelectedPath;
      }
    }
  }

  public class ArchiveExportData {
    public string PathFolder { get; set; }
    public int Interval { get; set; }
    public DateTime Start { get; set; }
    public DateTime Finish { get; set; }

  }
}
