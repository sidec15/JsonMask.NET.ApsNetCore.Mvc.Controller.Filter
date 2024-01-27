using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  public class JsonMaskedAsyncResultFilter : IAsyncResultFilter
  {

    private readonly IMaskerService _maskerService;

    public JsonMaskedAsyncResultFilter(IMaskerService maskerService)
    {
      _maskerService = maskerService;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
      string projectionValue = null;
      Stream memoryStream = null;
      Stream originalBodyStream = context.HttpContext.Response.Body;
      try
      {

        if (context.Controller is ControllerBase controller)
        {
          var jsonMaskedAttribute = controller
            .ControllerContext
            .ActionDescriptor
            .MethodInfo
            .GetCustomAttributes(true)
            .OfType<JsonMaskedAttribute>()
            .FirstOrDefault();

          if (jsonMaskedAttribute != null)
          {
            if (context.HttpContext.Request.Query.TryGetValue(jsonMaskedAttribute.QueryParameterName, out var projectionValueStringValues))
            {
              // Temporary memory stream
              memoryStream = new MemoryStream();
              context.HttpContext.Response.Body = memoryStream;
              projectionValue = projectionValueStringValues;
            }
          }
        }

        // Execute the action and get the result
        var executedContext = await next();

        if (projectionValue != null)
        {
          // Set the memory stream position to the beginning
          memoryStream.Seek(0, SeekOrigin.Begin);

          // Read the memory stream into a string
          var jsonResponse = await new StreamReader(memoryStream).ReadToEndAsync();

          // Modify the response here as needed
          var modifiedContent = ModifyResponse(jsonResponse, projectionValue);

          // Write the modified content back to the original stream
          var modifiedBytes = Encoding.UTF8.GetBytes(modifiedContent);

          // Write the modified content to the original response body
          // Note: We do not seek the originalBodyStream because it's not supported
          await originalBodyStream.WriteAsync(modifiedBytes, 0, modifiedBytes.Length);
          await originalBodyStream.FlushAsync(); // Ensure all bytes are written to the original stream

          // Restore the original body stream
          context.HttpContext.Response.Body = originalBodyStream;

        }

      }
      finally
      {
        // Ensure the memory stream is disposed
        memoryStream?.Dispose();

        // Restore the original response body
        if (context.HttpContext.Response.Body != originalBodyStream)
        {
          context.HttpContext.Response.Body = originalBodyStream;
        }
      }
    }

    private string ModifyResponse(string originalResponse, string projectionValue)
    {
      var projection = _maskerService.Mask(originalResponse, projectionValue);

      return projection;
    }
  }

}
