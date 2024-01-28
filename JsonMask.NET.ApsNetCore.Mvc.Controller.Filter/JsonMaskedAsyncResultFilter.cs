using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
    public class JsonMaskedAsyncResultFilter : IAsyncResultFilter
  {

    private readonly IJsonAttributeChecker _attributeChecker;
    private readonly IMaskerService _maskerService;

    public JsonMaskedAsyncResultFilter(IJsonAttributeChecker attributeChecker, IMaskerService maskerService)
    {
      _attributeChecker = attributeChecker;
      _maskerService = maskerService;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
      Stream memoryStream = null;
      Stream originalBodyStream = context.HttpContext.Response.Body;
      bool isJsonMasked = false;
      try
      {

        isJsonMasked = _attributeChecker.TryGetProjectionValue(context, out string projectionValue);
        if (isJsonMasked)
        {
          // Temporary memory stream
          memoryStream = new MemoryStream();
          context.HttpContext.Response.Body = memoryStream;
        }

        // Execute the action and get the result
        var executedContext = await next();

        if (isJsonMasked)
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
        if (isJsonMasked)
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
