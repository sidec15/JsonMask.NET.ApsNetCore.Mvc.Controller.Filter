using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System.IO;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  [TestFixture]
  public class JsonMaskedAsyncResultFilterTests
  {
    [Test]
    public async Task OnResultExecutionAsync_ModifiesResponse_WhenProjectionIsProvided()
    {
      var originalResponseBody = @"{p1: ""k1"", p2:""k2""}";

      // Arrange
      var filter = new JsonMaskedAsyncResultFilter();

      var httpContext = new DefaultHttpContext();
      var memoryStream = new MemoryStream();
      var writer = new StreamWriter(memoryStream);
      httpContext.Response.Body = memoryStream;

      var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
      var filters = new List<IFilterMetadata>();
      var controller = new TestController();

      // Create ActionDescriptor with JsonMaskedAttribute
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.TestAction));
      var actionDescriptor = new ControllerActionDescriptor
      {
        MethodInfo = methodInfo
      };

      var routeData = new RouteData();
      var controllerContext = new ControllerContext(new ActionContext(httpContext, routeData, actionDescriptor));

      controller.ControllerContext = controllerContext;

      var resultExecutingContext = new ResultExecutingContext(controllerContext, filters, new EmptyResult(), controller);
      var resultExecutedContext = new ResultExecutedContext(controllerContext, filters, new EmptyResult(), controller);

      var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "projection", new StringValues("p1") }
            });
      httpContext.Request.Query = new QueryCollection(queryCollection);

      // Write the original response to the memory stream
      writer.Write(originalResponseBody);
      writer.Flush();
      memoryStream.Position = 0;

      // Act
      await filter.OnResultExecutionAsync(resultExecutingContext, () => Task.FromResult(resultExecutedContext));

      // Assert
      memoryStream.Position = 0;
      var modifiedResponse = new StreamReader(memoryStream).ReadToEnd();
      writer.Close();
      Assert.That(modifiedResponse, Is.EqualTo(@"{p1: ""k1""}"));

    }
  }

  public class TestController : ControllerBase
  {
    [JsonMasked]
    public IActionResult TestAction()
    {
      return Ok();
    }
  }
}
