using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contpaqi.Sql.Contabilidad.Empresa;
using Contpaqi.Sql.Contabilidad.Generales;
using ListaNegraSat.Core.Application.Empresas.Interfaces;
using ListaNegraSat.Core.Application.Empresas.Models;

namespace ListaNegraSat.Infrastructure.ContpaqiContabilidad.Repositories
{
    public class EmpresaContabilidadRepository : IEmpresaContabilidadRepository
    {
        private readonly ContabilidadGeneralesDbContext _contabilidadGeneralesDbContext;

        public EmpresaContabilidadRepository(ContabilidadGeneralesDbContext contabilidadGeneralesDbContext)
        {
            _contabilidadGeneralesDbContext = contabilidadGeneralesDbContext;
        }

        public async Task<IEnumerable<EmpresaContabilidadDto>> BuscarTodasAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var empresas = await _contabilidadGeneralesDbContext.ListaEmpresas.OrderBy(e => e.Nombre).Select(e => new EmpresaContabilidadDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                BaseDatos = e.AliasBDD
            }).ToListAsync(cancellationToken);

            foreach (var empresa in empresas)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var builder = new SqlConnectionStringBuilder(_contabilidadGeneralesDbContext.Database.Connection.ConnectionString) {InitialCatalog = empresa.BaseDatos};

                using (var contabilidadEmpresaDbContext = new ContabilidadEmpresaDbContext(new SqlConnection(builder.ConnectionString), true))
                {
                    if (!contabilidadEmpresaDbContext.Database.Exists())
                    {
                        continue;
                    }

                    var parametros = await contabilidadEmpresaDbContext.Parametros.AsNoTracking().Select(p => new {p.RFC, p.GuidDSL}).FirstAsync(cancellationToken);
                    empresa.Rfc = parametros.RFC;
                    empresa.GuidCompany = parametros.GuidDSL;
                }
            }

            return empresas;
        }
    }
}