using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace Spark {
  public partial class InputSettingWindow : Window {

    public ObservableCollection<Service.Input> inputs = null;
    private Service.Input selectedInput = null;
    private Service.Input SelectedInput {
      get { return selectedInput; }
      set {
        selectedInput = value;
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

    public InputSettingWindow() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      inputs = ((SparkWindow)this.Owner).setting.CloneInputs();
      listInput.ItemsSource = inputs;
      paramsText.InputSettingWindow = this;
      paramsModbus.InputSettingWindow = this;      
    }

    private void AddSerial_Click(object sender, RoutedEventArgs e) {
      inputs.Add(new Service.InputSerial() {
        Title = "Вход " + (inputs.Count + 1).ToString(),
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
      inputs.Add(new Service.InputEthernet() {
        Title = "Вход " + (inputs.Count + 1).ToString(),
        IPSlave = "127.0.0.1",
        Port = 502,
        IsUsed = true,
        IDSlave = 1,
        InputType = Service.InputType.EthernetModbus
      });
    }

    private void RemoveInput_Click(object sender, RoutedEventArgs e) {
      if (SelectedInput == null)
        return;
      inputs.Remove(SelectedInput);
      SelectedInput = null;
    }

    private void listInput_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (listInput.SelectedItem == null)
        return;
      SelectedInput = (Service.Input)listInput.SelectedItem;
      paramsText.SparkWindow = (SparkWindow)this.Owner;
      paramsModbus.SparkWindow = (SparkWindow)this.Owner;
      if (SelectedInput.GetType() == typeof(Service.InputSerial)) {
        serialSetting.Visibility = Visibility.Visible;
        ethernetSetting.Visibility = Visibility.Collapsed;
        if (serialSetting.Input != null) {
          serialSetting.Input.OnChangeType -= Input_OnChangeType;
        }
        serialSetting.Input = (Service.InputSerial)SelectedInput;
        serialSetting.Input.OnChangeType += Input_OnChangeType;
        if (serialSetting.Input.InputType == Service.InputType.COMText) {
          paramsText.Visibility = Visibility.Visible;
          paramsText.ListDataParams = ((Service.InputSerial)SelectedInput).ListDataParams;
          paramsModbus.Visibility = Visibility.Collapsed;
        } else {
          paramsText.Visibility = Visibility.Collapsed;
          paramsModbus.Visibility = Visibility.Visible;
          paramsModbus.ListDataParams = ((Service.InputSerial)SelectedInput).ListDataParams;
        }

      } else {
        serialSetting.Visibility = Visibility.Collapsed;
        ethernetSetting.Visibility = Visibility.Visible;
        ethernetSetting.Input = (Service.InputEthernet)SelectedInput;
        paramsText.Visibility = Visibility.Collapsed;
        paramsModbus.Visibility = Visibility.Visible;
        paramsModbus.ListDataParams = ((Service.InputEthernet)SelectedInput).ListDataParams;

      }
    }

    private void Input_OnChangeType(Service.Input input, Service.InputType inputType) {
      if (inputType == Service.InputType.COMText) {
        paramsText.Visibility = Visibility.Visible;
        paramsText.ListDataParams = ((Service.InputSerial)SelectedInput).ListDataParams;
        paramsModbus.Visibility = Visibility.Collapsed;
      } else {
        paramsText.Visibility = Visibility.Collapsed;
        paramsModbus.Visibility = Visibility.Visible;
        paramsModbus.ListDataParams = ((Service.InputSerial)SelectedInput).ListDataParams;
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      if (!CheckNullTitle()) {
        return;
      }

      ((SparkWindow)this.Owner).setting.Inputs = new ObservableCollection<Service.Input>();
      foreach(Service.Input input in inputs) {
        ((SparkWindow)this.Owner).setting.Inputs.Add(input.Clone());
      }
      ((SparkWindow)this.Owner).setting.UpdateIndicators();
      ((SparkWindow)this.Owner).setting.UpdateWorkListParams();
      ((SparkWindow)this.Owner).setting.UpdateOutputsReferences();
      this.Close();
    }

    private bool CheckNullTitle() {
      bool error = false;
      foreach(Service.Input input in inputs) {
        if (error)
          break;
        foreach(Service.DataParam param in ((Service.InputCommon)input).ListDataParams) {
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
