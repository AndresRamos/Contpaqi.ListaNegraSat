using System.Reflection;
using ListaNegraSat.Core.Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ListaNegraSat.Core.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddSingleton<ConfiguracionAplicacion>();

        return services;
    }
}
