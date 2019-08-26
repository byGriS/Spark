using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Service {
  public partial class MessageInput : Form {

    private StringBuilder value;

    public MessageInput(string message, string caption, StringBuilder value, bool isOnlyDigit) {
      InitializeComponent();
      this.Text = caption;
      label1.Text = message;
      this.value = value;
      this.ActiveControl = textBox1;
      if (isOnlyDigit)
        textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
    }

    private void button1_Click(object sender, System.EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      value.Append(textBox1.Text);
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
      if (!char.IsControl(e.KeyChar)) {
        int dotIndex = textBox1.Text.IndexOf('.');
        if (char.IsDigit(e.KeyChar)) {
          if (dotIndex != -1 &&
              dotIndex < textBox1.SelectionStart &&
              textBox1.Text.Substring(dotIndex + 1).Length >= 2) {
            e.Handled = true;
          }
        } else
          e.Handled = e.KeyChar != '.' ||
                      dotIndex != -1 ||
                      textBox1.Text.Length == 0 ||
                      textBox1.SelectionStart + 2 < textBox1.Text.Length;
      }
    }

    private void button3_Click(object sender, EventArgs e) {
      string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System);
      Process.Start(path + @"\osk.exe");
    }

    private void MessageInput_FormClosing(object sender, FormClosingEventArgs e) {
      Process[] result = Process.GetProcessesByName("osk");
      if (result.Length > 0) {
        foreach (Process arg in result)
          arg.Kill();
      }
    }
  }

}
