using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text;
using System.Windows.Input;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Net;
using System.Data.SQLite;
using System.Data;

namespace Spark {
  public partial class SparkWindow : Window {

    public Core.Setting setting = new Core.Setting();
    public Core.SettingUnits settingUnits = new Core.SettingUnits();
    public Core.SettingCommon settingCommon = new Core.SettingCommon();
    public Core.Work work = new Core.Work();

    private System.Timers.Timer watchDogTimer = new System.Timers.Timer(5000);
    private System.Timers.Timer watchOnTable = new System.Timers.Timer(1000);

    private Label logo = new Label();

    public SparkWindow() {
      InitializeComponent();
      var uri = new Uri("Light.xaml", UriKind.Relative);
      ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
      Application.Current.Resources.Clear();
      Application.Current.Resources.MergedDictionaries.Add(resourceDict);
      var uriStyles = new Uri("ThemeStyles.xaml", UriKind.Relative);
      ResourceDictionary resourceStyles = Application.LoadComponent(uriStyles) as ResourceDictionary;
      Application.Current.Resources.MergedDictionaries.Add(resourceStyles);
      Core.Work.EnvPath = Environment.CurrentDirectory + "\\";
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      try {
        new Thread(new ThreadStart(CheckLicense)).Start();
        LoadSettingUnits();
        LoadSettingCommon();
        StartWCF();
        DataContext = setting;
        Work_OnChangeState("input", 0);
        Work_OnChangeState("output", 0);
        Work_OnChangedStateWeb("Сервер не работает", 0);
        Core.Work.OnChangedState += Work_OnChangeState;
        Core.Work.OnChangedStateWeb += Work_OnChangedStateWeb;
        Core.Work.OnUpdateInputData += Work_OnUpdateInputData;
        if (Core.Work.CheckWebServer()) {
          Work_OnChangedStateWeb("Сервер готов к работе", 2);
        }
        watchDogTimer.Elapsed += WatchDogTimer_Elapsed;
        watchDogTimer.Start();
        watchOnTable.Elapsed += WatchOnTable_Elapsed;
        watchOnTable.Enabled = true;
        UpdateEvents();
        License.CheckLicense();
        IsAdmin = true;
        if (settingCommon.PathConfig != "") {
          if (File.Exists(settingCommon.PathConfig))
            LoadSetting(settingCommon.PathConfig);
        }
        Thread checkUpdate = new Thread(new ThreadStart(CheckUpdate));
        checkUpdate.Start();
        DataContext = setting;
        AddLabel();
      } catch { }
    }

    private bool isLicense = false;
    public bool IsLicense {
      get { return isLicense; }
      set {
        isLicense = value;
        try {
          this.Dispatcher.Invoke(new ThreadStart(delegate {
            if (!isLicense) {
              InputStart(false);
              OutputStart(false);
              this.Title = "Искра | Демо-режим";
              inputStartBtn.IsEnabled = false;
              outputStartBtn.IsEnabled = false;
            } else {
              this.Title = "Искра";
              inputStartBtn.IsEnabled = true;
              outputStartBtn.IsEnabled = true;
            }
          }));
        } catch { }
      }
    }

    #region Общие методы для приложения

    private void AddLabel() {
      try {
        logo.Content = "ИСКРА" + " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,
           System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);
        logo.FontSize = 36;
        logo.FontWeight = FontWeights.Black;
        logo.Foreground = new SolidColorBrush(Color.FromRgb(97, 97, 97));
        Canvas.SetLeft(logo, windowShow.ActualWidth * 61 / 80 - 80);
        Canvas.SetTop(logo, windowShow.ActualHeight - 48);
        windowShow.Children.Add(logo);
      } catch { }
    }

    private void LoadSettingUnits() {
      try {
        if (File.Exists(Core.Work.EnvPath + "params.dat")) {
          string input = (string)Service.WorkFile.Do(Core.Work.EnvPath + "params.dat", Service.WorkFileMode.ReadAllText);
          settingUnits = Core.Deserialize.Units(input);
          if (settingUnits.ParamsTypes.Count < 3) {
            settingUnits.ParamsTypes.Add(new Service.ParamType {
              Title = "Уровень(A114)"
            });
            settingUnits.ParamsTypes[settingUnits.ParamsTypes.Count-1].ListUnits = new ObservableCollection<Service.ParamUnit>();
            settingUnits.ParamsTypes[settingUnits.ParamsTypes.Count-1].ListUnits.Add(new Service.ParamUnit {
              Title = "м"
            });
          }else {
            if (settingUnits.ParamsTypes[2].Title != "Уровень(A114)") {
              settingUnits.ParamsTypes.Insert(2,new Service.ParamType {
                Title = "Уровень(A114)"
              });
            }
            settingUnits.ParamsTypes[2].ListUnits = new ObservableCollection<Service.ParamUnit>();
            settingUnits.ParamsTypes[2].ListUnits.Add(new Service.ParamUnit {
              Title = "м"
            });
          }
        } else {
          Service.Log.LogShow(Core.Work.EnvPath, "Отсутствует файл со списком ед. изм.\nСоставьте список заново", "not params.dat", "Внимание", Service.MessageViewMode.Attention);
          settingUnits.ParamsTypes.Add(new Service.ParamType {
            Title = "Длина трубы"
          });
          settingUnits.ParamsTypes[0].ListUnits = new ObservableCollection<Service.ParamUnit>();
          settingUnits.ParamsTypes[0].ListUnits.Add(new Service.ParamUnit {
            Title = "м"
          });
          settingUnits.ParamsTypes.Add(new Service.ParamType {
            Title = "Кол-во труб"
          });
          settingUnits.ParamsTypes[1].ListUnits = new ObservableCollection<Service.ParamUnit>();
          settingUnits.ParamsTypes[1].ListUnits.Add(new Service.ParamUnit {
            Title = "шт"
          });
          settingUnits.ParamsTypes.Add(new Service.ParamType {
            Title = "Уровень(A114)"
          });
          settingUnits.ParamsTypes[2].ListUnits = new ObservableCollection<Service.ParamUnit>();
          settingUnits.ParamsTypes[2].ListUnits.Add(new Service.ParamUnit {
            Title = "м"
          });
        }
      } catch { }
    }

