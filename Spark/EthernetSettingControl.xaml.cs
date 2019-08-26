using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Spark {
  public partial class EthernetSettingControl : UserControl {

    private string tempIPAddress = "";

    public EthernetSettingControl() {
      InitializeComponent();
    }

    private Service.InputEthernet input = null;
    public Service.InputEthernet Input {
      get { return input; }
      set {
        input = value;
        DataContext = value;
        tbSerialName.Text = input.Title;
        tempIPAddress = input.IPSlave;
      }
    }

    private bool isSlave = false;
    public bool IsSlave {
      get { return isSlave; }
      set {
        isSlave = value;
        if (isSlave) {
          gridIPAddress.Visibility = System.Windows.Visibility.Collapsed;
          gridIDSlave.Visibility = System.Windows.Visibility.Visible;
        } else {
          gridIPAddress.Visibility = System.Windows.Visibility.Visible;
          gridIDSlave.Visibility = System.Windows.Visibility.Collapsed;
        }
      }
    }

    private void OnlyNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
      Regex regex = new Regex("[^0-9]+");
      e.Handled = regex.IsMatch(e.Text);
    }

    private void tbSerialName_TextChanged(object sender, TextChangedEventArgs e) {
      if (input == null)
        return;
      input.Title = tbSerialName.Text;
    }

    private void tbIP_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
      tbIP.Text = tbIP.Text.Replace(',', '.');
      IPAddress address;
      if (!IPAddress.TryParse(tbIP.Text,out address)){
        Service.Log.LogShow(Core.Work.EnvPath, "Неверный формат IP адреса: " + tbIP.Text, "input IP " + tbIP.Text, "Ошибка ввода", Service.MessageViewMode.Attention);
        tbIP.Text = tempIPAddress;
      } else {
        tempIPAddress = tbIP.Text;
      }
    }
  }
}
