using System.IO.Ports;

namespace Service {
  public class InputSerial : InputCommon, Input {
    public string PortName { get; set; }
    public int BaudRate { get; set; }
    public int DataBits { get; set; }
    public Parity Parity { get; set; }
    public StopBits StopBits { get; set; }
    public Handshake Handshake { get; set; }
    public int ReadBufferSize { get; set; }
    public int ReadTimeout { get; set; }
    public bool Dtr { get; set; }
    public bool Rts { get; set; }

    public string SymbolSplitter { get; set; }

    public Input Clone() {
      InputSerial input = new InputSerial() {
        PortName = this.PortName,
        BaudRate = this.BaudRate,
        DataBits = this.DataBits,
        Parity = this.Parity,
        StopBits = this.StopBits,
        Handshake = this.Handshake,
        ReadBufferSize = this.ReadBufferSize,
        ReadTimeout = this.ReadTimeout,
        Dtr = this.Dtr,
        Rts = this.Rts,
        Title = this.Title,
        InputType = this.InputType,
        IsUsed = this.IsUsed,
        SymbolSplitter = this.SymbolSplitter,
        IDSlave = this.IDSlave
      };
      foreach(DataParam dp in this.ListDataParams) {
        input.ListDataParams.Add(dp.Clone());
      }
      foreach (DataParamOutput dp in this.ListDataParamsOut) {
        input.ListDataParamsOut.Add(dp.Clone());
      }
      return input;
    }
  }
}
