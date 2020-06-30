using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Expedientes.Interfaces;
using ListaNegraSat.Core.Application.Expedientes.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Expedientes.Queries.BuscarExpedientesPorGuidPerteneciente
{
    public class BuscarExpedientesPorGuidPertenecienteQueryHandler : IRequestHandler<BuscarExpedientesPorGuidPertenecienteQuery, IEnumerable<ExpedienteAddDto>>
    {
        private readonly IExpedienteAddRepository _expedienteAddRepository;

        public BuscarExpedientesPorGuidPertenecienteQueryHandler(IExpedienteAddRepository expedienteAddRepository)
        {
            _expedienteAddRepository = expedienteAddRepository;
        }

        public async Task<IEnumerable<ExpedienteAddDto>> Handle(BuscarExpedientesPorGuidPertenecienteQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _expedienteAddRepository.BuscarExpedientesPorGuidPertenecienteAsync(request.GuidPertenece, cancellationToken);
        }
    }
}