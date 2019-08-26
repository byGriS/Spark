using System.ServiceModel;

namespace Service {
  [ServiceContract]
  public interface IContractOut {
    [OperationContract]
    bool UpdateOutputs(string[] data);

    [OperationContract]
    void UpdateDate(string titleOutput, PairDataParam[] values);

    [OperationContract]
    int WatchDog();
  }
}
