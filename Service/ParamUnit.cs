namespace Service {
  public class ParamUnit {
    public string Title { get; set; }

    public ParamUnit Clone() {
      ParamUnit pu = new ParamUnit() {
        Title = this.Title
      };
      return pu;
    }

    public override bool Equals(object obj) {
      if (obj == null || !(obj is ParamUnit))
        return false;
      return (((ParamUnit)obj).Title == this.Title);
    }
  }
}
