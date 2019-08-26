using System.Collections.Generic;

namespace Core {
  public class DataSpark {
    public string numWork;
    public List<ParamSpark> dataParam = new List<ParamSpark>();
  }

  public class ParamSpark {
    public string timestamp { get; set; }
    public string paramTitle { get; set; }
    public float paramValue { get; set; }
  }

  public class WorkSpark {
    public string
      id,
      team,
      field,
      bush,
      well,
      nktmm,
      lengthplan,
      speedplan,
      waterplan,
      startwork,
      endwork,
      numwork;

    public override string ToString() {
      return startwork;
    }
  }

  public class DataSparkArchive {
    public string numWork;
    public List<List<ParamSpark>> dataParam = new List<List<ParamSpark>>();
  }
}
