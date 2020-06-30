using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contpaqi.Sql.Contabilidad.Empresa;
using ListaNegraSat.Core.Application.Contribuyentes.Interfaces;
using ListaNegraSat.Core.Application.Contribuyentes.Models;

namespace ListaNegraSat.Infrastructure.ContpaqiContabilidad.Repositories
{
    public class ContribuyenteContabilidadRepository : IContribuyenteContabilidadRepository
    {
        private readonly ContabilidadEmpresaDbContext _context;

        public ContribuyenteContabilidadRepository(ContabilidadEmpresaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContribuyenteContabilidadDto>> BuscarTodosAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var personas = new List<ContribuyenteContabilidadDto>();

            personas.AddRange(await _context.Personas.Where(p => p.EsProveedor == true).OrderBy(p => p.Nombre).Select(p => new ContribuyenteContabilidadDto
            {
                Codigo = p.Codigo,
                RazonSocial = p.Nombre,
                Rfc = p.RFC,
                Tipo = p.EsCliente == true && p.EsProveedor == true ? ContribuyenteContabilidadTipoEnum.ClienteProveedor : p.EsCliente == true ? ContribuyenteContabilidadTipoEnum.Cliente : ContribuyenteContabilidadTipoEnum.Proveedor
            }).ToListAsync(cancellationToken));

            return personas;
        }
    }
}