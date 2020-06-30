using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contpaqi.Sql.ADD.OtherMetadata;
using ListaNegraSat.Core.Application.Expedientes.Interfaces;
using ListaNegraSat.Core.Application.Expedientes.Models;

namespace ListaNegraSat.Infrastructure.ContpaqiAdd.Repositories
{
    public class ExpedienteAddRepository : IExpedienteAddRepository
    {
        private readonly AddOtherMetadataDbContext _context;

        public ExpedienteAddRepository(AddOtherMetadataDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpedienteAddDto>> BuscarExpedientesPorGuidPertenecienteAsync(Guid guidPertenece, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _context.Expedientes.Where(e => e.Guid_Pertenece == guidPertenece).Select(e => new ExpedienteAddDto
            {
                GuidRelacionado = e.Guid_Relacionado,
                GuidPertenece = e.Guid_Pertenece,
                ApplicationTypeExp = e.ApplicationType_Exp,
                TypeExp = e.Type_Exp,
                CommentExp = e.Comment_Exp
            }).ToListAsync(cancellationToken);
        }
    }
}