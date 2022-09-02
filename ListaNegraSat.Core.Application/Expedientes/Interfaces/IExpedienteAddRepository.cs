using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Expedientes.Models;

namespace ListaNegraSat.Core.Application.Expedientes.Interfaces;

public interface IExpedienteAddRepository
{
    Task<IEnumerable<ExpedienteAddDto>> BuscarExpedientesPorGuidPertenecienteAsync(Guid guidPertenece, CancellationToken cancellationToken);
}
