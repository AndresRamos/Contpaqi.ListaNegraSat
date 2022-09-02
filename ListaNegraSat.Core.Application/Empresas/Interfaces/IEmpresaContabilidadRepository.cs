using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Empresas.Models;

namespace ListaNegraSat.Core.Application.Empresas.Interfaces;

public interface IEmpresaContabilidadRepository
{
    Task<IEnumerable<EmpresaContabilidadDto>> BuscarTodasAsync(CancellationToken cancellationToken);
}
