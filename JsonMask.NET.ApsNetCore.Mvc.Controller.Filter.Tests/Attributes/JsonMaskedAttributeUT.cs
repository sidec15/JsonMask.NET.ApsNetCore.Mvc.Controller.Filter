using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Tests.Attributes
{
  internal class JsonMaskedAttributeUT
  {
    [Test]
    public void Attribute_ShouldBeApplicableToMethod()
    {
      // Arrange
      var methodInfo = typeof(TestController).GetMethod(nameof(TestController.TestMethod));

      // Act
      var attribute = methodInfo?.GetCustomAttribute<JsonMaskedAttribute>();

      // Assert
      Assert.That(attribute, Is.Not.Null);
    }

    [Test]
    public void Attribute_ShouldSetDefaultQueryParameterName_WhenNoneProvided()
    {
      // Arrange & Act
      var attribute = new JsonMaskedAttribute();

      // Assert
      Assert.That(attribute.QueryParameterName, Is.EqualTo("projection"));
    }

    [Test]
    public void Attribute_ShouldSetCustomQueryParameterName_WhenProvided()
    {
      // Arrange
      var customName = "customParameter";

      // Act
      var attribute = new JsonMaskedAttribute(customName);

      // Assert
      Assert.That(attribute.QueryParameterName, Is.EqualTo(customName));
    }

    // Dummy controller for testing
    private class TestController
    {
      [JsonMasked]
      public void TestMethod() { }
    }
  }
}
