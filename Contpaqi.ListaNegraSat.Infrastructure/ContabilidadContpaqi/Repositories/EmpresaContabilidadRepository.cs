using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using ARSoftware.Contpaqi.Contabilidad.Sql.Factories;
using ListaNegraSat.Core.Application.Empresas.Interfaces;
using ListaNegraSat.Core.Application.Empresas.Models;
using Microsoft.EntityFrameworkCore;

namespace ListaNegraSat.Infrastructure.ContabilidadContpaqi.Repositories;

public class EmpresaContabilidadRepository : IEmpresaContabilidadRepository
{
    private readonly ContpaqiContabilidadGeneralesDbContext _contabilidadGeneralesDbContext;

    public EmpresaContabilidadRepository(ContpaqiContabilidadGeneralesDbContext contabilidadGeneralesDbContext)
    {
        _contabilidadGeneralesDbContext = contabilidadGeneralesDbContext;
    }

    public async Task<IEnumerable<EmpresaContabilidadDto>> BuscarTodasAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<EmpresaContabilidadDto> empresas = await _contabilidadGeneralesDbContext.ListaEmpresas.OrderBy(e => e.Nombre)
            .Select(e => new EmpresaContabilidadDto { Id = e.Id, Nombre = e.Nombre, BaseDatos = e.AliasBDD })
            .ToListAsync(cancellationToken);

        foreach (EmpresaContabilidadDto empresa in empresas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string empresaConnectionString =
                ContpaqiContabilidadSqlConnectionStringFactory.CreateContpaqiContabilidadEmpresaConnectionString(
                    _contabilidadGeneralesDbContext.Database.GetConnectionString(), empresa.BaseDatos);

            DbContextOptions<ContpaqiContabilidadEmpresaDbContext> empresaOptions =
                new DbContextOptionsBuilder<ContpaqiContabilidadEmpresaDbContext>().UseSqlServer(empresaConnectionString).Options;

            using (var contabilidadEmpresaDbContext = new ContpaqiContabilidadEmpresaDbContext(empresaOptions))
            {
                if (!await contabilidadEmpresaDbContext.Database.CanConnectAsync(cancellationToken)) continue;

                var parametros = await contabilidadEmpresaDbContext.Parametros.AsNoTracking()
                    .Select(p => new { p.RFC, p.GuidDSL })
                    .FirstAsync(cancellationToken);
                empresa.Rfc = parametros.RFC;
                empresa.GuidCompany = parametros.GuidDSL;
            }
        }

        return empresas;
    }
}
