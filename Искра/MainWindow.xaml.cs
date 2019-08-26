using System.Windows;
using System.Timers;
using System.Diagnostics;
using System.IO;
using System;

namespace Искра {
  public partial class MainWindow : Window {

    public Core.Work work = new Core.Work();

    public MainWindow() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      Timer startSpark = new Timer(1000);
      startSpark.Enabled = true;
      startSpark.AutoReset = false;
      startSpark.Elapsed += StartSpark_Elapsed;
      //work.StartApp();
    }

    private void StartSpark_Elapsed(object sender, ElapsedEventArgs e) {
      //string path = Path.GetFullPath("../../../Spark/bin/Debug/Spark.exe");
      string path = Path.GetFullPath("Spark.exe");
      if (File.Exists(path)) {
        Process startSpark = new Process();
        startSpark.StartInfo.FileName = path;
        startSpark.Start();
      } else {
        Service.Log.LogShow(Core.Work.EnvPath, "Не все файлы\nПереустановите приложение", "Not file Spark.exe", "Ошибка", Service.MessageViewMode.Error);
        work.CloseApp();
      }
      Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
        new Action(() => { this.Close(); })
        );
    }
  }
}
