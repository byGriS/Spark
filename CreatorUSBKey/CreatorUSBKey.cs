using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace CreatorUSBKey {
  public partial class CreatorUSBKey : Form {
    private static USBData KeyUSB;
    private string vervionKey = "v1.0.1";

    public CreatorUSBKey() {
      InitializeComponent();
      RefreshListDisk();
    }

    private void button1_Click(object sender, EventArgs e) {
      int countActivate = 0;
      if (!int.TryParse(textBox2.Text, out countActivate)) {
        MessageBox.Show("Введены некорректные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      if (MessageBox.Show("USB диск будет отформатирован", "Вопрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {

        //GetKeyUSB();
        if (!CheckVolume(comboBox1.Text)) {
          MessageBox.Show("Данное USB устройство не подходит для создания флешки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        Format(comboBox1.Text);
        Directory.CreateDirectory(comboBox1.Text + @"\ИСКРА.LIC");
        GetKeyUSB();
        /* if (KeyUSB == null)
            MessageBox.Show("key null");
         if (KeyUSB.serialNumber == null)
            MessageBox.Show("serial null");
         if (KeyUSB.serialVolume == null)
            MessageBox.Show("volumn null");*/
        Random r = new Random();
        string data = Encoding(r.Next(1000, 100000).ToString(), KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(vervionKey, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(textBox1.Text, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(KeyUSB.serialNumber, KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(countActivate.ToString(), KeyUSB.serialVolume) + "#?!\r\n";
        data += Encoding(DateTime.Now.ToString("dd/MM/yy HH:mm"), KeyUSB.serialVolume);
        File.WriteAllText(KeyUSB.pathFile, data);
        ProtectFile();
        MessageBox.Show("Ключ создан");
      }

    }

    private static void ProtectFile() {
      try {
        DirectoryInfo directoryInfo = new DirectoryInfo(KeyUSB.pathDir);
        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
        accessControl.SetAccessRuleProtection(true, false);
        directoryInfo.SetAccessControl(accessControl);
      } catch { }
    }

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

    private static bool GetKeyUSB() {
      List<USBData> usb = GetAllUSB();
      foreach (USBData arg in usb) {
        string path = arg.logicalDisk + @"\ИСКРА.LIC";
        if (Directory.Exists(path)) {
          KeyUSB = arg;
          KeyUSB.pathDir = path;
          KeyUSB.pathFile = path + @"\ИСКРА.LIC";
          break;
        }
      }
      if (KeyUSB == null)
        return false;
      return true;
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
          /*if (arg[2] == null)
             usbData.serialNumber = "";
          else*/
          usbData.serialNumber = arg[2];
          if (usbData.serialNumber == null)
            usbData.serialNumber = "";
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

    private bool CheckVolume(string disk) {
      List<string[]> volume = SelectWMI("Win32_LogicalDisk", new string[] { "Name", "VolumeSerialNumber" });
      foreach (string[] arg in volume) {
        if (arg[0] == disk) {
          if ((arg[1] != null) && (arg[1].Trim() != ""))
            return true;
        }
      }
      return false;
    }

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

    private void button2_Click(object sender, EventArgs e) {
      RefreshListDisk();
    }

    private void RefreshListDisk() {
      comboBox1.Items.Clear();
      comboBox1.Text = "";
      List<string[]> diskDrive = SelectWMI("Win32_DiskDrive", new string[] { "DeviceID", "InterfaceType", "SerialNumber" });
      List<string> partition = new List<string>();
      List<string> logical = new List<string>();
      foreach (string[] arg in diskDrive) {
        if (arg[1] == "USB") {
          List<string> part = AssociatorsWMI("Win32_DiskDrive.DeviceID='" + arg[0] + "'", "Win32_DiskDriveToDiskPartition", "DeviceID");
          foreach (string data in part) {
            partition.Add(data);
          }
        }
      }
      foreach (string arg in partition) {
        List<string> tempLogical = AssociatorsWMI("Win32_DiskPartition.DeviceID='" + arg + "'", "Win32_LogicalDiskToPartition", "Name");
        foreach (string data in tempLogical) {
          logical.Add(data);
        }
      }
      foreach (string arg in logical) {
        comboBox1.Items.Add(arg);
      }
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
}
