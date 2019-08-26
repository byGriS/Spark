using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Service {
  public abstract class InputCommon : INotifyPropertyChanged {

    public delegate void ChangeType(Input input, InputType inputType);
    public event ChangeType OnChangeType;


    private string title;
    public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }
    public bool IsUsed { get; set; }
    public int IDSlave { get; set; }

    public DateTime LastSendWebServer { get; set; }

    private InputType inputType = InputType.COMText;
    public InputType InputType {
      get { return inputType; }
      set {
        inputType = value;
        OnChangeType?.Invoke((Input)this, inputType);
      }
    }

    private ObservableCollection<DataParam> listDataParams = new ObservableCollection<DataParam>();
    public ObservableCollection<DataParam> ListDataParams { get { return listDataParams; } set { listDataParams = value; } }

    private ObservableCollection<DataParamOutput> listDataParamsOut = new ObservableCollection<DataParamOutput>();
    public ObservableCollection<DataParamOutput> ListDataParamsOut { get { return listDataParamsOut; } set { listDataParamsOut = value; } }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string prop = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
