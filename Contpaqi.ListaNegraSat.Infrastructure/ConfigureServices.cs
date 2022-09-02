using ARSoftware.Contpaqi.Add.Sql.Contexts;
using ARSoftware.Contpaqi.Add.Sql.Factories;
using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using ARSoftware.Contpaqi.Contabilidad.Sql.Factories;
using Contpaqi.ListaNegraSat.Infrastructure.AddContpaqi.Repositories;
using Contpaqi.ListaNegraSat.Infrastructure.ContabilidadContpaqi.Repositories;
using ListaNegraSat.Core.Application.Cfdis.Interfaces;
using ListaNegraSat.Core.Application.Common;
using ListaNegraSat.Core.Application.Contribuyentes.Interfaces;
using ListaNegraSat.Core.Application.Empresas.Interfaces;
using ListaNegraSat.Core.Application.Expedientes.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contpaqi.ListaNegraSat.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddContpaqiContabilidadServices();
        serviceCollection.AddContpaqiAddServices();
        return serviceCollection;
    }

    private static void AddContpaqiContabilidadServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ContpaqiContabilidadGeneralesDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadGeneralesConnectionString(
                configuracionAplicacion.ContpaqiContabilidadConnectionString);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddDbContext<ContpaqiContabilidadEmpresaDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadEmpresaConnectionString(
                configuracionAplicacion.ContpaqiContabilidadConnectionString,
                configuracionAplicacion.EmpresaContabilidad.BaseDatos);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddTransient<IEmpresaContabilidadRepository, EmpresaContabilidadRepository>();
        serviceCollection.AddTransient<IContribuyenteContabilidadRepository, ContribuyenteContabilidadRepository>();
    }

    private static void AddContpaqiAddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<ContpaqiAddDocumentMetadataDbContext>((provider, builder) =>
        {
            var configuracionAplicacion = provider.GetRequiredService<ConfiguracionAplicacion>();
            string connectionString = ContpaqiAddSqlConnectionStringFactory.CreateContpaqiAddDocumentMetadataConnectionString(
                configuracionAplicacion.ContpaqiAddConnetionString,
                configuracionAplicacion.EmpresaContabilidad.GuidCompany);
            builder.UseSqlServer(connectionString);
        });

        serviceCollection.AddTransient<IComprobanteAddRepository, ComprobanteAddRepository>();
        serviceCollection.AddTransient<IExpedienteAddRepository, ExpedienteAddRepository>();
    }
}
