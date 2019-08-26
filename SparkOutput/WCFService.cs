using System.ServiceModel;

namespace SparkOutput {
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  class WCFService : Service.IContractOut {
    public bool UpdateOutputs(string[] data) {
      Program.UpdateOutputs(data);
      return true;
    }

    public void UpdateDate(string titleOutput, Service.PairDataParam[] values) {
      Program.UpdateData(titleOutput, values);
    }

    public int WatchDog() {
      return Program.Status;
    }
  }
}
