using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Empresas.Interfaces;
using ListaNegraSat.Core.Application.Empresas.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Empresas.Queries.BuscarEmpresasContabilidad
{
    public class BuscarEmpresasContabilidadQueryHandler : IRequestHandler<BuscarEmpresasContabilidadQuery, IEnumerable<EmpresaContabilidadDto>>
    {
        private readonly IEmpresaContabilidadRepository _empresaContabilidadRepository;

        public BuscarEmpresasContabilidadQueryHandler(IEmpresaContabilidadRepository empresaContabilidadRepository)
        {
            _empresaContabilidadRepository = empresaContabilidadRepository;
        }

        public Task<IEnumerable<EmpresaContabilidadDto>> Handle(BuscarEmpresasContabilidadQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _empresaContabilidadRepository.BuscarTodasAsync(cancellationToken);
        }
    }
}