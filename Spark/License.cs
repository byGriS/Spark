using System;
using System.Collections.Generic;
using System.Management;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Timers;

namespace Spark {
  public class License {
    private static USBData KeyUSB;
    public static USBData KeyReadUSB;
    private static Timer checkLicense;
    public static SystemData systemDataEx = new SystemData();
    private static bool isBusy = false;
    private static SparkWindow sparkWindow;

    public static bool CheckLicense() {
      sparkWindow.IsLicense = true;
      return true;
      bool result = false;
      result = CheckUSBKey();
      if (result) {
        sparkWindow.IsLicense = true;
        return true;
      }
      result = CheckLocalKey();
      if (result) {
        sparkWindow.IsLicense = true;
        return true;
      }
      sparkWindow.IsLicense = false;
      return false;
    }

    public static void StartLoopCheckLicense(SparkWindow sparkWindow) {
      try {
        License.sparkWindow = sparkWindow;
        checkLicense = new Timer(10000);
        checkLicense.Elapsed += CheckLicense_Elapsed;
        checkLicense.Enabled = true;
      } catch { }
    }

    private static void CheckLicense_Elapsed(object sender, ElapsedEventArgs e) {
      try {
        if (isBusy)
          return;
        CheckLicense();
      } catch { }
    }

    public static void EndTimers() {
      if (checkLicense != null) {
        checkLicense.Enabled = false;
        checkLicense.Dispose();
      }
    }

