using System;
using System.Collections.Generic;
using ListaNegraSat.Core.Application.Expedientes.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Expedientes.Queries.BuscarExpedientesPorGuidPerteneciente
{
    public class BuscarExpedientesPorGuidPertenecienteQuery : IRequest<IEnumerable<ExpedienteAddDto>>
    {
        public BuscarExpedientesPorGuidPertenecienteQuery(Guid guidPertenece)
        {
            GuidPertenece = guidPertenece;
        }

        public Guid GuidPertenece { get; }
    }
}