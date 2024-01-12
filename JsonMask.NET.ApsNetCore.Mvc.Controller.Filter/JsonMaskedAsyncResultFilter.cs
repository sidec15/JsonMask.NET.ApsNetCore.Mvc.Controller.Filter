﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  public class JsonMaskedAsyncResultFilter : IAsyncResultFilter
  {
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

        //if (executedContext.Result is ObjectResult objectResult)
        //{
        if (projectionValue != null)
        {
          // Modify the response here based on the projection value
          //objectResult.Value = ModifyResponse(objectResult.Value, projectionValue);

          // Set the memory stream position to the beginning
          memoryStream.Seek(0, SeekOrigin.Begin);

          // Read the memory stream into a string
          var jsonResponse = await new StreamReader(memoryStream).ReadToEndAsync();

          // Modify the response here as needed
          var modifiedResponse = ModifyResponse(jsonResponse, projectionValue);

          // Write the modified response back to the original stream
          var responseBytes = Encoding.UTF8.GetBytes(modifiedResponse);
          await originalBodyStream.WriteAsync(responseBytes, 0, responseBytes.Length);

          // Set the original stream back
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
      var projection = Masker.Mask(originalResponse, projectionValue);

      return projection;
    }
  }

}
