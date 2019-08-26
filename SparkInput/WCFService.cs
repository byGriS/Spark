using Service;
using System;
using System.ServiceModel;

namespace SparkInput {
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  class WCFService : IContractIn {
    public void SendData(int idDataParam, float newValue) {
      Program.SendData(idDataParam, newValue);
    }

    public void SendStart(string titleInput) {
      Program.SendStart(titleInput);
    }

    public void SendSoOn(string titleInput) {
      Program.SendSoOn(titleInput);
    }

    public bool UpdateInputs(string[] data, string titleDB) {
      Program.UpdateInputs(data, titleDB);
      return true;
    }

    public int WatchDog() {
      throw new NotImplementedException();
    }
  }
}
