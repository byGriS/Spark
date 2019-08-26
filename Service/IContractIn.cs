using System.ServiceModel;

namespace Service {
  [ServiceContract]
  public interface IContractIn {
    [OperationContract]
    bool UpdateInputs(string[] data, string titleDB);

    [OperationContract]
    void SendData(int idDataParam, float newValue);

    [OperationContract]
    void SendStart(string titleInput);

    [OperationContract]
    void SendSoOn(string titleInput);

    [OperationContract]
    int WatchDog();
  }
}
