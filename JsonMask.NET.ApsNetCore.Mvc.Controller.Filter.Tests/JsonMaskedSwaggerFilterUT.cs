using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace JsonMask.NET.ApsNetCore.Controller.Filter.Mvc.Tests
{
  [TestFixture]
  public class JsonMaskedSwaggerFilterTests
  {

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Apply_AddsQueryParameter_WhenJsonMaskedAttributeIsPresent(bool isParametersNull)
    {
      // Arrange
      var filter = new JsonMaskedSwaggerFilter();
      var operation = new OpenApiOperation();

      // Create a ControllerActionDescriptor that mimics having the JsonMaskedAttribute
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.TestAction));
      var actionDescriptor = new ControllerActionDescriptor
      {
        MethodInfo = methodInfo
      };

      var context = new OperationFilterContext(
          new ApiDescription { ActionDescriptor = actionDescriptor },
          null, // Replace with appropriate IApiDescriptionProvider
          null, // Replace with appropriate ISchemaGenerator
          methodInfo  // Replace with appropriate ISchemaRepository
      );

      if(isParametersNull )
      {
        operation.Parameters = null;
      }

      // Act
      filter.Apply(operation, context);

      // Assert
      var queryParameter = operation.Parameters.FirstOrDefault(p => p.Name == TestController.QUERY_PARAMETER_NAME);
      Assert.That(queryParameter, Is.Not.Null, $"Query parameter '{TestController.QUERY_PARAMETER_NAME}' was not added.");
      Assert.Multiple(() =>
      {
        Assert.That(queryParameter.In, Is.EqualTo(ParameterLocation.Query), "Parameter location is not correct.");
        Assert.That(queryParameter.Schema.Type, Is.EqualTo("string"), "Parameter type is not correct.");
      });
      Assert.Multiple(() =>
      {
        Assert.That(queryParameter.Required, Is.False);
        Assert.That(queryParameter.Schema.Type, Is.EqualTo("string"));
      });
    }

    // Dummy controller and action for test
    private class TestController
    {
      public const string QUERY_PARAMETER_NAME = "foo";
      [JsonMasked(QUERY_PARAMETER_NAME)]
      public void TestAction() { }
    }
  }
}
