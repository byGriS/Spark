using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Updater {
  class Program {

    private static string EnvPath = "";

    static void Main(string[] args) {
      if (args.Length == 1)
        return;
      try {
        EnvPath = args[1];
      } catch { }
      Console.WriteLine("Обновление:");
      if (args[0] == "d") {
        try {
          CloseSpark();
          Console.WriteLine("5%");
          DeleteFile("Newtonsoft.Json.dll");
          Console.WriteLine("10%");
          DeleteFile("Core.dll");
          Console.WriteLine("15%");
          DeleteFile("Service.dll");
          Console.WriteLine("20%");
          DeleteFile("SparkControls.dll");
          Console.WriteLine("25%");
          DeleteFile("Spark.exe");
          Console.WriteLine("30%");
          DeleteFile("SparkInput.exe");
          Console.WriteLine("40%");
          DeleteFile("SparkOutput.exe");
          Console.WriteLine("45%");
          DeleteFile("SReport.exe");
          Console.WriteLine("50%");


          DownloadFile("Newtonsoft.Json.dll");
          Console.WriteLine("55%");
          DownloadFile("Core.dll");
          Console.WriteLine("60%");
          DownloadFile("SReport.exe");
          Console.WriteLine("65%");
          DownloadFile("Service.dll");
          Console.WriteLine("70%");
          DownloadFile("SparkControls.dll");
          Console.WriteLine("75%");
          DownloadFile("Spark.exe");
          Console.WriteLine("78%");
          DownloadFile("SparkInput.exe");
          Console.WriteLine("85%");
          DownloadFile("SparkOutput.exe");
          Console.WriteLine("95%");
          Process start = new Process();
          start.StartInfo.FileName = "Искра.exe";
          start.Start();
          Console.WriteLine("100%");
          Console.WriteLine("Обновление успешно завершено");
        }catch(Exception ex) {
          string s = DateTime.Now.ToString("########## dd.MM.yyyy HH:mm:ss");
          string path = Directory.GetCurrentDirectory();
          StreamWriter sw = new StreamWriter(path + "\\errors.txt", true, Encoding.Unicode);
          sw.WriteLine(s + "\n " + ex.Message + "\n" + ex.ToString());
          sw.Close();
        }
      }
    }

    static private void DownloadFile(string fileName) {
      string remoteUri = "http://spautomation.ru/spark/";
      string myStringWebResource = null;
      WebClient myWebClient = new WebClient();
      myWebClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
      myStringWebResource = remoteUri + fileName;
      myWebClient.DownloadFile(myStringWebResource, EnvPath + fileName);
    }

    static private void CloseSpark() {
      foreach (Process proc in Process.GetProcessesByName("Искра")) {
        proc.Kill();
      }
      foreach (Process proc in Process.GetProcessesByName("Spark")) {
        proc.Kill();
      }
      foreach (Process proc in Process.GetProcessesByName("SparkInput")) {
        proc.Kill();
      }
      foreach (Process proc in Process.GetProcessesByName("SparkOutput")) {
        proc.Kill();
      }
      foreach (Process proc in Process.GetProcessesByName("SReport")) {
        proc.Kill();
      }
      foreach (Process proc in Process.GetProcessesByName("EmulPipes")) {
        proc.Kill();
      }
    }

    static private void DeleteFile(string fileName) {
      if (File.Exists(fileName)) {
        for (int i = 0; i < 10; i++) {
          try {
            File.Delete(EnvPath + fileName);
            break;
          } catch {
            Thread.Sleep(1000);
          }
        }
      }
    }
  }
}
