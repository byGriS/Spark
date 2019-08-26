using System;
using System.Windows;

namespace Spark {
  public partial class CommonDataWindow : Window {

    public CommonDataWindow() {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      textBox.Text = ((SparkWindow)this.Owner).setting.Title;
      tbField.Text = ((SparkWindow)this.Owner).setting.Field;
      tbBush.Text = ((SparkWindow)this.Owner).setting.Bush;
      tbWell.Text = ((SparkWindow)this.Owner).setting.Well;
      tbNKTmm.Text = ((SparkWindow)this.Owner).setting.NKTmm.ToString();
      tbLengthPlan.Text = ((SparkWindow)this.Owner).setting.LengthPlan.ToString();
      tbSpeedPlan.Text = ((SparkWindow)this.Owner).setting.SpeedPlan.ToString();
      tbWaterPlan.Text = ((SparkWindow)this.Owner).setting.WaterPlan.ToString();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      ((SparkWindow)this.Owner).setting.Title = textBox.Text;
      ((SparkWindow)this.Owner).setting.Field = tbField.Text;
      ((SparkWindow)this.Owner).setting.Bush = tbBush.Text;
      ((SparkWindow)this.Owner).setting.Well = tbWell.Text;
      try {
        ((SparkWindow)this.Owner).setting.NKTmm = Convert.ToInt16(tbNKTmm.Text);
        ((SparkWindow)this.Owner).setting.LengthPlan = Convert.ToDouble(tbLengthPlan.Text.Replace('.', ','));
        ((SparkWindow)this.Owner).setting.SpeedPlan = Convert.ToDouble(tbSpeedPlan.Text.Replace('.', ','));
        ((SparkWindow)this.Owner).setting.WaterPlan = Convert.ToDouble(tbWaterPlan.Text.Replace('.', ','));
        this.Close();
      } catch {
        new Service.MessageView("Некорректные данные", "Ошибка", Service.MessageViewMode.Error).Show();
      }      
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }
  }
}
