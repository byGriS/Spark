using System;
using System.Drawing;
using System.IO.Ports;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace EmulPipes {
  public partial class MainWindow : Window {
    private SerialPort sp = null;

    private double lenght = 0;
    private double Lenght {
      get { return lenght; }
      set {
        lenght = value;
        labelLenght.Content = lenght.ToString();
      }
    }
    private Timer timer = new Timer(1000);
    private Timer time = new Timer(1000);
    private Random random = new Random();

    private double inc = 0;

    public MainWindow() {
      InitializeComponent();
      cbPortName.ItemsSource = SerialPort.GetPortNames();
      timer.Elapsed += Timer_Elapsed;
      time.Elapsed += Time_Elapsed;
      time.Enabled = true;
      timerFile.Elapsed += TimerFile_Elapsed;
    }

    private void Time_Elapsed(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
        labelTime.Content = DateTime.Now.ToString("HH:mm:ss");
      }));
    }

    private void Start_Click(object sender, RoutedEventArgs e) {
      if (cbPortName.SelectedItem == null) {
        MessageBox.Show("Выберите порт", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      Button b = (Button)sender;
      if (b.Content.ToString() == "Подключиться") {
        b.Content = "Отключиться";
        if (sp == null) {
          sp = new SerialPort();
        }
        if (sp.IsOpen)
          sp.Close();
        sp.PortName = cbPortName.SelectedItem.ToString();
        sp.BaudRate = Convert.ToInt32(cbBaudRate.Text);
        sp.DataBits = Convert.ToInt32(cbDataBits.Text);
        sp.Parity = GetParity();
        sp.StopBits = GetStopBits();
        sp.Open();
        sp.DataReceived += Sp_DataReceived;
        timer.Enabled = true;
      } else {
        timer.Enabled = false;
        b.Content = "Подключиться";
        sp.Close();
        sp = null;
      }
    }

    private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e) {
      SerialPort sp = (SerialPort)sender;
      string result = sp.ReadTo("\r");
      if (result.IndexOf("start") > -1) {
        this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
          this.Lenght = 0;
        }));
      }
      if (result.IndexOf("so") > -1) {
        this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
          this.Lenght += 100;
        }));
      }
    }

    private Parity GetParity() {
      switch (cbParity.Text) {
        case "Четн.": return Parity.Even;
        case "Нечетн.": return Parity.Odd;
        case "Mark": return Parity.Mark;
        case "Space": return Parity.Space;
        default: return Parity.None;
      }
    }

    private StopBits GetStopBits() {
      switch (cbStopBits.Text) {
        case "1": return StopBits.One;
        case "1.5": return StopBits.OnePointFive;
        default: return StopBits.Two;
      }
    }

    private void Inc_Click(object sender, RoutedEventArgs e) {
      labelStatus.Content = "увеличивается";
      inc = Convert.ToDouble(tbInc.Text);
    }

    private void Dec_Click(object sender, RoutedEventArgs e) {
      labelStatus.Content = "уменьшается";
      inc = Convert.ToDouble(tbDec.Text) * -1.0;
    }

    private void Stop_Click(object sender, RoutedEventArgs e) {
      labelStatus.Content = "стоит";
      inc = 0;
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
        Lenght += inc;
        sp.WriteLine(DateTime.Now.ToString("H:m:s") + ";" + Lenght.ToString() + ";0.5;" + random.Next(0, 100) / 100.0);
      }));

    }

    private void tb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      TextBox tb = (TextBox)sender;
      if (IsNotDigit(e.Text)) {
        e.Handled = true;
      }
      if (e.Text == ",") {
        string[] split = tb.Text.Split(',');
        if (split.Length > 1)
          e.Handled = true;
        else
          e.Handled = false;
        if (tb.Text.Length == 0)
          e.Handled = true;
      }
    }

    private bool IsNotDigit(string input) {
      return (new System.Text.RegularExpressions.Regex("[^0-9-]+").IsMatch(input));
    }

    private string[] lines;
    private int indexLine = 1;
    private DateTime fileDateTime = DateTime.MinValue;
    private Timer timerFile = new Timer(1000);

    private void SelectFile_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
      ofd.Filter = "CSV file (*.csv)|*.csv";
      ofd.Title = "Открыть";
      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        lSelectedFile.Content = ofd.FileName;
        lines = (string[])Service.WorkFile.Do(ofd.FileName, Service.WorkFileMode.ReadArrayString);
      }
    }

    private void StartFile_Click(object sender, RoutedEventArgs e) {
      if (bStart.Content.ToString() == "Start") {
        bStart.Content = "Stop";
        timerFile.Start();
      } else {
        bStart.Content = "Start";
        timerFile.Stop();
      }
    }

    private void TimerFile_Elapsed(object sender, ElapsedEventArgs e) {

      string[] data = lines[indexLine].Split(';');
      if (fileDateTime == DateTime.MinValue) {
        fileDateTime = DateTime.Parse(data[0].Replace('"', ' '));
      } 
      if (fileDateTime == DateTime.Parse(data[0].Replace('"', ' '))) {
        sp.WriteLine(DateTime.Now.ToString("H:m:s") + ";" + data[1].ToString() + ";" + data[2]);
        indexLine++;
      }
      fileDateTime = fileDateTime.AddSeconds(1);
    }
  }
}