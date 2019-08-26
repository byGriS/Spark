using System;
using System.Windows.Forms;

namespace Service {
  public partial class MessageView : Form {
    public MessageView(string message, string caption, MessageViewMode mode, bool autoClose = true) {
      InitializeComponent();
      this.Text = caption;
      label1.Text = message;
      if (mode == MessageViewMode.Message){
        this.Height = 75;
        button1.Visible = false;
        System.Timers.Timer timer = new System.Timers.Timer(2000);
        timer.AutoReset = false;
        timer.Elapsed += Timer_Elapsed;
        timer.Enabled = autoClose;
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
      if (this.Created != false)
        this.BeginInvoke(new Action(() => {        
        this.Close();
      }));
    }

    private void button1_Click(object sender, System.EventArgs e) {
      this.Close();
    }
  }

  public enum MessageViewMode {
    Error = 0,
    Message = 1,
    Attention = 2
  }

}
