using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Spark {
  public partial class PipeCounterSetWindow : Window {

    private string pathFolder;

    public PipeCounterSetWindow(string pathFolder) {
      InitializeComponent();
      this.pathFolder = pathFolder;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      string[] data = Service.WorkFile.Do(pathFolder + "pipeCounter.txt", Service.WorkFileMode.ReadAllText).ToString().Split('#');
      if (data.Length == 4) {
        tbDempf.Text = data[0];
        tbLengthFrom.Text = data[1];
        tbLengthTo.Text = data[2];
        tbTime.Text = data[3];
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e) {
      string result = "";
      result += tbDempf.Text + "#";
      result += tbLengthFrom.Text + "#";
      result += tbLengthTo.Text + "#";
      result += tbTime.Text;
      Service.WorkFile.Do(pathFolder + "pipeCounter.txt", Service.WorkFileMode.WriteNew, result);
      this.Close();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
      e.Handled = !IsTextAllowed(e.Text);
    }

    private static readonly Regex _regex = new Regex("[^0-9]+");
    private static bool IsTextAllowed(string text) {
      return !_regex.IsMatch(text);
    }

  }
}
