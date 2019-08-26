using System;
using System.IO;
using System.Text;

namespace Service {
  public class Log {
    public static void LogWrite(string path, string message, string error) {
      WriteInFile(path, error, message, null, MessageViewMode.Message);
    }

    public static void LogShow(string path, string message, string error, string caption, MessageViewMode mode) {
      WriteInFile(path, error, message, caption, mode);
    }

    private static void WriteInFile(string path, string error, string message, string caption, MessageViewMode mode) {
      string s = DateTime.Now.ToString("########## dd.MM.yyyy HH:mm:ss");
      StreamWriter sw = new StreamWriter(path + "errors.txt", true, Encoding.Unicode);
      sw.WriteLine(s + "\n " + message + "\n" + error);
      sw.Close();
      if (caption != null) {
        MessageView mv = new MessageView(message, caption, mode);
        mv.ShowDialog();
      }
    }
  }
}
