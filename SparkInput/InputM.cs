namespace SparkInput {
  public class InputM {
    public Service.Input Input { get; set; }
    public Modbus.Device.IModbusSerialMaster MasterSerial { get; set; }
    public Modbus.Device.ModbusIpMaster MasterTCP { get; set; }
  }
}
