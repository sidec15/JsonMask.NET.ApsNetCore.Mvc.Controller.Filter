using JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Attributes;
using NUnit.Framework;
using System;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter.Tests.Attributes
{
  internal class JsonMaskedControllerAttributeUT
  {
    [Test]
    public void Attribute_ShouldBeApplicableToClass()
    {
      // Arrange
      var classAttribute = typeof(TestController)
          .GetCustomAttributes(typeof(JsonMaskedControllerAttribute), false)
          .FirstOrDefault() as JsonMaskedControllerAttribute;

      // Act & Assert
      Assert.That(classAttribute, Is.Not.Null);
    }

    [Test]
    public void Attribute_ShouldSetDefaultValues_WhenNoParametersProvided()
    {
      // Arrange & Act
      var attribute = new JsonMaskedControllerAttribute();
      Assert.Multiple(() =>
      {

        // Assert
        Assert.That(attribute.ApplyToHttpGetOnly, Is.True);
        Assert.That(attribute.QueryParameterName, Is.EqualTo("projection"));
      });
    }

    [Test]
    public void Attribute_ShouldSetCustomValues_WhenParametersProvided()
    {
      // Arrange
      var customApplyToHttpGetOnly = false;
      var customQueryParameterName = "customParam";

      // Act
      var attribute = new JsonMaskedControllerAttribute(customApplyToHttpGetOnly, customQueryParameterName);
      Assert.Multiple(() =>
      {
        // Assert
        Assert.That(attribute.ApplyToHttpGetOnly, Is.EqualTo(customApplyToHttpGetOnly));
        Assert.That(attribute.QueryParameterName, Is.EqualTo(customQueryParameterName));
      });
    }

    // Dummy controller for testing
    [JsonMaskedController]
    private class TestController
    {
    }
  }
}
