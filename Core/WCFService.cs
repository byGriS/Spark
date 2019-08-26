using System.ServiceModel;

namespace Core {
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class WCFService : Service.IContract {
    public void SetValue(Service.PairDataParam[] values) {
      Work.UpdateValues(values);
      Work.SendDataToWebServer(values);
    }

    public void ChangeState(string source, int state) {
      Work.ChangeState(source, state);
    }
  }
}
