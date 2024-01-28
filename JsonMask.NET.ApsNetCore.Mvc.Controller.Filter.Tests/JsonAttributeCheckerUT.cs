using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes;
using NUnit.Compatibility;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Net.Http;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Tests
{
  internal class JsonAttributeCheckerUT
  {

    private IJsonAttributeChecker _attributeChecker;
    private ResultExecutingContext _resultExecutingContext;
    private ActionDescriptor _actionDescriptor;
    private ControllerContext _controllerContext;
    private HttpContext _httpContext;
    private HttpRequest _httpRequest;
    private HttpResponse _httpResponse;

    private TestController1 _controller;

    public void SetUpControllerAndAction(Type controllerType, string methodName)
    {
      _attributeChecker = new JsonAttributeChecker();

      _httpContext = new DefaultHttpContext();

      _controller = new TestController1();
      //var testMethodInfo = typeof(TestController).GetMethod(nameof(TestController.TestAction));
      var testMethodInfo = controllerType.GetMethod(methodName);
      _actionDescriptor = new ControllerActionDescriptor
      {
        MethodInfo = testMethodInfo
      };

      _controllerContext = new ControllerContext(new ActionContext(_httpContext, A.Fake<RouteData>(), _actionDescriptor));

      _controller.ControllerContext = _controllerContext;

      _httpRequest = _httpContext.Request;
      _httpResponse = _httpContext.Response;

      _resultExecutingContext = new ResultExecutingContext(_controllerContext, A.Fake<IList<IFilterMetadata>>(), new EmptyResult(), _controller);
    }

    [Test]
    public void Should_ApplyFilter_When_ControllerHasAttribute_And_QueryIsPresent()
    {
      SetUpControllerAndAction(typeof(TestController1), nameof(TestController1.TestAction));

      string projectionValue = "value";
      _httpRequest.Query = new QueryCollection(new Dictionary<string, StringValues>
      {
          { GlobalVariables.PROJECTION_QUERY_PARAM, projectionValue }
      });

      // Act
      bool result = _attributeChecker.TryGetProjectionValue(_resultExecutingContext, out string actualProjeValue);

      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(result, Is.True);
        Assert.That(actualProjeValue, Is.EqualTo(projectionValue));
      });
    }

    [Test]
    public void Should_Not_ApplyFilter_When_ControllerHasAttribute_And_QueryIsNotPresent()
    {
      SetUpControllerAndAction(typeof(TestController1), nameof(TestController1.TestAction));

      // Act
      bool result = _attributeChecker.TryGetProjectionValue(_resultExecutingContext, out string actualProjeValue);

      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(result, Is.False);
        Assert.That(actualProjeValue, Is.Null);
      });
    }

    [JsonMaskedController]
    private class TestController1 : ControllerBase
    {
      [HttpGet]
      public void TestAction() { }
    }

  }

}
