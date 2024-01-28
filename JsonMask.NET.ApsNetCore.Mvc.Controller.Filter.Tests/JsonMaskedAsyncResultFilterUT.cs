using FakeItEasy;
using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using System.Net;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  [TestFixture]
  public class JsonMaskedAsyncResultFilterUT
  {
    [Test]
    public async Task WhenProjectionIsProvided()
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
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.WithMask));
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
      Assert.That(modifiedResponse, Is.EqualTo(expected));

    }

    [Test]
    public async Task WhenProjectionIsNotProvided()
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
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.WithMask));
      var actionDescriptor = new ControllerActionDescriptor
      {
        MethodInfo = methodInfo
      };

      var routeData = new RouteData();
      var controllerContext = new ControllerContext(new ActionContext(httpContext, routeData, actionDescriptor));

      controller.ControllerContext = controllerContext;

      var resultExecutingContext = new ResultExecutingContext(controllerContext, filters, new EmptyResult(), controller);
      var resultExecutedContext = new ResultExecutedContext(controllerContext, filters, new EmptyResult(), controller);

      // Write the original response to the memory stream
      var writer = new StreamWriter(memoryStream);
      writer.Write(originalResponseBody);
      writer.Flush();
      memoryStream.Position = 0;

      var expected = originalResponseBody;

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

    [Test]
    public async Task WhenAttributeIsNotPresent()
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
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.WithoutMask));
      var actionDescriptor = new ControllerActionDescriptor
      {
        MethodInfo = methodInfo
      };

      var routeData = new RouteData();
      var controllerContext = new ControllerContext(new ActionContext(httpContext, routeData, actionDescriptor));

      controller.ControllerContext = controllerContext;

      var resultExecutingContext = new ResultExecutingContext(controllerContext, filters, new EmptyResult(), controller);
      var resultExecutedContext = new ResultExecutedContext(controllerContext, filters, new EmptyResult(), controller);

      // Write the original response to the memory stream
      var writer = new StreamWriter(memoryStream);
      writer.Write(originalResponseBody);
      writer.Flush();
      memoryStream.Position = 0;

      var expected = originalResponseBody;

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

    [Test]
    public void ErrorModifyingResponse()
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
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.WithMask));
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

      Exception exception = new ("^_^");
      A.CallTo(() => maskerService.Mask(A<string>._, A<string>._)).Throws(exception);

      // Act
      Exception ex = Assert.ThrowsAsync<Exception>(async delegate
      {
        await filter.OnResultExecutionAsync(resultExecutingContext, () =>
        {
          return Task.FromResult(resultExecutedContext);
        });
      });
      Assert.That(ex, Is.EqualTo(exception));

    }


  }

  public class TestController : ControllerBase
  {
    [JsonMasked]
    public IActionResult WithMask()
    {
      return Ok();
    }
    public IActionResult WithoutMask()
    {
      return Ok();
    }
  }
}
