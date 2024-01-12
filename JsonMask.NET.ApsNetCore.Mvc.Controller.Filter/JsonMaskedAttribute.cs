namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  [AttributeUsage(AttributeTargets.Method)]
  public class JsonMaskedAttribute : Attribute
  {
    public string QueryParameterName { get; private set; }

    public JsonMaskedAttribute(string queryParameterName = "projection")
    {
      QueryParameterName = queryParameterName;
    }
  }

}
