
namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class JsonMaskedControllerAttribute : Attribute
  {
    public bool ApplyToHttpGetOnly { get; private set; }
    public string QueryParameterName { get; private set; }

    public JsonMaskedControllerAttribute(bool applyToHttpGetOnly = true, string queryParameterName = "projection")
    {
      ApplyToHttpGetOnly = applyToHttpGetOnly;
      QueryParameterName = queryParameterName;
    }
  }

}
