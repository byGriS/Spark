using System;
using System.Windows.Forms;

namespace Service {
  public partial class MessageToggle : Form {
    public MessageToggle() {
      InitializeComponent();
    }

    private void button2_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void button1_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
