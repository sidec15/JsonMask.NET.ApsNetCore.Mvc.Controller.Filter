using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  [TestFixture]
  public class JsonMaskedAsyncResultFilterUT
  {
    [Test]
    public async Task OnResultExecutionAsync_ModifiesResponse_WhenProjectionIsProvided()
    {
      var originalResponseBody = @"{""p1"":""k1"", ""p2"":""k2""}";

      // Arrange
      IMaskerService maskerService = A.Fake<IMaskerService>();

      var filter = new JsonMaskedAsyncResultFilter(maskerService);

      var httpContext = new DefaultHttpContext();
      var memoryStream = new MemoryStream();
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

      var projectionValue = "p1";
      var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "projection", new StringValues(projectionValue) }
            });
      httpContext.Request.Query = new QueryCollection(queryCollection);

      // Write the original response to the memory stream
      var writer = new StreamWriter(memoryStream);
      writer.Write(originalResponseBody);
      writer.Flush();
      memoryStream.Position = 0;

      var expected = @"{""p1"":""k1""}";
      A.CallTo(() => maskerService.Mask(originalResponseBody, projectionValue)).Returns(expected);

      // Act
      await filter.OnResultExecutionAsync(resultExecutingContext, () =>
      {
        var memoryStreamInt = httpContext.Response.Body;
        var writerInt = new StreamWriter(memoryStreamInt);
        writerInt.Write(originalResponseBody);
        writerInt.Flush();
        return Task.FromResult(resultExecutedContext);
      });

      // Assert
      var str = resultExecutingContext.HttpContext.Response.Body;
      str.Position = 0;
      var modifiedResponse = new StreamReader(str).ReadToEnd();
      writer.Close();
      Assert.That(modifiedResponse, Is.EqualTo(expected));

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