    private void LoadSettingCommon() {
      try {
        if (File.Exists(Core.Work.EnvPath + "setting.dat")) {
          string input = (string)Service.WorkFile.Do(Core.Work.EnvPath + "setting.dat", Service.WorkFileMode.ReadAllText);
          settingCommon = Core.Deserialize.SettingCommon(input);
        } else {
          settingCommon.IsWindowMode = true;
        }
        CompliteSettingCommon();
      } catch { }
    }

    private void Work_OnChangeState(string source, int state) {
      try {
        this.Dispatcher.Invoke(new ThreadStart(delegate {
          if (source == "input") {
            inputWork.Children.Clear();
          } else {
            outputWork.Children.Clear();
          }
          Ellipse myEllipse = new Ellipse();
          SolidColorBrush mySolidColorBrush = new SolidColorBrush();
          mySolidColorBrush.Color = Colors.Gray;
          myEllipse.Fill = mySolidColorBrush;
          myEllipse.StrokeThickness = 2;
          myEllipse.Stroke = Brushes.Black;
          myEllipse.Width = 20;
          myEllipse.Height = 20;
          switch (state) {
            case 0:
              mySolidColorBrush.Color = Colors.Gray;
              if (source == "input") {
                inputWork.ToolTip = "Модуль выключен";
              } else {
                outputWork.ToolTip = "Модуль выключен";
              }
              break;
            case 1:
              mySolidColorBrush.Color = Colors.Yellow;
              if (source == "input") {
                inputWork.ToolTip = "Модуль включен, но не работает";
              } else {
                outputWork.ToolTip = "Модуль включен, но не работает";
              }
              break;
            case 2:
              mySolidColorBrush.Color = Colors.LightGreen;
              if (source == "input") {
                inputWork.ToolTip = "Модуль работает";
              } else {
                outputWork.ToolTip = "Модуль работает";
              }
              break;
            case 3:
              mySolidColorBrush.Color = Colors.Red;
              if (source == "input") {
                inputWork.ToolTip = "Ошибка в модуле";
              } else {
                outputWork.ToolTip = "Ошибка в модуле";
              }
              break;
          }
          if (source == "input") {
            inputWork.Children.Add(myEllipse);
          } else {
            outputWork.Children.Add(myEllipse);
          }
        }));
      } catch { }
    }

    private void Work_OnChangedStateWeb(string title, int state) {
      try {
        this.Dispatcher.Invoke(new ThreadStart(delegate {
          webWork.Children.Clear();
          Ellipse myEllipse = new Ellipse();
          SolidColorBrush mySolidColorBrush = new SolidColorBrush();
          mySolidColorBrush.Color = Colors.Gray;
          myEllipse.Fill = mySolidColorBrush;
          myEllipse.StrokeThickness = 2;
          myEllipse.Stroke = Brushes.Black;
          myEllipse.Width = 20;
          myEllipse.Height = 20;
          switch (state) {
            case 0:
              mySolidColorBrush.Color = Colors.Red;
              break;
            case 1:
              mySolidColorBrush.Color = Colors.LightGreen;
              break;
            case 2:
              mySolidColorBrush.Color = Colors.Yellow;
              break;
          }
          webWork.ToolTip = title;
          webWork.Children.Add(myEllipse);
        }));
      } catch { }
    }

