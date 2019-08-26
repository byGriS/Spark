using System;
using System.Windows.Forms;

namespace Service {
  public partial class MessageQuestion : Form {
    public MessageQuestion(string message, string caption, MessageViewMode mode) {
      InitializeComponent();
      this.Text = caption;
      label1.Text = message;
      if (mode == MessageViewMode.Message) {
        this.Height = 125;
        button1.Visible = false;
        System.Timers.Timer timer = new System.Timers.Timer(2000);
        timer.AutoReset = false;
        timer.Enabled = true;
        timer.Elapsed += Timer_Elapsed;
      }
      switch (mode) {
        case MessageViewMode.Message:
          pictureBox1.Image = Properties.Resources.Message;
          break;
        case MessageViewMode.Attention:
          pictureBox1.Image = Properties.Resources.Attention;
          break;
        case MessageViewMode.Error:
          pictureBox1.Image = Properties.Resources.Error;
          break;
      }

    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      this.BeginInvoke(new Action(() => {
        this.Close();
      }));
    }

    private void button1_Click(object sender, System.EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }

}
