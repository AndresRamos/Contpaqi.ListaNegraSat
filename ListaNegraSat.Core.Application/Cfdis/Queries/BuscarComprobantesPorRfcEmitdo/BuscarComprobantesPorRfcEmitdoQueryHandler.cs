using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Cfdis.Interfaces;
using ListaNegraSat.Core.Application.Cfdis.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Cfdis.Queries.BuscarComprobantesPorRfcEmitdo;

public class BuscarComprobantesPorRfcEmitdoQueryHandler : IRequestHandler<BuscarComprobantesPorRfcEmitdoQuery,
    IEnumerable<ComprobanteAddDto>>
{
    private readonly IComprobanteAddRepository _comprobanteAddRepository;

    public BuscarComprobantesPorRfcEmitdoQueryHandler(IComprobanteAddRepository comprobanteAddRepository)
    {
        _comprobanteAddRepository = comprobanteAddRepository;
    }

    public async Task<IEnumerable<ComprobanteAddDto>> Handle(BuscarComprobantesPorRfcEmitdoQuery request,
                                                             CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _comprobanteAddRepository.BuscarPorRfcAsync(request.RfcEmisor, cancellationToken);
    }
}
