
# JsonMask.NET.ApsNetCore.Mvc.Controller.Filter

This ASP.NET Core MVC filter empowers developers to selectively retrieve specific JSON elements from a resource's response. Built on the robust capabilities of [JsonMask.NET](https://github.com/sidec15/JsonMask.NET), this library offers a streamlined and efficient approach to manipulating JSON responses. By leveraging query parameters, developers can easily tailor the JSON output, ensuring that clients receive only the data they need. This not only enhances the flexibility of API responses but also optimizes data transfer and processing, making it an ideal solution for scenarios where bandwidth and processing efficiency are crucial.

## Features

- Filter JSON responses in ASP.NET Core MVC applications.
- Specify elements of a JSON response to include using simple query parameters.
- Easy integration with existing ASP.NET Core MVC applications.

## Getting Started

### Prerequisites

- .NET 6 or higher
- Familiarity with MVC pattern in ASP.NET Core

### Installation

Install `JsonMask.NET.ApsNetCore.Mvc.Controller.Filter` via NuGet:

```sh
Install-Package JsonMask.NET.ApsNetCore.Mvc.Controller.Filter -Version [Version]
```

Replace `[Version]` with the desired version number.

### Setup

#### 1. Add JsonMask Service and Filter

In your `Startup.cs`, register the necessary services and filters:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddJsonMask();

    // ...
}
```

#### 2. Add Swagger Support

If you are using Swagger, add the `JsonMaskedSwaggerFilter` to your Swagger configuration:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddSwaggerGen(options =>
    {
        options.OperationFilter<JsonMaskedSwaggerFilter>();
    });

    // ...
}
```

### Usage

Apply the `JsonMasked` attribute at the action level:

```csharp
[JsonMasked]
public IActionResult MyAction()
{
    // Your action logic here
}
```

## Documentation

For more detailed information on usage and syntax, please refer to the [JsonMask.NET documentation](https://github.com/sidec15/JsonMask.NET).

## Contributing

Contributions to `JsonMask.NET.ApsNetCore.Mvc.Controller.Filter` are welcome. Please read our [contributing guidelines](CONTRIBUTING.md) for details on submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Based on [JsonMask.NET](https://github.com/sidec15/JsonMask.NET).
- Inspired by [json-mask](https://github.com/nemtsov/json-mask) by nemtsov
