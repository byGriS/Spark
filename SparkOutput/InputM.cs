namespace SparkOutput {
  public class InputM {
    public Service.InputCommon Input { get; set; }
    public Modbus.Device.ModbusSerialSlave SlaveSerial { get; set; }
    public Modbus.Device.ModbusTcpSlave SlaveTCP { get; set; }
  }
}
