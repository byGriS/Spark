namespace Service {
  public class InputEthernet : InputCommon, Input {
    public string IPSlave { get; set; }
    public int Port { get; set; }

    public Input Clone() {
      InputEthernet input = new InputEthernet() {
        IPSlave = this.IPSlave,
        Port = this.Port,
        Title = this.Title,
        InputType = this.InputType,
        IsUsed = this.IsUsed,
        IDSlave = this.IDSlave
      };
      foreach (DataParam dp in this.ListDataParams) {
        input.ListDataParams.Add(dp.Clone());
      }
      foreach (DataParamOutput dp in this.ListDataParamsOut) {
        input.ListDataParamsOut.Add(dp.Clone());
      }
      return input;
    }
  }
}
