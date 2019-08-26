using System.IO;
using System.Text;

namespace Service {
  public class WorkFile {
    private static object accessMethod = new object();

    public static object Do(string path, WorkFileMode mode, string value = null) {
      lock (accessMethod) {
        try {
          switch (mode) {
            case WorkFileMode.ReadAllText:
              if (File.Exists(path))
                try {
                  return File.ReadAllText(path);
                } catch { return ""; } else
                return "";
            case WorkFileMode.ReadArrayString:
              if (File.Exists(path))
                return File.ReadAllLines(path);
              else
                return "";
            default:
              if (mode == WorkFileMode.WriteNew)
                File.WriteAllText(path, string.Empty);
              FileStream fs;
              StreamWriter sw;
              fs = new FileStream(path, FileMode.Append);
              sw = new StreamWriter(fs, Encoding.Unicode);
              sw.Write(value);
              sw.Flush();
              fs.Close();
              return null;
          }
        } catch { return ""; }
      }
    }

    public static void DeleteFile(string path) {
      try {
        File.Delete(path);
      } catch { }
    }
  }

  public enum WorkFileMode {
    WriteAppend = 0,
    WriteNew = 1,
    ReadAllText = 2,
    ReadArrayString = 3
  }
}
