using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PipeCounter {

  public partial class MainWindow : Window {

    private string pathSelectedFolder = "";
    private ObservableCollection<DirectoryInfo> selectedDirs = new ObservableCollection<DirectoryInfo>();
    private ObservableCollection<FileInfo> selectedFiles = new ObservableCollection<FileInfo>();
    private Setting setting = new Setting();
    public Setting Setting {
      get { return setting; }
      set { setting = value; }
    }

    private int positionLength = 0;

    private bool statusPipe = false;
    public bool StatusPipe {
      get { return statusPipe; }
      set {
        statusPipe = value;
        statusPipeLabel.Content = (statusPipe) ? "Есть" : "Нет";
        startButton.IsEnabled = statusPipe;
        reportButton.IsEnabled = statusPipe;
      }
    }

    public MainWindow() {
      InitializeComponent();
      this.DataContext = this;
      Refresh_Click(listDir, null);
    }

    private void listDir_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (((ListBox)sender).SelectedItem == null)
        return;
      DirectoryInfo selectedDir = (DirectoryInfo)((ListBox)sender).SelectedItem;
      pathSelectedFolder = selectedDir.FullName;
      FileInfo[] filesInfo = selectedDir.GetFiles();
      listFiles.ItemsSource = filesInfo;
      selectedDirs.Clear();
      foreach (object arg in ((ListBox)sender).SelectedItems) {
        selectedDirs.Add((DirectoryInfo)arg);
      }
    }

    private void listFiles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      StatusPipe = false;
      if ((((ListBox)sender).SelectedItems == null) || (((ListBox)sender).SelectedItems.Count == 0))
        return;
      IList selectedItems = ((ListBox)sender).SelectedItems;
      IEnumerable<FileInfo> test = selectedItems.OfType<FileInfo>().OrderBy(f => f.Name);
      selectedItems = test.ToList();
      selectedFiles.Clear();

      foreach (object arg in selectedItems) {
        selectedFiles.Add((FileInfo)arg);
        if (StatusPipe)
          continue;
        string[] lines = File.ReadAllLines(((FileInfo)arg).FullName);
        string[] paramsStr = lines[0].Split(';');
        for (int i = 0; i < paramsStr.Length; i++) {
          if (paramsStr[i] == "Длина,м") {
            StatusPipe = true;
            positionLength = i;
            break;
          }
        }
      }

    }

    private void Refresh_Click(object sender, RoutedEventArgs e) {
      string thisDir = Directory.GetCurrentDirectory();
      DirectoryInfo[] allDirInfo = new DirectoryInfo(thisDir).GetDirectories();
      listDir.ItemsSource = allDirInfo;
      listFiles.ItemsSource = null;
    }

    private void Start_Click(object sender, RoutedEventArgs e) {
      StartCounter(false);
    }

    private void Report_Click(object sender, RoutedEventArgs e) {
      StartCounter(true);
    }

    private void StartCounter(bool report) {
      string path = Directory.GetCurrentDirectory() + '\\' + selectedDirs[0].Name + "_pipe";
      if (report) Directory.CreateDirectory(path);
      Setting.CountPipe = 0;
      DateTime startTime = new DateTime(0);
      bool pipeUp = true;
      bool isPipe = false;
      double pipeLengthPre = -1;
      double pipeLengthStart = -1;
      foreach (FileInfo arg in selectedFiles) {
        string[] lines = File.ReadAllLines(((FileInfo)arg).FullName);
        FileStream fs = null;
        StreamWriter sw = null;
        string filePath = path + "\\" + arg.Name.Split('.')[0] + "_pipe.csv";
        if (report) {
          fs = new FileStream(filePath, FileMode.OpenOrCreate);
          sw = new StreamWriter(fs, Encoding.Unicode);
        }
        string value = "";

        for (int i = 0; i < lines.Length; i++) {
          if (i == 0) {
            value = lines[0] + "Кол-во труб,шт;";
            if (report) sw.WriteLine(value);
            continue;
          }
          string[] paramsStr = lines[i].Split(';');
          double pipeLength = Math.Abs(Convert.ToDouble(paramsStr[positionLength].Replace('.', ',')));
          if (pipeLengthPre == -1) {
            pipeLengthPre = pipeLength;
            pipeLengthStart = pipeLength;
            startTime = DateTime.Parse(paramsStr[0]);
          }

          if (pipeLength > pipeLengthPre) {
            pipeUp = true;
            if (pipeLengthPre - pipeLengthStart > Setting.PipeLengthTo) {
              Setting.CountPipe++;
              pipeLengthStart = pipeLength;
            }
            if (pipeLengthPre - pipeLengthStart > Setting.PipeLengthFrom) {
              isPipe = true;
            } else {
              isPipe = false;
            }
            startTime = DateTime.Parse(paramsStr[0]);
          }
          if (pipeLength < pipeLengthPre) {
            pipeUp = false;
            if (pipeLengthStart - pipeLengthPre > Setting.PipeLengthTo) {
              Setting.CountPipe--;
              pipeLengthStart = pipeLength;
            }
            if (pipeLengthStart - pipeLengthPre > Setting.PipeLengthFrom) {
              isPipe = true;
            } else {
              isPipe = false;
            }
            startTime = DateTime.Parse(paramsStr[0]);
          }
          if (pipeLength == pipeLengthPre) {
            double subTime = DateTime.Parse(paramsStr[0]).Subtract(startTime).TotalSeconds;
            if (subTime > Setting.PipeTime) {
              if (isPipe) {
                if (pipeUp)
                  Setting.CountPipe++;
                else
                  Setting.CountPipe--;
                pipeLengthStart = pipeLength;
                isPipe = false;
              }
            }
          }
          pipeLengthPre = pipeLength;
          value = lines[i] + ";" + Setting.CountPipe.ToString() + ";";
          if (report) sw.WriteLine(value);
        }
        if (report) {
          sw.Flush();
          fs.Close();
        }
      }
    }

    private void pipeTime_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      var textBox = sender as TextBox;
      e.Handled = System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[^0-9]+");
    }
  }
}

