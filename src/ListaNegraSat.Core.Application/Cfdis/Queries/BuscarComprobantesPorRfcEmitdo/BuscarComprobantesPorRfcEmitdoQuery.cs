using System.Collections.Generic;
using ListaNegraSat.Core.Application.Cfdis.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Cfdis.Queries.BuscarComprobantesPorRfcEmitdo;

public class BuscarComprobantesPorRfcEmitdoQuery : IRequest<IEnumerable<ComprobanteAddDto>>
{
    public BuscarComprobantesPorRfcEmitdoQuery(string rfcEmisor)
    {
        RfcEmisor = rfcEmisor;
    }

    public string RfcEmisor { get; }
}
