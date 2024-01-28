
using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  public interface IJsonAttributeChecker
  {
    bool TryGetProjectionValue(ResultExecutingContext context, out string projectionValue);
  }

  public class JsonAttributeChecker : IJsonAttributeChecker
  {

    public bool TryGetProjectionValue(ResultExecutingContext context, out string projectionValue)
    {
      projectionValue = null;

      if (context.Controller is ControllerBase controller)
      {
        var controllerAttribute = context.Controller
          .GetType()
          .GetCustomAttributes(typeof(JsonMaskedControllerAttribute), true)
          .FirstOrDefault() as JsonMaskedControllerAttribute;

        var methodAttribute = controller
          .ControllerContext
          .ActionDescriptor
          .MethodInfo
          .GetCustomAttributes(true)
          .OfType<JsonMaskedAttribute>()
          .FirstOrDefault();

        bool shouldApplyFilter = methodAttribute != null ||
                               (controllerAttribute != null &&
                               (controllerAttribute.ApplyToHttpGetOnly == false ||
                                controller.ControllerContext.ActionDescriptor.MethodInfo
                                .GetCustomAttributes(true)
                                .OfType<HttpGetAttribute>()
                                .FirstOrDefault() != null));

        if (shouldApplyFilter)
        {

          // get projection value
          if (methodAttribute != null)
          {
            if (context.HttpContext.Request.Query.TryGetValue(methodAttribute.QueryParameterName, out var projectionValueStringValues))
            {
              projectionValue = projectionValueStringValues;
            }
          }
          if (projectionValue == null)
          {
            if (controllerAttribute != null)
            {
              if (context.HttpContext.Request.Query.TryGetValue(controllerAttribute.QueryParameterName, out var projectionValueStringValues))
              {
                projectionValue = projectionValueStringValues;
              }
            }
          }
        }
      }

      return projectionValue != null;
    }

  }
}
