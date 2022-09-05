using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Contpaqi.Contabilidad.Sql.Contexts;
using ListaNegraSat.Core.Application.Contribuyentes.Interfaces;
using ListaNegraSat.Core.Application.Contribuyentes.Models;
using Microsoft.EntityFrameworkCore;

namespace ListaNegraSat.Infrastructure.ContabilidadContpaqi.Repositories;

public class ContribuyenteContabilidadRepository : IContribuyenteContabilidadRepository
{
    private readonly ContpaqiContabilidadEmpresaDbContext _context;

    public ContribuyenteContabilidadRepository(ContpaqiContabilidadEmpresaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContribuyenteContabilidadDto>> BuscarTodosAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var personas = new List<ContribuyenteContabilidadDto>();

        personas.AddRange(await _context.Personas.Where(p => p.EsProveedor == true)
            .OrderBy(p => p.Nombre)
            .Select(p => new ContribuyenteContabilidadDto
            {
                Codigo = p.Codigo,
                RazonSocial = p.Nombre,
                Rfc = p.RFC,
                Tipo = p.EsCliente == true && p.EsProveedor == true ? ContribuyenteContabilidadTipoEnum.ClienteProveedor :
                    p.EsCliente == true ? ContribuyenteContabilidadTipoEnum.Cliente : ContribuyenteContabilidadTipoEnum.Proveedor
            })
            .ToListAsync(cancellationToken));

        return personas;
    }
}
