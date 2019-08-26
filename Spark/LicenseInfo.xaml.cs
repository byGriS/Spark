using System.Windows;

namespace Spark {
  public partial class LicenseInfo : Window {
    public LicenseInfo() {
      InitializeComponent();
      Service.MessageView mv =  new Service.MessageView("Чтение данных", "", Service.MessageViewMode.Message);
      mv.Show();
      int result = 0;
      License.CheckUSBKey();
      if ((License.KeyReadUSB != null) && (License.KeyReadUSB.numberKey != null))
        result = 1;
      else {
        if (License.CheckLocalKey())
          result = 2;
      }
      mv.Close();
      if (result == 1) {
        lType.Content = "USB ключ";
        lNum.Content = License.KeyReadUSB.numberKey;
        lVer.Content = License.KeyReadUSB.versionKey;
        lOwner.Content = License.KeyReadUSB.owner;
        lCount.Content = License.KeyReadUSB.countActivate;
      } else {
        if (result == 2) {
          lType.Content = "Локальный файл-ключ";
          lNum.Content = License.systemDataEx.numberKey;
          lVer.Content = License.systemDataEx.versionKey;
          lOwner.Content = License.systemDataEx.owner;
        } else {
          lType.Content = "Ключ отсутствует";
        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }
  }
}
