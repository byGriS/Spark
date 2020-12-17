using System;
using System.Windows;
using System.Windows.Media;

namespace Spark {
  public partial class SettingCommonWindow : Window {

    private Core.SettingCommon settingCommonReturn;
    private Core.SettingCommon settingCommon;

    public SettingCommonWindow(Core.SettingCommon settingCommon) {
      InitializeComponent();
      settingCommonReturn = settingCommon;
      this.settingCommon = settingCommon.Clone();
      DataContext = this.settingCommon;
      bMinColor.Background = new SolidColorBrush(Color.FromRgb(settingCommon.MinColor.R, settingCommon.MinColor.G, settingCommon.MinColor.B));
      bMaxColor.Background = new SolidColorBrush(Color.FromRgb(settingCommon.MaxColor.R, settingCommon.MaxColor.G, settingCommon.MaxColor.B));
    }

    private void NewPass_Click(object sender, RoutedEventArgs e) {
      if (tbOldPass.Text == settingCommon.PassAdmin) {
        settingCommon.PassAdmin= tbNewPass.Text;
        tbNewPass.Text = "";
        tbOldPass.Text = "";
        new Service.MessageView("Пароль изменен", "", Service.MessageViewMode.Message).Show();
      } else {
        new Service.MessageView("Пароль неверный", "", Service.MessageViewMode.Error).Show();
      }
    }

    private void AutoRunFile_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
      ofd.Filter = "Spark file (*.spf)|*.spf";
      ofd.Title = "Открыть";
      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        settingCommon.PathConfig = ofd.FileName;
      }
    }

    private void AutoRunFileCancel_Click(object sender, RoutedEventArgs e) {
      settingCommon.PathConfig = "";
    }

    private void bMinColor_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
      cd.Color = System.Drawing.Color.FromArgb(settingCommon.MinColor.R, settingCommon.MinColor.G, settingCommon.MinColor.B);
      if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        bMinColor.Background = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
        settingCommon.MinColor = Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B);
      }
    }

    private void bMaxColor_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
      cd.Color = System.Drawing.Color.FromArgb(settingCommon.MaxColor.R, settingCommon.MaxColor.G, settingCommon.MaxColor.B);
      if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        bMaxColor.Background = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
        settingCommon.MaxColor = Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B);
      }
    }

    private void BindingPC_Click(object sender, RoutedEventArgs e) {
      Application.Current.Dispatcher.Invoke((Action)(() => {
        Service.MessageView mw = new Service.MessageView("Чтение данных", "", Service.MessageViewMode.Message);
        mw.Show();
        bool check = License.CheckLocalKey();
        mw.Close();
        if (check)
          new Service.MessageView("Данный компьютер уже содержит лицензию", "", Service.MessageViewMode.Message).Show();
        else {
          Service.MessageView bind = new Service.MessageView("Привязка ПК", "", Service.MessageViewMode.Message);
          bind.Show();
          string result = License.BindingPC();
          bind.Close();
          new Service.MessageView(result, "", Service.MessageViewMode.Message).Show();
        }
      }));
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      WriteSetting();
      this.Close();
    }

    private void WriteSetting() {
      settingCommonReturn.IsWindowMode = settingCommon.IsWindowMode;
      settingCommonReturn.IsAutorunWin = settingCommon.IsAutorunWin;
      settingCommonReturn.IsRunModules = settingCommon.IsRunModules;
      settingCommonReturn.PathConfig = settingCommon.PathConfig;
      settingCommonReturn.MinColor = Color.FromRgb(settingCommon.MinColor.R, settingCommon.MinColor.G, settingCommon.MinColor.B);
      settingCommonReturn.MaxColor = Color.FromRgb(settingCommon.MaxColor.R, settingCommon.MaxColor.G, settingCommon.MaxColor.B);
      settingCommonReturn.PassAdmin = settingCommon.PassAdmin;
      settingCommonReturn.Dark = settingCommon.Dark;
      settingCommonReturn.changed = true;
    }
  }
}
