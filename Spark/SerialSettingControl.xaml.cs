using System.Windows.Controls;
using System.IO.Ports;
using System;
using System.Text.RegularExpressions;

namespace Spark {
  public partial class SerialSettingControl : UserControl {

    public SerialSettingControl() {
      InitializeComponent();
      cbPortName.ItemsSource = SerialPort.GetPortNames();
      int[] baudRate = new int[] { 2400,4800,9600,19200,38400,57600,115200};
      cbBaudRate.ItemsSource = baudRate;
      int[] dataBits = new int[] { 4, 5, 6, 7, 8 };
      cbDataBits.ItemsSource = dataBits;
    }

    private Service.InputSerial input = null;
    public Service.InputSerial Input {
      get { return input; }
      set {
        input = value;
        DataContext = value;
        tbSerialName.Text = input.Title;
        cbParity.Text = Service.SerialPortConvert.ParityToString(input.Parity);
        cbStopBits.Text = Service.SerialPortConvert.StopBitsToString(input.StopBits);
        cbHandshake.Text = Service.SerialPortConvert.HandshakeToString(input.Handshake);
        cbInputType.SelectedIndex = (int)input.InputType;
      }
    }

    private bool isSlave = false;
    public bool IsSlave {
      get { return isSlave; }
      set {
        isSlave = value;
        if (isSlave) {
          gridSplitter.Visibility = System.Windows.Visibility.Collapsed;
          gridIDSlave.Visibility = System.Windows.Visibility.Visible;
        }else {
          gridSplitter.Visibility = System.Windows.Visibility.Visible;
          gridIDSlave.Visibility = System.Windows.Visibility.Collapsed;
        }
      }
    }

    private void tbSerialName_TextChanged(object sender, TextChangedEventArgs e) {
      if (input == null)
        return;
      input.Title = tbSerialName.Text;
    }
    
    private void cbParity_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (input == null)
        return;
      if (e.AddedItems.Count > 0)
        input.Parity = Service.SerialPortConvert.StringToParity((e.AddedItems[0] as ComboBoxItem).Content as string);
    }

    private void cbStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (input == null)
        return;
      if (e.AddedItems.Count > 0)
        input.StopBits = Service.SerialPortConvert.StringToStopBits((e.AddedItems[0] as ComboBoxItem).Content as string);
    }

    private void cbHandshake_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (input == null)
        return;
      if (e.AddedItems.Count > 0)
        input.Handshake = Service.SerialPortConvert.StringToHandshake((e.AddedItems[0] as ComboBoxItem).Content as string);
    }

    private void OnlyNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      Regex regex = new Regex("[^0-9]+");
      e.Handled = regex.IsMatch(e.Text);
    }

    private void cbInputType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (input == null)
        return;
      input.InputType = (Service.InputType)cbInputType.SelectedIndex;
      if (input.InputType == Service.InputType.COMText) {
        lSymbolSplitter.Visibility = System.Windows.Visibility.Visible;
        tbSymbolSplitter.Visibility = System.Windows.Visibility.Visible;
      } else {
        lSymbolSplitter.Visibility = System.Windows.Visibility.Collapsed;
        tbSymbolSplitter.Visibility = System.Windows.Visibility.Collapsed;
      }
    }

    
  }
}
