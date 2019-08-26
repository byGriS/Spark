using Core;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Spark {
  public partial class ArchiveWindow : Window {

    private SQLiteConnection dbConn = new SQLiteConnection();
    private SQLiteCommand sqlCmd = new SQLiteCommand();
    private ObservableCollection<CheckedParam> selectedParam = new ObservableCollection<CheckedParam>();
    private ObservableCollection<WorkSpark> works = new ObservableCollection<WorkSpark>();

    private GraphSetting graphSetting = null;
    private PlotModel model;
    private Timer timerEmul = new Timer(1000);

    private SparkWindow spark;

    public bool IsEmul { get; set; }

    public ArchiveWindow() {
      InitializeComponent();
      DataContext = this;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      spark = (SparkWindow)this.Owner;
      if (graphSetting == null)
        graphSetting = spark.settingCommon.graphSetting;
      DirectoryInfo folder = new DirectoryInfo(Core.Work.EnvPath);
      FileInfo[] files = folder.GetFiles("*.sqlite");
      string[] filesName = new string[files.Length];
      for (int i = 0; i < files.Length; i++)
        filesName[i] = Path.GetFileNameWithoutExtension(files[i].FullName);
      lbDataBases.ItemsSource = filesName;
      timerEmul.Elapsed += TimerEmul_Elapsed;

      var controller = new PlotController();
      controller.UnbindAll();
      controller.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
      controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control, PlotCommands.ZoomRectangle);
      controller.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Alt, PlotCommands.PointsOnlyTrack);
      controller.BindMouseWheel(PlotCommands.ZoomWheel);
      controller.BindKeyDown(OxyKey.R, PlotCommands.Reset);
      plotter.Controller = controller;
    }   

    private void lbDataBases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
      if (lbDataBases.SelectedItem != null)
        btnConnect.IsEnabled = true;
      else
        btnConnect.IsEnabled = false;
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      plotter.Model = null;
      if (lbDataBases.SelectedItem == null) {
        new Service.MessageView("Выберите базу данных", "", Service.MessageViewMode.Message).Show();
        return;
      }
      string dbFileName = Core.Work.EnvPath + lbDataBases.SelectedItem.ToString() + ".sqlite";
      try {
        if (dbConn.State != ConnectionState.Closed)
          dbConn.Close();
        dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
        dbConn.Open();
        sqlCmd.Connection = dbConn;
      } catch (Exception ex) {
        Service.Log.LogWrite(Core.Work.EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
      try {
        sqlCmd.CommandText = "Select name From sqlite_master where type='table' order by name;";
        SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(dataReader);

        ObservableCollection<CheckedParam> list = new ObservableCollection<CheckedParam>();
        foreach (DataRow row in dt.Rows) {
          if (row.ItemArray[0].ToString() == "sqlite_sequence" || row.ItemArray[0].ToString() == "works")
            continue;
          list.Add(new CheckedParam {
            Title = row.ItemArray[0].ToString().Split('|')[0],
            Unit = row.ItemArray[0].ToString().Split('|')[1],
            IsChecked = false
          });
        }
        lbParams.ItemsSource = list;
        btnRead.IsEnabled = true;
        selectedParam = new ObservableCollection<CheckedParam>();
        FillingParamsData();
      } catch (Exception ex) {
        Service.Log.LogWrite(Core.Work.EnvPath, "Ошибка подключения к базе данных", ex.ToString());
      }
    }

    private Service.MessageView readingWindow;

    private void Read_Click(object sender, RoutedEventArgs e) {
      plotter.Model = null;
      System.Threading.Thread window = null;
      readingWindow = new MessageView("Считывание данных", "", MessageViewMode.Message, false);
      window = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWindowReading));
      window.Start();

      selectedParam = new ObservableCollection<CheckedParam>();
      works = new ObservableCollection<WorkSpark>();
      int indexColor = 0;
      foreach (CheckedParam param in lbParams.Items) {
        param.Points.Clear();
        if (param.IsChecked) {
          param.ColorLine = SparkControls.Helper.ListColor[indexColor++];
          selectedParam.Add(param);
        }
      }

      foreach (CheckedParam param in selectedParam) {
        sqlCmd.CommandText = "update '" + param.Title + "|" + param.Unit + "' set value = replace(value, ',','.')";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "select * from '" + param.Title + "|" + param.Unit + "';";
        SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
        DataTable dt = new DataTable();
        dt.Load(dataReader);
        bool oldData = false;
        foreach (DataRow dr in dt.Rows) {
          try {
            if (oldData) {
              param.Points.Add(new PointDate { X = Convert.ToDateTime(dr[2]), Y = Convert.ToDouble(dr[1]) });
            } else {
              PointDate pd = new PointDate { X = Convert.ToDateTime(dr[2]), Y = Convert.ToDouble(dr[1]), NumWork = Convert.ToInt32(dr[3]) };
              param.Points.Add(pd);
              if (works.Count == 0)
                works.Add(new WorkSpark() { id = pd.NumWork.ToString() , startwork = pd.X.ToString("yyyy-MM-dd HH:mm:ss") });
              else {
                bool add = true;
                foreach (WorkSpark ws in works)
                  if (ws.id == pd.NumWork.ToString()) {
                    add = false;
                    break;
                  }
                if (add) {
                  works.Add(new WorkSpark() { id = pd.NumWork.ToString(), startwork = pd.X.ToString("yyyy-MM-dd HH:mm:ss") });
                }
              }                  
            }
          } catch {
            param.Points.Add(new PointDate { X = Convert.ToDateTime(dr[2]), Y = Convert.ToDouble(dr[1]) });
            oldData = true;
          }
        }
      }
      FillingParamsData();
      System.Threading.Thread.Sleep(100);
      readingWindow.Invoke(new Action(() => readingWindow.Close()));
    }

    private void ShowWindowReading() {
      try {
        readingWindow.ShowDialog();
      } catch{ }
    }

    private void FillingParamsData() {
      if (selectedParam.Count == 0) {
        lCountParams.Content = "0";
        lStart.Content = "";
        lFinish.Content = "";
        btnDrawGraph.IsEnabled = false;
        btnExportData.IsEnabled = false;
        dtpStart.IsEnabled = false;
        dtpFinish.IsEnabled = false;
        dtpStart.Value = null;
        dtpFinish.Value = null;
      } else {
        lCountParams.Content = selectedParam.Count;
        btnDrawGraph.IsEnabled = true;
        btnExportData.IsEnabled = true;
        dtpStart.IsEnabled = true;
        dtpFinish.IsEnabled = true;
        DateTime start = DateTime.MaxValue;
        DateTime finish = DateTime.MinValue;
        foreach (CheckedParam param in selectedParam) {
          if (start > param.Points[0].X)
            start = param.Points[0].X;
          if (finish < param.Points[param.Points.Count - 1].X)
            finish = param.Points[param.Points.Count - 1].X;
        }
        lStart.Content = start.ToString("dd.MM.yyyy HH:mm:ss");
        lFinish.Content = finish.ToString("dd.MM.yyyy HH:mm:ss");
        dtpStart.Value = start;
        dtpFinish.Value = finish;
      }
    }

    private void btnDrawGraph_Click(object sender, RoutedEventArgs e) {
      timerEmul.Stop();
      if (IsEmul) {
        switch (cbSpeed.SelectedIndex) {
          case 0:
            timerEmul.Interval = 1000 / 1.0;
            break;
          case 1:
            timerEmul.Interval = 1000 / 2.0;
            break;
          case 2:
            timerEmul.Interval = 1000 / 4.0;
            break;
          case 3:
            timerEmul.Interval = 1000 / 8.0;
            break;
          case 4:
            timerEmul.Interval = 1000 / 25.0;
            break;
        }
      }
      model = new PlotModel();
      DateTimeAxis axisX = new DateTimeAxis() {
        Title = "Время",
        Position = AxisPosition.Bottom,
        StringFormat = "HH:mm:ss\ndd/MM/yy",
      };
      DateTime start = dtpStart.Value.Value;
      DateTime finish = dtpFinish.Value.Value;
      if (!IsEmul) {
        axisX.Minimum = start.ToOADate();
        axisX.Maximum = finish.ToOADate();
      }
      axisX.MajorGridlineStyle = LineStyle.Solid;
      axisX.MajorGridlineThickness = 1;
      model.Axes.Add(axisX);

      for (int i = 0; i < selectedParam.Count; i++) {
        LinearAxis axeY = new LinearAxis() {
          Title = selectedParam[i].Title + ", " + selectedParam[i].Unit,
          Key = selectedParam[i].Title,
          Position = AxisPosition.Left,
          PositionTier = i,
        };
        
        if (i == 0) {
          axeY.AxisChanged += AxeY_AxisChanged;
          axeY.MajorGridlineStyle = LineStyle.Solid;
          axeY.MajorGridlineThickness = 1;
        }
        
        model.Axes.Add(axeY);
        LineSeries series = new LineSeries() {
          Title = selectedParam[i].Title,
          YAxisKey = selectedParam[i].Title
        };
        model.Series.Add(series);
        int sample = Convert.ToInt32(cbSample.SelectionBoxItem);
        if (!IsEmul) {
          for (int point = 0; point < selectedParam[i].Points.Count; point += sample) {
            if ((selectedParam[i].Points[point].X > start) && (selectedParam[i].Points[point].X < finish)) {
              series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(selectedParam[i].Points[point].X), selectedParam[i].Points[point].Y));
            }
          }
        }
      }     
      plotter.Model = model;
      for (int i = 1; i < model.Axes.Count; i++) {
        if (model.Axes[i].ActualMaximum > Math.Abs(model.Axes[i].ActualMinimum)) {
          model.Axes[i].Minimum = -1 * model.Axes[i].ActualMaximum;
          selectedParam[i - 1].MinY = model.Axes[i].Minimum;
          selectedParam[i - 1].MaxY = model.Axes[i].ActualMaximum;
        } else {
          model.Axes[i].Maximum = -1 * model.Axes[i].ActualMinimum;
          selectedParam[i - 1].MinY = model.Axes[i].ActualMinimum;
          selectedParam[i - 1].MaxY = model.Axes[i].Maximum;
        } 
      }

      if (IsEmul) {
        timerEmul.Start();
      }
      ContextMenu cm = new ContextMenu();
      MenuItem reset = new MenuItem() { Header = "Масштаб по умолчанию" };
      reset.Click += Reset_Click;
      cm.Items.Add(reset);
      MenuItem setting = new MenuItem() { Header = "Настройки" };
      setting.Click += Setting_Click;
      cm.Items.Add(setting);
      MenuItem saveAsPic = new MenuItem() { Header = "Сохранить как картинку" };
      saveAsPic.Click += SaveAsPic_Click;
      cm.Items.Add(saveAsPic);
      MenuItem exportManyPic = new MenuItem() { Header = "Выгрузить несколько картинок" };
      exportManyPic.Click += ExportManyPic_Click;
      cm.Items.Add(exportManyPic);
      plotter.ContextMenu = cm;
      InitGraphSetting();
    }

    private void Reset_Click(object sender, RoutedEventArgs e) {
      if (plotter.ActualModel == null)
        return;
      plotter.ResetAllAxes();
    }

    private void Setting_Click(object sender, RoutedEventArgs e) {
      for (int i = 1; i < model.Axes.Count; i++) {
        selectedParam[i - 1].MinY = model.Axes[i].ActualMinimum;
        selectedParam[i - 1].MaxY = model.Axes[i].ActualMaximum;
      }
      SettingGraphsWindow sgw = new SettingGraphsWindow(graphSetting, selectedParam);
      sgw.ShowDialog();
      InitGraphSetting();
    }

    private void SaveAsPic_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
      sfd.Filter = "PNG file (*.png)|*.png";
      sfd.Title = "Сохранить";
      if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        string well = "";
        DateTime min = DateTimeAxis.ToDateTime(plotter.Model.Axes[0].ActualMinimum);
        foreach (PointDate pd in selectedParam[0].Points) {
          if (pd.X > min) {
            sqlCmd.CommandText = "select * from 'works' where id = "+pd.NumWork.ToString();
            SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dataReader);
            well = dt.Rows[0][1].ToString() + " " + dt.Rows[0][2].ToString()+ " " + dt.Rows[0][3].ToString();
            break;
          }
        }
        plotter.Model.Title = lbDataBases.SelectedItem.ToString() + "\n" + well;
        plotter.SaveBitmap(sfd.FileName, 1920, 1080, plotter.ActualModel.Background);
        plotter.Model.Title = "";
      }
    }

    private void ExportManyPic_Click(object sender, RoutedEventArgs e) {
      ArchiveExportData archiveData = new ArchiveExportData();
      archiveData.Start =  DateTimeAxis.ToDateTime(model.Axes[0].ActualMinimum);
      archiveData.Finish = DateTimeAxis.ToDateTime(model.Axes[0].ActualMaximum);
      ArchiveExporterWindow aew = new ArchiveExporterWindow(archiveData);
      aew.ShowDialog();
      if ((archiveData.PathFolder == null)|| (archiveData.PathFolder == "")) {
        return;
      }else {
        ExportGraphs(archiveData);
      }
    }

    private void ExportGraphs(ArchiveExportData archiveData) {
      DateTime finish = DateTimeAxis.ToDateTime(model.Axes[0].ActualMaximum);
      model.Axes[0].Minimum = archiveData.Start.ToOADate();
      DateTime period = archiveData.Start;
      plotter.Model.Title = lbDataBases.SelectedItem.ToString();
      for (int i = 0; true; i++) {
        if (i > 0)
          model.Axes[0].Minimum = period.ToOADate();
        period = period.AddSeconds(archiveData.Interval);
        model.Axes[0].Maximum = period.ToOADate();
        Directory.CreateDirectory(archiveData.PathFolder + Path.DirectorySeparatorChar.ToString() + "archive");
       
        plotter.SaveBitmap(
          archiveData.PathFolder + 
          Path.DirectorySeparatorChar.ToString() + 
          "archive"+
          Path.DirectorySeparatorChar.ToString() +
          (i.ToString() + ".png"), 1920, 1080, plotter.ActualModel.Background);
        if (period > finish)
          break;
      }
      plotter.Model.Title = "";
    }

    private void AxeY_AxisChanged(object sender, AxisChangedEventArgs e) {
      Axis ax = sender as Axis;
      if (e.ChangeType == AxisChangeTypes.Pan) {
        double delta = ax.ActualMaximum - ax.ActualMinimum;
        foreach (Axis axis in plotter.ActualModel.Axes) {
          if ((axis.GetType() != typeof(DateTimeAxis)) && (axis != ax)) {
            axis.Pan(e.DeltaMaximum * (axis.ActualMaximum - axis.ActualMinimum) / delta * axis.Scale * -1);
          }
        }
      }
      if (e.ChangeType == AxisChangeTypes.Zoom) {
        double delta = (ax.ActualMaximum - e.DeltaMaximum) - (ax.ActualMinimum - e.DeltaMinimum);
        foreach (Axis axis in plotter.ActualModel.Axes) {
          if ((axis.GetType() != typeof(DateTimeAxis)) && (axis != ax)) {
            double z = e.DeltaMaximum * (axis.ActualMaximum - axis.ActualMinimum) / delta;
            double x = e.DeltaMinimum * (axis.ActualMaximum - axis.ActualMinimum) / delta;
            axis.Zoom(axis.ActualMinimum + x, axis.ActualMaximum + z);
          }
        }
      }
    }

    private void InitGraphSetting() {
      model.IsLegendVisible = graphSetting.LegendVis;
      model.LegendPosition = graphSetting.LegendPos;
      model.LegendFontSize = graphSetting.LegendSizeFont;
      int indexDataParam = 0;
      foreach (LineSeries series in model.Series) {
        series.StrokeThickness = graphSetting.LineWidth;
        series.Color = OxyColor.FromRgb(selectedParam[indexDataParam].ColorLine.Color.R, selectedParam[indexDataParam].ColorLine.Color.G, selectedParam[indexDataParam].ColorLine.Color.B);
        indexDataParam++;
      }
      indexDataParam = 0;
      for (int i = 0; i < model.Axes.Count; i++) {
        model.Axes[i].TitleFontSize = graphSetting.AxisSizeFont;
        model.Axes[i].FontSize = graphSetting.AxisSizeFont;
        if (!model.Axes[i].IsHorizontal())
          if (indexDataParam < selectedParam.Count) {
            model.Axes[i].Zoom(selectedParam[indexDataParam].MinY, selectedParam[indexDataParam].MaxY);
            if (selectedParam[indexDataParam++].IsRight)
              model.Axes[i].Position = AxisPosition.Right;
            else
              model.Axes[i].Position = AxisPosition.Left;
          }

      }
      plotter.InvalidatePlot(false);
    }

    private int index = 0;
    private void TimerEmul_Elapsed(object sender, ElapsedEventArgs e) {
      this.Dispatcher.Invoke(new System.Threading.ThreadStart(delegate {
        int sample = Convert.ToInt32(cbSample.SelectionBoxItem);
        for (int i = 0; i < selectedParam.Count; i++) {
          ((LineSeries)plotter.Model.Series[i]).Points.Add(new DataPoint(DateTimeAxis.ToDouble(selectedParam[i].Points[index].X), selectedParam[i].Points[index].Y));
        }
        plotter.InvalidatePlot(true);
        index+= sample;
        if (index >= selectedParam[0].Points.Count) {
          timerEmul.Stop();
          index = 0;
        }
      }));
    }

    private void btnExportData_Click(object sender, RoutedEventArgs e) {
      ArchiveExportToServerWindow aetsw = new ArchiveExportToServerWindow(lbDataBases.SelectedItem.ToString(), sqlCmd, works, selectedParam);
      if (aetsw.IsEnabled)
        aetsw.ShowDialog();
      else
        aetsw.Close();
    }
  }

  public class CheckedParam : INotifyPropertyChanged {

    private string title = "";
    public string Title {
      get { return title; }
      set {
        title = value;
        OnPropertyChanged("Title");
      }
    }

    private string unit = "";
    public string Unit {
      get { return unit; }
      set {
        unit = value;
        OnPropertyChanged("Unit");
      }
    }

    private bool isChecked = false;
    public bool IsChecked {
      get { return isChecked; }
      set {
        isChecked = value;
        OnPropertyChanged("IsChecked");
      }
    }

    public SolidColorBrush ColorLine { get; set; }
    public bool IsRight { get; set; }
    public double MinY { get; set; }
    public double MaxY { get; set; }

    public ObservableCollection<PointDate> Points = new ObservableCollection<PointDate>();

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

  }

  public struct PointDate {
    public DateTime X;
    public double Y;
    public int NumWork { get; set; }
  }



}