    public static string BindingPC() {
      isBusy = true;
      if (GetKeyUSB()) {
        AllowFile();
        try { ReadKey(); } catch {
          ProtectFile();
          isBusy = false;
          return "Ошибка работы с USB ключом";
        }
        if (KeyReadUSB.countActivate < 1) {
          ProtectFile();
          isBusy = false;
          return "USB ключ не имеет лицензий активации";
        }
        Format(KeyUSB.logicalDisk);
        Directory.CreateDirectory(KeyUSB.logicalDisk + @"\ИСКРА.LIC");
        GetKeyUSB();
        string data = Encoding(KeyReadUSB.numberKey.ToString(), KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(KeyReadUSB.versionKey, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(KeyReadUSB.owner, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(KeyUSB.serialNumber, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding((KeyReadUSB.countActivate - 1).ToString(), KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(DateTime.Now.ToString("dd/MM/yy HH:mm"), KeyUSB.serialVolume);
        File.WriteAllText(KeyUSB.pathFile, data);
        ProtectFile();
        CreateLocalKey();
        return "Программа активирована,\r\nкол-во лицензий на USB ключе: " + (KeyReadUSB.countActivate - 1).ToString();
      }
      isBusy = false;
      return "Отсутствует USB ключ";
    }

    public static bool CheckUSBKey() {
      if (GetKeyUSB()) {
        AllowFile();
        try { ReadKey(); } catch { }
        ProtectFile();
        if (KeyReadUSB.serialNumber != KeyUSB.serialNumber)
          return false;
        if (KeyReadUSB.dateTimeCreate != KeyUSB.dateTimeCreate)
          return false;
        return true;
      } else {
        return false;
      }
    }

    private static void ReadKey() {
      KeyReadUSB = new USBData();
      FileInfo fileInfo = new FileInfo(KeyUSB.pathFile);
      KeyUSB.dateTimeCreate = fileInfo.CreationTime.ToString("dd/MM/yy HH:mm");
      string inputText = File.ReadAllText(KeyUSB.pathFile);
      string[] input = inputText.Split(new string[] { "#?!\r\n" }, StringSplitOptions.None);
      KeyReadUSB.numberKey = Decoding(input[0], KeyUSB.serialVolume);
      KeyReadUSB.versionKey = Decoding(input[1], KeyUSB.serialVolume);
      KeyReadUSB.owner = Decoding(input[2], KeyUSB.serialVolume);
      KeyReadUSB.serialNumber = Decoding(input[3], KeyUSB.serialVolume);
      KeyReadUSB.countActivate = Convert.ToInt32(Decoding(input[4], KeyUSB.serialVolume));
      KeyReadUSB.dateTimeCreate = Decoding(input[5], KeyUSB.serialVolume);
    }

    private static bool GetKeyUSB() {
      List<USBData> usb = GetAllUSB();
      foreach (USBData arg in usb) {
        string path = arg.logicalDisk + "/ИСКРА.LIC";
        if (Directory.Exists(path)) {
          KeyUSB = arg;
          KeyUSB.pathDir = path;
          KeyUSB.pathFile = path + "/ИСКРА.LIC";
          break;
        }
      }
      if (KeyUSB == null)
        return false;

      return true;
    }

    #region protectUSB
    private static void ProtectFile() {
      try {
        DirectoryInfo directoryInfo = new DirectoryInfo(KeyUSB.pathDir);
        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
        accessControl.SetAccessRuleProtection(true, false);
        directoryInfo.SetAccessControl(accessControl);
      } catch { }
    }

    private static void AllowFile() {
      try {
        DirectoryInfo directoryInfo = new DirectoryInfo(KeyUSB.pathDir);
        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
        accessControl.SetAccessRuleProtection(false, true);
        directoryInfo.SetAccessControl(accessControl);
      } catch { }
    }
    #endregion

    #region codingUSB
    private static string Encoding(string input, string key) {
      string output = "";
      int index = 0;
      foreach (char sym in input) {
        if (index == key.Length)
          index = 0;
        output += ((char)(sym + key[index])).ToString();
        output += ((char)(sym - key[index++])).ToString();
      }
      return output;
    }

    private static string Decoding(string input, string key) {
      string output = "";
      int index = 0;
      for (int i = 0; i < input.Length; i += 2) {
        if (index == key.Length)
          index = 0;
        output += ((char)(input[i] - key[index++])).ToString();
      }
      return output;
    }
    #endregion

    public static void Format(string disk) {
      Process process = new Process();
      ProcessStartInfo startInfo = new ProcessStartInfo();
      startInfo.FileName = "cmd.exe";
      startInfo.Arguments = "/C format /fs:ntfs /q " + disk;
      startInfo.RedirectStandardInput = true;
      startInfo.UseShellExecute = false;
      startInfo.CreateNoWindow = true;
      process.StartInfo = startInfo;
      process.Start();
      using (StreamWriter pWriter = process.StandardInput) {
        if (pWriter.BaseStream.CanWrite) {
          pWriter.WriteLine("{ENTER}");
        }
      }
      while (true) {
        try {
          var a = process.ExitTime;
          break;
        } catch { }
        System.Threading.Thread.Sleep(1000);
      }
    }

    private static List<USBData> GetAllUSB() {
      List<USBData> usb = new List<USBData>();
      List<string[]> diskDrive = SelectWMI("Win32_DiskDrive", new string[] { "DeviceID", "InterfaceType", "SerialNumber" });
      List<string> partition = new List<string>();
      List<string> logical = new List<string>();
      List<string[]> volume;
      foreach (string[] arg in diskDrive) {
        if (arg[1] == "USB") {
          USBData usbData = new USBData();
          usbData.serialNumber = arg[2];
          if (usbData.serialNumber == null) {
            usbData.serialNumber = "";
          }
          usb.Add(usbData);
          List<string> part = AssociatorsWMI("Win32_DiskDrive.DeviceID='" + arg[0] + "'", "Win32_DiskDriveToDiskPartition", "DeviceID");
          foreach (string data in part) {
            partition.Add(data);
          }
        }
      }
      if (usb.Count == 0) {
        return usb;
      }
      foreach (string arg in partition) {
        List<string> tempLogical = AssociatorsWMI("Win32_DiskPartition.DeviceID='" + arg + "'", "Win32_LogicalDiskToPartition", "Name");
        foreach (string data in tempLogical) {
          logical.Add(data);
        }
      }
      for (int i = 0; i < logical.Count; i++)
        usb[i].logicalDisk = logical[i];
      volume = SelectWMI("Win32_LogicalDisk", new string[] { "Name", "VolumeSerialNumber" });
      int indexDisk = 0;
      foreach (string[] arg in volume) {
        if (usb.Count == indexDisk)
          break;
        if (arg[0] == usb[indexDisk].logicalDisk) {
          usb[indexDisk++].serialVolume = arg[1];
        }
      }
      return usb;
    }

    #region Создание и проверка файла
    public static bool CheckLocalKey() {
      GetSystemData();
      string pathFile = Core.Work.EnvPath + @"key.lic";
      if (!File.Exists(pathFile)) {
        return false;
      } else {
        string textRead = File.ReadAllText(pathFile);
        string c = "";
        int i = 0;
        int y = 0;
        foreach (char a in textRead) {
          if ((i % 2) == 0) {
            if (y < systemDataEx.lineResult.Length) {
              char g = systemDataEx.lineResult[y];
              if (systemDataEx.lineResult[y] == a) {
                y++;

              }
            }
            c = c + a;
          }
          i++;
        }
        if (y == systemDataEx.lineResult.Length) {
          string[] dataSplit = c.Split(new string[] { "#?!" }, StringSplitOptions.None);
          if (dataSplit.Length == 4) {
            systemDataEx.numberKey = dataSplit[1];
            systemDataEx.versionKey = dataSplit[2];
            systemDataEx.owner = dataSplit[3];
          }
          return true;
        }
        return false;
      }
    }
    public static void CreateLocalKey() {
      GetSystemData();
      string newLineResult = "";
      char b = ' ';
      string c = "";
      Random rn = new Random();
      string write = systemDataEx.lineResult + "#?!" +
         systemDataEx.numberKey + "#?!" +
         systemDataEx.versionKey + "#?!" +
         systemDataEx.owner;
      foreach (char a in write) {
        for (int i = 0; i < 1; i++) {
          b = ((char)rn.Next(0x0041, 0x0250));
          c = c + b;
        }
        newLineResult = newLineResult + a + c;
        c = "";
      }
      File.WriteAllText((Core.Work.EnvPath + @"key.lic"), newLineResult);
    }
    #endregion

    private static void GetSystemData() {
      List<string[]> processor = SelectWMI("Win32_Processor", new string[] { "NumberOfCores", "ProcessorId", "Name", "SocketDesignation" });
      if ((processor.Count > 0) && (processor[0].Length == 4)) {
        systemDataEx.numberOfCores = processor[0][0];
        systemDataEx.processorID = processor[0][1];
        systemDataEx.name = processor[0][2];
        systemDataEx.socketDesignation = processor[0][3];
      }
      List<string[]> bios = SelectWMI("Win32_BIOS", new string[] { "Manufacturer", "Name", "Version" });
      if ((bios.Count > 0) && (bios[0].Length == 3)) {
        systemDataEx.biosManufacturer = bios[0][0];
        systemDataEx.biosName = bios[0][1];
        systemDataEx.biosVersion = bios[0][2];
      }
      List<string[]> board = SelectWMI("Win32_BaseBoard", new string[] { "Product" });
      if ((board.Count > 0) && (board[0].Length == 1)) {
        systemDataEx.baseBoard = board[0][0];
      }
      if (KeyReadUSB != null) {
        systemDataEx.numberKey = KeyReadUSB.numberKey;
        systemDataEx.versionKey = KeyReadUSB.versionKey;
        systemDataEx.owner = KeyReadUSB.owner;
      }
      systemDataEx.lineResult = systemDataEx.numberOfCores +
         systemDataEx.processorID +
         systemDataEx.name +
         systemDataEx.socketDesignation +
         systemDataEx.biosManufacturer +
         systemDataEx.biosName +
         systemDataEx.biosVersion +
         systemDataEx.baseBoard;

    }

    #region работа с WMI
    private static List<string[]> SelectWMI(string table, string[] columns) {
      List<string[]> result = new List<string[]>();
      string query = "select * from " + table;
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
      try {
        foreach (ManagementObject arg in searcher.Get()) {
          string[] data = new string[columns.Length];
          for (int i = 0; i < columns.Length; i++) {
            try { data[i] = arg[columns[i]].ToString().Trim(); } catch { }
          }
          result.Add(data);
        }
      } catch { }
      return result;
    }

    private static List<string> AssociatorsWMI(string associator, string assocClass, string column) {
      List<string> result = new List<string>();
      string query = "associators of {" + associator + "} where AssocClass = " + assocClass;
      ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
      try {
        foreach (ManagementObject arg in searcher.Get()) {
          result.Add(arg[column].ToString().Trim());
        }
      } catch { }
      return result;
    }
    #endregion
  }

  public class USBData {
    public string serialNumber;
    public string logicalDisk;
    public string serialVolume;
    public string pathFile;
    public string pathDir;
    public string dateTimeCreate;
    public string numberKey;
    public string versionKey;
    public string owner;
    public int countActivate;
  }

  public class SystemData {
    public string numberOfCores = "";
    public string processorID = "";
    public string name = "";
    public string socketDesignation = "";
    public string biosManufacturer = "";
    public string biosName = "";
    public string biosVersion = "";
    public string baseBoard = "";
    public string lineResult = "";
    public string numberKey;
    public string versionKey;
    public string owner;
  }
}