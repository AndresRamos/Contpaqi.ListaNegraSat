using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSoftware.Contpaqi.Add.Sql.Contexts;
using ListaNegraSat.Core.Application.Cfdis.Interfaces;
using ListaNegraSat.Core.Application.Cfdis.Models;
using Microsoft.EntityFrameworkCore;

namespace ListaNegraSat.Infrastructure.AddContpaqi.Repositories;

public class ComprobanteAddRepository : IComprobanteAddRepository
{
    private readonly ContpaqiAddDocumentMetadataDbContext _context;

    public ComprobanteAddRepository(ContpaqiAddDocumentMetadataDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ComprobanteAddDto>> BuscarPorRfcAsync(string rfc, CancellationToken cancellationToken)
    {
        return await _context.Comprobante.Where(r => r.RFCEmisor == rfc)
            .Select(c => new ComprobanteAddDto
            {
                GuidDocument = c.GuidDocument,
                Fecha = c.Fecha,
                Serie = c.Serie,
                Folio = c.Folio,
                RfcEmisor = c.RFCEmisor,
                NombreEmisor = c.NombreEmisor,
                Moneda = c.MonedaDesc,
                TipoCambio = c.TipoCambio,
                Total = c.Total,
                Uuid = c.UUID
            })
            .ToListAsync(cancellationToken);
    }
}
