using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace JsonMask.NET.ApsNetCore.Mvc.Controller.Filter
{
  public static class ServicesExtensions

  {

    public static IServiceCollection AddJsonMask(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
      // Register the MaskerService with the specified lifetime
      services.Add(new ServiceDescriptor(typeof(IMaskerService), typeof(MaskerService), lifetime));

      // Assuming AddControllers has already been called in Startup,
      // we just configure the options to add the filter
      services.Configure<MvcOptions>(options =>
      {
        options.Filters.Add<JsonMaskedAsyncResultFilter>();
      });

      return services;
    }

  }

}
