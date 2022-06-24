using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Blazug;

public static class BlazugExtensions
{
    public static IServiceCollection AddBlazug(this IServiceCollection services)
    {
        services.AddSingleton(typeof(Controls));
        
        return services;
    }
}
