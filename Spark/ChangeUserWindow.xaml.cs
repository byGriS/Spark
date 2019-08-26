using System.Text;
using System.Windows;

namespace Spark {
  public partial class ChangeUserWindow : Window {
    private SparkWindow sparkWindow = null;

    public ChangeUserWindow(SparkWindow sparkWindow) {
      InitializeComponent();
      this.sparkWindow = sparkWindow;
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void admin_Click(object sender, RoutedEventArgs e) {
      StringBuilder pass = new StringBuilder();
      if (new Service.MessageInput("Введите пароль администратора","Пароль", pass, false).ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        if (pass.ToString() == sparkWindow.settingCommon.PassAdmin) {
          sparkWindow.IsAdmin = true;
          this.Close();
        }else {
          new Service.MessageView("Неверный пароль", "", Service.MessageViewMode.Attention).ShowDialog();
        }
      }
    }

    private void operator_Click(object sender, RoutedEventArgs e) {
      sparkWindow.IsAdmin = false;
      this.Close();
    }
  }
}
