using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  public class JsonMaskedSwaggerFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var jsonMaskedAttribute = context.MethodInfo
          .GetCustomAttributes(true)
          .OfType<JsonMaskedAttribute>()
          .FirstOrDefault();

      if (jsonMaskedAttribute != null)
      {
        operation.Parameters ??= new List<OpenApiParameter>();

        var description = string.Join(Environment.NewLine, new string[]
        {
          "Fields to retrieve.",
          string.Empty,
          "The syntax is loosely based on XPath:",
          "- a,b,c comma-separated list will select multiple fields",
          "- a/b/c path will select a field from its parent",
          "- a(b,c) sub-selection will select many fields from a parent",
          "- a/'*'/c the star * wildcard will select all items in a field.",
          string.Empty,
          "Check the [syntax](https://www.npmjs.com/package/json-mask)."
        });

        operation.Parameters.Add(new OpenApiParameter
        {
          Name = jsonMaskedAttribute.QueryParameterName,
          In = ParameterLocation.Query,
          Description = description,
          Required = false,
          Schema = new OpenApiSchema
          {
            Type = "string"
          }
        });
      }
    }
  }


}
