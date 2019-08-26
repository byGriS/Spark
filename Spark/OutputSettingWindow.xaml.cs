using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;

namespace Spark {
  public partial class OutputSettingWindow : Window {

    public ObservableCollection<Service.Input> outputs = null;
    private Service.Input selectedOutput = null;
    private Service.Input SelectedOutput {
      get { return selectedOutput; }
      set {
        selectedOutput = value;
        if (value == null) {
          bRemoveInput.Visibility = Visibility.Hidden;
          serialSetting.Visibility = Visibility.Hidden;
          paramsText.Visibility = Visibility.Hidden;
          ethernetSetting.Visibility = Visibility.Hidden;
          paramsModbus.Visibility = Visibility.Hidden;
        } else {
          bRemoveInput.Visibility = Visibility.Visible;
        }
      }
    }

    public OutputSettingWindow() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      outputs = ((SparkWindow)this.Owner).setting.CloneOutputs();
      listOutput.ItemsSource = outputs;
      paramsText.SparkWindow = (SparkWindow)this.Owner;
      paramsModbus.SparkWindow = (SparkWindow)this.Owner;
      paramsText.OutputSettingWindow = this;
    }

    private void AddSerial_Click(object sender, RoutedEventArgs e) {
      outputs.Add(new Service.InputSerial() {
        Title = "Выход " + (outputs.Count + 1).ToString(),
        BaudRate = 9600,
        DataBits = 8,
        Dtr = true,
        Handshake = Handshake.None,
        InputType = Service.InputType.COMText,
        IsUsed = true,
        Parity = Parity.None,
        ReadBufferSize = 1024,
        ReadTimeout = 100,
        Rts = true,
        StopBits = StopBits.One,
        IDSlave = 1,
        SymbolSplitter = ";"
      });
    }

    private void AddEthernet_Click(object sender, RoutedEventArgs e) {
      outputs.Add(new Service.InputEthernet() {
        Title = "Выход " + (outputs.Count + 1).ToString(),
        IPSlave = "127.0.0.1",
        Port = 502,
        IsUsed = true,
        IDSlave = 1,
        InputType = Service.InputType.EthernetModbus
      });
    }

    private void RemoveInput_Click(object sender, RoutedEventArgs e) {
      if (SelectedOutput == null)
        return;
      outputs.Remove(SelectedOutput);
      SelectedOutput = null;
    }

    private void listInput_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (listOutput.SelectedItem == null)
        return;
      SelectedOutput = (Service.Input)listOutput.SelectedItem;
      if (SelectedOutput.GetType() == typeof(Service.InputSerial)) {
        serialSetting.Visibility = Visibility.Visible;
        ethernetSetting.Visibility = Visibility.Collapsed;
        if (serialSetting.Input != null) {
          serialSetting.Input.OnChangeType -= Output_OnChangeType;
        }
        serialSetting.Input = (Service.InputSerial)SelectedOutput;
        serialSetting.Input.OnChangeType += Output_OnChangeType;
        if (serialSetting.Input.InputType == Service.InputType.COMText) {
          paramsText.Visibility = Visibility.Visible;
          paramsText.ListDataParams = ((Service.InputSerial)SelectedOutput).ListDataParamsOut;
          paramsModbus.Visibility = Visibility.Collapsed;
        } else {
          serialSetting.IsSlave = true;
          paramsText.Visibility = Visibility.Collapsed;
          paramsModbus.Visibility = Visibility.Visible;
          paramsModbus.ListDataParams = ((Service.InputSerial)SelectedOutput).ListDataParamsOut;
        }

      } else {
        serialSetting.Visibility = Visibility.Collapsed;
        ethernetSetting.Visibility = Visibility.Visible;
        ethernetSetting.IsSlave = true;
        ethernetSetting.Input = (Service.InputEthernet)SelectedOutput;
        paramsText.Visibility = Visibility.Collapsed;
        paramsModbus.Visibility = Visibility.Visible;
        paramsModbus.ListDataParams = ((Service.InputEthernet)SelectedOutput).ListDataParamsOut;

      }
    }

    private void Output_OnChangeType(Service.Input input, Service.InputType inputType) {
      if (inputType == Service.InputType.COMText) {
        serialSetting.IsSlave = false;
        paramsText.Visibility = Visibility.Visible;
        paramsText.ListDataParams = ((Service.InputSerial)SelectedOutput).ListDataParamsOut;
        paramsModbus.Visibility = Visibility.Collapsed;
      } else {
        serialSetting.IsSlave = true;
         paramsText.Visibility = Visibility.Collapsed;
         paramsModbus.Visibility = Visibility.Visible;
         paramsModbus.ListDataParams = ((Service.InputSerial)SelectedOutput).ListDataParamsOut;
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      if (!CheckNullTitle()) {
        return;
      }

      ((SparkWindow)this.Owner).setting.Outputs = new ObservableCollection<Service.Input>();
      foreach (Service.Input input in outputs) {
        ((SparkWindow)this.Owner).setting.Outputs.Add(input.Clone());
      }
      this.Close();
    }

    private bool CheckNullTitle() {
      bool error = false;
      foreach (Service.Input input in outputs) {
        if (error)
          break;
        foreach (Service.DataParam param in ((Service.InputCommon)input).ListDataParams) {
          if ((param.Title == null) || (param.Title == "")) {
            new Service.MessageView("Во входе " + ((Service.InputCommon)input).Title + " не задано имя параметра", "Ошибка", Service.MessageViewMode.Error).ShowDialog();
            error = true;
            break;
          }
        }
      }
      return !error;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }
  }
}