    public void CloseApplication() {
      Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
       new Action(() => { this.Close(); })
       );
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      work.CloseApp();
      License.EndTimers();
    }

    private void StartWCF() {
      try {
        ServiceHost serviceHost = new ServiceHost(typeof(Core.WCFService), new Uri("http://localhost:8000/sparkMain"));
        serviceHost.AddServiceEndpoint(typeof(Service.IContract), new BasicHttpBinding(), "");
        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
        behavior.HttpGetEnabled = true;
        serviceHost.Description.Behaviors.Add(behavior);
        serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
        serviceHost.Open();
      } catch (Exception e) {
        Service.Log.LogShow(Core.Work.EnvPath, "Нет прав администратора,\nпрограмма будет работать некорректно", e.ToString(), "Ошибка", Service.MessageViewMode.Error);
      }
    }

    private void UpdateEvents() {
      setting.OnInputIsStart += Setting_OnInputIsStart;
      setting.OnOutputIsStart += Setting_OnOutputIsStart;
    }

    private void Setting_OnInputIsStart(bool value) {
      menuInputSetting.IsEnabled = !value;
      menuCounterPipe.IsEnabled = !value;
      menuCommonData.IsEnabled = !value;
    }

    private void Setting_OnOutputIsStart(bool value) {
      menuOutputSetting.IsEnabled = !value;
    }

    private void CheckLicense() {
      License.StartLoopCheckLicense(this);
    }

    private void WatchDogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      try {
        this.Dispatcher.Invoke(new ThreadStart(delegate {
          if (inputStartBtn.Header.ToString() == "Остановить входы") {
            string[] checkName = new string[] { "SparkInput", "SparkInput.exe" };
            bool restartInput = true;
            for (int i = 0; i < checkName.Length; i++) {
              Process[] result = Process.GetProcessesByName(checkName[i]);
              if (result.Length > 0) {
                restartInput = false;
              }
            }
            if (restartInput)
              InputStart(true, true);
          }
          if (outputStartBtn.Header.ToString() == "Остановить выходы") {
            string[] checkName = new string[] { "SparkOutput", "SparkOutput.exe" };
            bool restartOutput = true;
            for (int i = 0; i < checkName.Length; i++) {
              Process[] result = Process.GetProcessesByName(checkName[i]);
              if (result.Length > 0) {
                restartOutput = false;
              }
            }
            if (restartOutput)
              OutputStart(true);
          }
        }));
      } catch { }
    }

    private void WatchOnTable_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
        this.Dispatcher.Invoke(new ThreadStart(delegate {
          lTime.Content = DateTime.Now.ToString("HH:mm:ss");
        }));
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
      Canvas.SetLeft(logo, windowShow.ActualWidth * 61 / 80 - 80);
      Canvas.SetTop(logo, windowShow.ActualHeight - 48);
    }

    private void UpdateTheme(bool dark) {
      var uri = new Uri(dark?"Dark.xaml":"Light.xaml", UriKind.Relative);
      ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
      Application.Current.Resources.Clear();
      Application.Current.Resources.MergedDictionaries.Add(resourceDict);
      var uriStyles = new Uri("ThemeStyles.xaml", UriKind.Relative);
      ResourceDictionary resourceStyles = Application.LoadComponent(uriStyles) as ResourceDictionary;
      Application.Current.Resources.MergedDictionaries.Add(resourceStyles);
      foreach(SparkControls.WindowIndicators wi in setting.Windows) {
        foreach(SparkControls.Indicator indi in wi.ListIndicators) {
          if (indi.GetType() == typeof(SparkControls.IndiGraph))
            ((SparkControls.IndiGraph)indi).DarkTheme = settingCommon.Dark;
        }
      }
    }

    #endregion

    #region Меню Файл
    private void New_Click(object sender, RoutedEventArgs e) {
      setting = new Core.Setting();
      tabWindows.Items.Clear();
      InputStart(false);
      OutputStart(false);
      Service.WorkFile.Do(Core.Work.EnvPath + "inputState.dat", Service.WorkFileMode.WriteNew, "0");
      Service.WorkFile.Do(Core.Work.EnvPath + "outputState.dat", Service.WorkFileMode.WriteNew, "0");
      UpdateEvents();
      DataContext = setting;
    }

    private void Save_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
      sfd.Filter = "Spark file (*.spf)|*.spf";
      sfd.Title = "Сохранить";
      FileInfo savePath = null;
      if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        savePath = new FileInfo(sfd.FileName);
      }
      if (savePath == null) return;
      string result = Core.Serialize.Setting(setting);
      Service.WorkFile.Do(savePath.FullName, Service.WorkFileMode.WriteNew, result);
    }

    private void Load_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
      ofd.Filter = "Spark file (*.spf)|*.spf";
      ofd.Title = "Открыть";
      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        LoadSetting(ofd.FileName);
      }
    }

    private void LoadSetting(string pathFile) {
      string input = (string)Service.WorkFile.Do(pathFile, Service.WorkFileMode.ReadAllText);
      setting = Core.Deserialize.Setting(input, settingUnits);
      Refresh();
      DataContext = setting;
      UpdateEvents();
      if (settingCommon.IsRunModules) {
        if (setting.CountInput > 0)
          InputStart_Click(null, null);
        if (setting.CountOutput > 0)
          OutputStart_Click(null, null);
      }
    }

    private void Refresh() {
      tabWindows.Items.Clear();
      foreach(SparkControls.WindowIndicators window in setting.Windows) {
        TabItem ti = new TabItem();
        ti.Header = window.Title;
        tabWindows.Items.Add(ti);
        ti.Content = window;
        foreach(SparkControls.Indicator indi in window.ListIndicators) {
          ShowElement(indi, window);
          indi.UpdateBindingValue();
        }
      }
      tabWindows.SelectedIndex = 0;
      SparkControls.SparkControlsService.ListInputParams = setting.GetListInputParams();
      SparkControls.SparkControlsService.ListInputs = setting.Inputs;
      setting.UpdateWorkListParams();
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
      CloseApplication();
    }
    #endregion EndMenu

    #region Меню Данные
    private void CommonSetting_Click(object sender, RoutedEventArgs e) {
      CommonDataWindow cdw = new CommonDataWindow();
      cdw.Owner = this;
      cdw.ShowDialog();
      DataContext = setting;
      //WriteCommonTitles();
    }

    private void InputSetting_Click(object sender, RoutedEventArgs e) {
      InputSettingWindow isw = new InputSettingWindow();
      isw.Owner = this;
      isw.ShowDialog();
      SparkControls.SparkControlsService.ListInputParams = setting.GetListInputParams();
      SparkControls.SparkControlsService.ListInputs = setting.Inputs;

    }

    private void OutputSetting_Click(object sender, RoutedEventArgs e) {
      OutputSettingWindow osw = new OutputSettingWindow();
      osw.Owner = this;
      osw.ShowDialog();
    }

    private void menuCounterPipe_Click(object sender, RoutedEventArgs e) {
      PipeCounterCorrectWindow pccw = new PipeCounterCorrectWindow();
      pccw.Owner = this;
      pccw.ShowDialog();
    }
    #endregion

    #region Меню Окна
    private SparkControls.WindowIndicators selectedWindow = null;
    private SparkControls.WindowIndicators SelectedWindow {
      get { return selectedWindow; }
      set {
        selectedWindow = value;
        if (value != null) {
          windowDelete.IsEnabled = true;
          elementAdd.IsEnabled = true;
        } else {
          windowDelete.IsEnabled = false;
          elementAdd.IsEnabled = false;
        }
      }
    }

    private void WindowAdd_Click(object sender, RoutedEventArgs e) {
      StringBuilder title = new StringBuilder();
      if (new Service.MessageInput("Введите название окна", "Новое окно", title, false).ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        TabItem ti = new TabItem();
        SparkControls.WindowIndicators wi = new SparkControls.WindowIndicators() {
          Title = title.ToString()
        };
        ti.Content = wi;
        ti.Header = wi.Title;
        tabWindows.Items.Add(ti);
        tabWindows.SelectedIndex = tabWindows.Items.Count - 1;
        SelectedWindow = wi;
        setting.Windows.Add(wi);
      }
    }

    private void tabWindows_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if ((TabItem)(sender as TabControl).SelectedItem != null)
        SelectedWindow = (SparkControls.WindowIndicators)((TabItem)(sender as TabControl).SelectedItem).Content;
    }

    private void windowDelete_Click(object sender, RoutedEventArgs e) {
      setting.Windows.Remove(SelectedWindow);
      SelectedWindow = null;
      tabWindows.Items.Remove(tabWindows.SelectedItem);

    }

    private void elementAdd_Click(object sender, RoutedEventArgs e) {
      if (SelectedWindow == null)
        return;
      SparkControls.SelectIndicator selecter = new SparkControls.SelectIndicator();
      if (selecter.ShowDialog() == true) {
        SparkControls.Indicator indi = selecter.Indicator;
        SelectedWindow.ListIndicators.Add(indi);
        ShowElement(indi, SelectedWindow);
      }
    }

    private void ShowElement(SparkControls.Indicator indi, SparkControls.WindowIndicators window) {
      Canvas.SetLeft((UserControl)indi, indi.Location.X);
      Canvas.SetTop((UserControl)indi, indi.Location.Y);
      ((UserControl)indi).Height = indi.Size.Height;
      ((UserControl)indi).Width = indi.Size.Width;
      ((UserControl)indi).MouseDown += Indicator_MouseDown;
      ((UserControl)indi).MouseDoubleClick += Indicator_MouseDoubleClick;
      ((UserControl)indi).MouseMove += Indicator_MouseMove;
      ((UserControl)indi).MouseLeave += Indicator_MouseLeave;
      ContextMenu cm = new ContextMenu();
      MenuItem menuSetting = new MenuItem();
      menuSetting.Header = "Настройки";
      menuSetting.Click += MenuSetting_Click;
      cm.Items.Add(menuSetting);
      MenuItem menuDelete = new MenuItem();
      menuDelete.Header = "Удалить";
      menuDelete.Click += MenuDelete_Click; ;
      cm.Items.Add(menuDelete);
      if (indi.GetType() == typeof(SparkControls.IndiGraph)) {
        MenuItem menuGraphSetting = new MenuItem();
        menuGraphSetting.Header = "Настройки графиков";
        menuGraphSetting.Click += MenuGraphSetting_Click;
        MenuItem menuGraphReset = new MenuItem();
        menuGraphReset.Header = "Масштаб по умолчанию";
        menuGraphReset.Click += MenuGraphReset_Click;
        cm.Items.Add(menuGraphSetting);
        cm.Items.Add(menuGraphReset);
        ((SparkControls.IndiGraph)indi).DarkTheme = settingCommon.Dark;
      }
      if (indi.GetType() == typeof(SparkControls.IndiDigital)) {
        ((SparkControls.IndiDigital)indi).OnSendData -= SparkWindow_OnSendData;
        ((SparkControls.IndiDigital)indi).OnSendData += SparkWindow_OnSendData;
      }
      if (indi.GetType() == typeof(SparkControls.IndiBoolean)) {
        ((SparkControls.IndiBoolean)indi).OnToggleData -= SparkWindow_OnSendData;
        ((SparkControls.IndiBoolean)indi).OnToggleData += SparkWindow_OnSendData;
      }
      if (indi.GetType() == typeof(SparkControls.IndiSendStart)) {
        ((SparkControls.IndiSendStart)indi).OnSendStart -= SparkWindow_OnSendStart;
        ((SparkControls.IndiSendStart)indi).OnSendStart += SparkWindow_OnSendStart;
      }
      if (indi.GetType() == typeof(SparkControls.IndiSendSoOn)) {
        ((SparkControls.IndiSendSoOn)indi).OnSendSoOn -= SparkWindow_OnSendSoOn;
        ((SparkControls.IndiSendSoOn)indi).OnSendSoOn += SparkWindow_OnSendSoOn;
      }
      ((UserControl)indi).ContextMenu = cm;
      window.Children.Add((UserControl)indi);
    }

    private void MenuSetting_Click(object sender, RoutedEventArgs e) {
      SparkControls.Indicator indi = (SparkControls.Indicator)((ContextMenu)((sender as MenuItem).Parent)).PlacementTarget;
      ((Window)indi.IndiSetting).ShowDialog();
    }

    private void MenuGraphReset_Click(object sender, RoutedEventArgs e) {
      SparkControls.IndiGraph indi = (SparkControls.IndiGraph)((ContextMenu)((sender as MenuItem).Parent)).PlacementTarget;
      indi.plotter.ResetAllAxes();
    }

    private void MenuDelete_Click(object sender, RoutedEventArgs e) {
      SparkControls.Indicator indi = (SparkControls.Indicator)((ContextMenu)((sender as MenuItem).Parent)).PlacementTarget;
      SelectedWindow.ListIndicators.Remove(indi);
      SelectedWindow.Children.Remove((UserControl)indi);
    }

    private void MenuGraphSetting_Click(object sender, RoutedEventArgs e) {
      SparkControls.IndiGraph indi = (SparkControls.IndiGraph)((ContextMenu)((sender as MenuItem).Parent)).PlacementTarget;
      SettingGraphsWindow sgw = new SettingGraphsWindow(indi);
      sgw.ShowDialog();
    }

    private void SparkWindow_OnSendData(int idDataParam, float newValue) {
      work.SendDataToInput(idDataParam, newValue);
    }

    private void SparkWindow_OnSendStart(string titleInput) {
      work.SendStartToInput(titleInput);
    }

    private void SparkWindow_OnSendSoOn(string titleInput) {
      work.SendSoOnToInput(titleInput);
    }

    private void Work_OnUpdateInputData(string inputTitle) {
      foreach (SparkControls.Indicator indi in SelectedWindow.ListIndicators) {
        if (indi.GetType() == typeof(SparkControls.IndiInputStatus)) {
          if (((SparkControls.IndiInputStatus)indi).IndiTitle == inputTitle) {
            ((SparkControls.IndiInputStatus)indi).InputDataUpdate();
          }
        }
      }
    }

    private SparkControls.Indicator indiUp = null;
    private SparkControls.Indicator indiDown = null;
    private bool isMove = false;
    private bool isCanSize = false;
    private bool isSize = false;
    private bool isMouseDown = false;
    private Point oldPointMove = new Point();
    private Point oldPointSize = new Point();

    private void Indicator_MouseDown(object sender, MouseButtonEventArgs e) {
      if (e.ClickCount > 1)
        return;
      if (!IsAdmin) return;
      indiDown = sender as SparkControls.Indicator;
      oldPointSize = e.GetPosition((UserControl)indiDown);
      oldPointMove = e.GetPosition((UserControl)indiDown);
      if (!isCanSize)
        isMove = true;
    }

    private void Indicator_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
      isMove = false;
      indiDown = null;
    }

    public void Indicator_MouseMove(object sender, MouseEventArgs e) {
      if (!IsAdmin) return;
      indiUp = sender as SparkControls.Indicator;
    }

    public void Indicator_MouseLeave(object sender, MouseEventArgs e) {
      indiUp = null;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
      if (!IsAdmin) return;
      isMouseDown = true;
      if (indiUp != null) {
        if (isCanSize) {
          isSize = true;
        }
      }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e) {
      try {
        if (indiUp != null) {
          if (!isMouseDown) {
            Point CursorPoint = e.GetPosition((UserControl)indiUp);
            if ((CursorPoint.X > ((UserControl)indiUp).Width - 10) && (CursorPoint.X <= ((UserControl)indiUp).Width) &&
               (CursorPoint.Y > ((UserControl)indiUp).Height - 10) && (CursorPoint.Y <= ((UserControl)indiUp).Height)) {
              ((UserControl)indiUp).Cursor = Cursors.SizeNWSE;
              isCanSize = true;
            } else {
              ((UserControl)indiUp).Cursor = Cursors.Arrow;
              isCanSize = false;
            }
          }
        }
        if (indiDown != null) {
          Point p2 = e.GetPosition(this);
          Point pCon = ((UserControl)indiDown).PointToScreen(new Point(0, 0));
          if ((isSize) && (p2.X > pCon.X + 50) && (p2.Y > pCon.Y + 50)) {
            Point p = e.GetPosition((UserControl)indiDown);
            Point dp = new Point(p.X - oldPointSize.X, p.Y - oldPointSize.Y);
            double dif = ((UserControl)indiDown).Width + dp.X;
            ((UserControl)indiDown).Width = (dif > 50) ? dif : 50;

            if (indiDown.GetType() == typeof(SparkControls.IndiArrow)) {
              dif = ((UserControl)indiDown).Width;
            } else {
              dif = ((UserControl)indiDown).Height + dp.Y;
            }
            ((UserControl)indiDown).Height = (dif > 50) ? dif : 50;
            oldPointSize = e.GetPosition(((UserControl)indiDown));
          } else {
            if (isMove) {
              Point p = e.GetPosition(((UserControl)indiDown));
              Point dp = new Point(p.X - oldPointMove.X, p.Y - oldPointMove.Y);
              Canvas.SetLeft((UserControl)indiDown, Canvas.GetLeft((UserControl)indiDown) + dp.X);
              Canvas.SetTop((UserControl)indiDown, Canvas.GetTop((UserControl)indiDown) + dp.Y);
            }
          }
        }
      } catch { }
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
      isMouseDown = false;
      isSize = false;
      isMove = false;
      if (indiDown != null) {
        indiDown.Size = new Size(((UserControl)indiDown).Width, ((UserControl)indiDown).Height);
        indiDown.Location = new Point(Canvas.GetLeft((UserControl)indiDown), Canvas.GetTop((UserControl)indiDown));
      }
      indiDown = null;
    }
    #endregion

    #region Меню работа (запуск модулей)
    private void InputStart_Click(object sender, RoutedEventArgs e) {
      if (!IsLicense) {
        if (!License.CheckLicense()) {
          new Service.MessageView("Отсутствует лицензия, модуль ввода не запустится", "Лицензия", Service.MessageViewMode.Attention).Show();
          return;
        }
      }
      if (inputStartBtn.Header.ToString() == "Запуск входов") {
        InputStart(true);       
      } else {
        InputStart(false);
      }
    }

    private void InputStart(bool start, bool restart = false) {
      if (start) {
        inputStartBtn.Header = "Остановить входы";
        if (!restart) AddDataWorkToDB();
        work.UpdateInput(setting, restart);
        setting.InputIsStarted = true;
      } else {
        inputStartBtn.Header = "Запуск входов";
        work.StopInput();
        Work_OnChangeState("input", 0);
        setting.InputIsStarted = false;
      }
      if (Core.Work.CheckWebServer()) {
        Work_OnChangedStateWeb("Сервер готов к работе", 2);
      }
    }

    private void AddDataWorkToDB() {
      SQLiteConnection dbConn = new SQLiteConnection();
      SQLiteCommand sqlCmd = new SQLiteCommand();
      dbConn = new SQLiteConnection("Data Source=" + setting.Title+".sqlite" + ";Version=3;");
      dbConn.Open();
      sqlCmd.Connection = dbConn;

      sqlCmd.CommandText = "Select name From sqlite_master where type='table' order by name;";
      SQLiteDataReader dataReader = sqlCmd.ExecuteReader();
      DataTable dt = new DataTable();
      dt.Load(dataReader);
      string table = "";
      foreach(DataRow row in dt.Rows) {
        if (row.ItemArray[0].ToString() == "sqlite_sequence" || row.ItemArray[0].ToString() == "works")
          continue;
        table = row.ItemArray[0].ToString();
        break;        
      }
      if (table != "") {
        try {
          sqlCmd.CommandText = "select numwork from '" + table + "' where id = (select count(*) from works);";
          int NumWork = Convert.ToInt32(sqlCmd.ExecuteScalar());
        } catch {
          dbConn.Close();
          for (int i = 0; true;) {
            try {
              File.Copy(Core.Work.EnvPath + setting.Title + ".sqlite", Core.Work.EnvPath + setting.Title + "_old" + i.ToString() + ".sqlite", false);
              File.Delete(Core.Work.EnvPath + setting.Title + ".sqlite");
              break;
            } catch { i++; }
          }
          dbConn = new SQLiteConnection("Data Source=" + setting.Title + ".sqlite" + ";Version=3;");
          dbConn.Open();
          sqlCmd.Connection = dbConn;
        }
      }

     
      sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS 'works' (id INTEGER PRIMARY KEY AUTOINCREMENT, numwork int, field text, bush text, well text, nktmm text, lengthPlan real, speedPlan real, waterPlan real )";
      sqlCmd.ExecuteNonQuery();

      setting.NumWork = new Random().Next(int.MaxValue);
      sqlCmd.CommandText = "INSERT INTO 'works' ('field', 'bush', 'well', 'nktmm', 'lengthPlan', 'speedPlan', 'waterPlan', 'numwork') values ('" +
              setting.Field + "' , '" +
              setting.Bush + "', '" +
              setting.Well + "', '" +
              setting.NKTmm.ToString() + "', '" +
              setting.LengthPlan.ToString() + "', '" +
              setting.SpeedPlan.ToString() + "', '" +
              setting.WaterPlan.ToString() + "' , '" +
              setting.NumWork.ToString() + "')";
      try {
        sqlCmd.ExecuteNonQuery();
      }catch {
        sqlCmd.CommandText = "alter table 'works' add 'numwork' int";
        sqlCmd.ExecuteNonQuery();
        sqlCmd.CommandText = "INSERT INTO 'works' ('field', 'bush', 'well', 'nktmm', 'lengthPlan', 'speedPlan', 'waterPlan', 'numwork') values ('" +
        setting.Field + "' , '" +
        setting.Bush + "', '" +
        setting.Well + "', '" +
        setting.NKTmm.ToString() + "', '" +
        setting.LengthPlan.ToString() + "', '" +
        setting.SpeedPlan.ToString() + "', '" +
        setting.WaterPlan.ToString() + "', '" +
        setting.NumWork.ToString() + "')";
        sqlCmd.ExecuteNonQuery();
      }

      //sqlCmd.CommandText = "select id from 'works' where id = (select count(*) from works);";
      //setting.NumWork = Convert.ToInt32(sqlCmd.ExecuteScalar());

      dbConn.Close();
    }

    private void OutputStart_Click(object sender, RoutedEventArgs e) {
      if (!IsLicense) {
        if (!License.CheckLicense()) {
          new Service.MessageView("Отсутствует лицензия, модуль вывода не запустится", "Лицензия", Service.MessageViewMode.Attention).Show();
          return;
        }
      }
      if (outputStartBtn.Header.ToString() == "Запуск выходов") {
        OutputStart(true);
      } else {
        OutputStart(false);
      }
    }

    private void OutputStart(bool start) {
      if (start) {
        outputStartBtn.Header = "Остановить выходы";
        work.UpdateOutput(setting);
        work.StartUpdateOutput();
        setting.OutputIsStarted = true;
      } else {
        outputStartBtn.Header = "Запуск выходов";
        work.StopUpdateOutput();
        work.StopOutput();
        Work_OnChangeState("output", 0);
        setting.OutputIsStarted = false;
      }
    }

    private void ReportNKTStartBtn_Click(object sender, RoutedEventArgs e) {
      if (File.Exists(Core.Work.EnvPath + "SReport.exe")) {
        Process input = new Process();
        input.StartInfo.FileName = Core.Work.EnvPath + "SReport.exe";
        input.Start();
      }else {
        new Service.MessageView("Модуль отчета отсутствует", "Ошибка", Service.MessageViewMode.Error, true).Show();
      }
    }
    #endregion

    #region Меню Архив
    private void Archive_Click(object sender, RoutedEventArgs e) {
      try {
        ArchiveWindow aw = new ArchiveWindow();
        aw.Owner = this;
        //aw.ShowDialog();
        aw.Show();
      }catch(Exception ex) {
        MessageBox.Show(ex.Message);
      }
    }
    #endregion

    #region Меню Настройки
    private void GenSetting_Click(object sender, RoutedEventArgs e) {
      SettingCommonWindow scw = new SettingCommonWindow(settingCommon);
      scw.ShowDialog();
      if (settingCommon.changed) {
        string result = Core.Serialize.SettingCommon(settingCommon);
        Service.WorkFile.Do(Core.Work.EnvPath + "setting.dat", Service.WorkFileMode.WriteNew, result);
        settingCommon.changed = false;
        CompliteSettingCommon();
      }
    }

    private void CompliteSettingCommon() {
      SparkControls.Helper.MinColor = settingCommon.MinColor;
      SparkControls.Helper.MaxColor = settingCommon.MaxColor;
      InitIsWindowMode(settingCommon.IsWindowMode);

      string exePath = System.Windows.Forms.Application.ExecutablePath;
      try {
        if (settingCommon.IsAutorunWin) {
          Microsoft.Win32.TaskScheduler.TaskDefinition td = Microsoft.Win32.TaskScheduler.TaskService.Instance.NewTask();
          td.RegistrationInfo.Description = "Start Spark";
          Microsoft.Win32.TaskScheduler.LogonTrigger lt = new Microsoft.Win32.TaskScheduler.LogonTrigger();
          td.Triggers.Add(lt);
          td.Actions.Add(exePath);
          td.Principal.RunLevel = Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest;
          Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.RegisterTaskDefinition("Искра", td);
        } else {
          Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.DeleteTask("Искра");
        }
      } catch { }
      Microsoft.Win32.RegistryKey reg;
      reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
      try {
        if (settingCommon.IsAutorunWin)
          reg.SetValue("Искра", exePath);
        else
          reg.DeleteValue("Искра");
        reg.Close();
      } catch (Exception e) {
        if ((e.Message != "No value exists with that name.") && (e.Message != "Значения для этого имени не существует."))
          Service.Log.LogShow(Core.Work.EnvPath, "Ошибка записи данных в реестр", e.ToString(), "Ошибка", Service.MessageViewMode.Error);
      }
      UpdateTheme(settingCommon.Dark);
    }

    private void InitIsWindowMode(bool windowMode) {
      if (windowMode) {
        this.WindowStyle = WindowStyle.SingleBorderWindow;
        Unhook();
        this.ResizeMode = ResizeMode.CanResize;
        this.WindowState = WindowState.Maximized;
      } else {
        SetHook();
        this.WindowStyle = WindowStyle.None;
        this.WindowState = WindowState.Normal;
        this.Top = 0;
        this.Left = 0;
        this.ResizeMode = ResizeMode.NoResize;
        this.Width = SystemParameters.PrimaryScreenWidth + 10;
        this.Height = SystemParameters.PrimaryScreenHeight + 10;
      }
    }

    private void GraphSetting_Click(object sender, RoutedEventArgs e) {
      SettingGraphsWindow sgw = new SettingGraphsWindow(settingCommon.graphSetting);
      sgw.ShowDialog();
      string result = Core.Serialize.SettingCommon(settingCommon);
      Service.WorkFile.Do(Core.Work.EnvPath + "setting.dat", Service.WorkFileMode.WriteNew, result);
    }

    private void UnitSetting_Click(object sender, RoutedEventArgs e) {
      UnitSetWindow usw = new UnitSetWindow();
      usw.Owner = this;
      usw.ShowDialog();
      setting.UpdateBindPramUnits(settingUnits);
    }

    private void PipeCounter_Click(object sender, RoutedEventArgs e) {
      Process.Start(Core.Work.EnvPath + "PipeCounter.exe");
    }

    private void PipeEmul_Click(object sender, RoutedEventArgs e) {
      Process.Start(Core.Work.EnvPath + "EmulPipes.exe");
    }

    private void PipeCounterSet_Click(object sender, RoutedEventArgs e) {
      PipeCounterSetWindow pcsw = new PipeCounterSetWindow(Core.Work.EnvPath);
      pcsw.ShowDialog();
    }

    #endregion

    #region Меню Права доступа
    private void Admin_Click(object sender, RoutedEventArgs e) {
      new ChangeUserWindow(this).ShowDialog();
    }

    private bool isAdmin = true;
    public bool IsAdmin {
      get { return isAdmin; }
      set {
        isAdmin = value;
        menuData.IsEnabled = isAdmin;
        menuWindows.IsEnabled = isAdmin;
        menuWork.IsEnabled = isAdmin;
        menuCommonSetting.IsEnabled = isAdmin;
        menuSettingGraph.IsEnabled = isAdmin;
        menuUnits.IsEnabled = isAdmin;
        menuNew.IsEnabled = isAdmin;
        menuSave.IsEnabled = isAdmin;
        menuLoad.IsEnabled = isAdmin;
      }
    }
    #endregion

    #region Меню Справка
    private void License_Click(object sender, RoutedEventArgs e) {
      LicenseInfo li = new LicenseInfo();
      li.ShowDialog();
    }

    private void Update_Click(object sender, RoutedEventArgs e) {
      Thread update = new Thread(new ThreadStart(UpdateVersion));
      update.Start();
    }

    private void UpdateVersion() {
      Application.Current.Dispatcher.Invoke((Action)(() => {
        string version = "";
        try {
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://spautomation.ru/spark/version2.txt");
          HttpWebResponse response = (HttpWebResponse)request.GetResponse();
          if (response.StatusCode == HttpStatusCode.OK) {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            if (response.CharacterSet == null) {
              readStream = new StreamReader(receiveStream);
            } else {
              readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }
            version = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
          } else {
            new Service.MessageView("Сервер обновлений недоступен", "", Service.MessageViewMode.Attention).ShowDialog();
          }
        } catch {
          new Service.MessageView("Сервер обновлений недоступен", "", Service.MessageViewMode.Attention).ShowDialog();
        }
        if (version == "")
          return;
        string curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,
           System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);
        if (version != curVersion) {
          string remoteUri = "http://spautomation.ru/spark/";
          string fileName = "Updater.exe", myStringWebResource = null;
          WebClient myWebClient = new WebClient();
          myWebClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
          myStringWebResource = remoteUri + fileName;
          Uri url = new Uri(remoteUri + fileName);
          myWebClient.DownloadFileAsync(url, Core.Work.EnvPath + fileName);
          Thread.Sleep(500);
          Process updater = new Process();
          updater.StartInfo.FileName = "updater";
          updater.StartInfo.Arguments = "d " + "\""+Core.Work.EnvPath;
          for (int i = 0; i < 10; i++) {
            if (File.Exists(Core.Work.EnvPath + "Updater.exe")) {
              try {
                updater.Start();
                break;
              } catch (Exception e) {
                Service.Log.LogWrite(Core.Work.EnvPath, e.Message, e.ToString());
              }
            } else {
              Thread.Sleep(500);
            }
          }
        } else {
          new Service.MessageView("Текущая версия актуальна", "", Service.MessageViewMode.Message).ShowDialog();
        }
      }));
    }

    private void CheckUpdate() {
      Application.Current.Dispatcher.Invoke((Action)(() => {
        string version = "";
        try {
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://spautomation.ru/spark/version2.txt");
          HttpWebResponse response = (HttpWebResponse)request.GetResponse();
          if (response.StatusCode == HttpStatusCode.OK) {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            if (response.CharacterSet == null) {
              readStream = new StreamReader(receiveStream);
            } else {
              readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            }
            version = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
          }
        } catch {
        }
        if (version == "")
          return;
        string curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0,
           System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Length - 2);
        if (version != curVersion) {
          new Service.MessageView("Имеется более новая версия", "", Service.MessageViewMode.Attention).ShowDialog();
        }
      }));
    }
    #endregion

    #region Запрет нажатия кнопки WIN и скрытие панели задач

    private const int WH_KEYBOARD_LL = 13;

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT {
      public System.Windows.Forms.Keys key;
    }

    private LowLevelKeyboardProcDelegate m_callback;

    private IntPtr m_hHook;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);

    [Flags]
    public enum SetWindowPosFlags : uint {
      SWP_ASYNCWINDOWPOS = 0x4000,
      SWP_DEFERERASE = 0x2000,
      SWP_DRAWFRAME = 0x0020,
      SWP_FRAMECHANGED = 0x0020,
      SWP_HIDEWINDOW = 0x0080,
      SWP_NOACTIVATE = 0x0010,
      SWP_NOCOPYBITS = 0x0100,
      SWP_NOMOVE = 0x0002,
      SWP_NOOWNERZORDER = 0x0200,
      SWP_NOREDRAW = 0x0008,
      SWP_NOREPOSITION = 0x0200,
      SWP_NOSENDCHANGING = 0x0400,
      SWP_NOSIZE = 0x0001,
      SWP_NOZORDER = 0x0004,
      SWP_SHOWWINDOW = 0x0040,
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    public IntPtr LowLevelKeyboardHookProc_win(int nCode, IntPtr wParam, IntPtr lParam) {
      if (nCode >= 0) {
        KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
        if (objKeyInfo.key == System.Windows.Forms.Keys.RWin || objKeyInfo.key == System.Windows.Forms.Keys.LWin) {
          return (IntPtr)1;
        }
      }
      return CallNextHookEx(m_hHook, nCode, wParam, lParam);
    }

    private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

    public void SetHook() {
      m_callback = LowLevelKeyboardHookProc_win;
      m_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback, GetModuleHandle(IntPtr.Zero), 0);
      IntPtr hid = FindWindow("Shell_TrayWnd", "");
      ShowWindow(hid, 0);
      EnableWindow(hid, false);
      IntPtr start = FindWindow("Button", "");
      SetWindowPos(start, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_HIDEWINDOW);
      EnableWindow(start, false);
    }

    public void Unhook() {
      UnhookWindowsHookEx(m_hHook);
      IntPtr hid = FindWindow("Shell_TrayWnd", "");
      ShowWindow(hid, 1);
      EnableWindow(hid, true);
      IntPtr start = FindWindow("Button", "");
      SetWindowPos(start, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_SHOWWINDOW);
      EnableWindow(start, true);
    }
    #endregion

  }
}