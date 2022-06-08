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

    public static IServiceProvider UseBlazug(this IServiceProvider serviceProvider,int maxLogs)
    {
        var controls = serviceProvider.GetService(typeof(Controls)) as Controls;

        controls?.Init(maxLogs);

        return serviceProvider;
    }


    public static IServiceProvider UseBlazug(this WebAssemblyHost host, int maxLogs) =>
        host.UseBlazug(maxLogs);

}
