using System.ServiceModel;

namespace Service {
  [ServiceContract]
  public interface IContract {
    [OperationContract]
    void SetValue(Service.PairDataParam[] values);

    [OperationContract]
    void ChangeState(string source, int state);
  }
}
