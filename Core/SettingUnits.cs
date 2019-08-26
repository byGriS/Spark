using System.Collections.ObjectModel;

namespace Core {
  public class SettingUnits{
    private ObservableCollection<Service.ParamType> paramsTypes = new ObservableCollection<Service.ParamType>();
    public ObservableCollection<Service.ParamType> ParamsTypes { get { return paramsTypes; } }

    public Service.ParamType GetParamTypeByTitle(string title) {
      foreach (Service.ParamType paramType in ParamsTypes)
        if (paramType.Title == title)
          return paramType;
      Service.ParamType pt = new Service.ParamType { Title = title };
      paramsTypes.Add(pt);
      return pt;
    }

    public Service.ParamUnit GetUnitByTitle(Service.ParamType paramType, string title) {
      foreach (Service.ParamType type in ParamsTypes)
        if (type.Title == paramType.Title) {
          foreach (Service.ParamUnit unit in type.ListUnits)
            if (unit.Title == title)
              return unit;
        }
      Service.ParamUnit pu = new Service.ParamUnit { Title = title };
      paramType.ListUnits.Add(pu);
      return pu;
    }

    public SettingUnits Clone() {
      SettingUnits su = new SettingUnits();
      foreach(Service.ParamType pt in this.paramsTypes) {
        su.ParamsTypes.Add(pt.Clone());
      }
      return su;
    }
  }
}
