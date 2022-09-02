using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Contribuyentes.Models;

namespace ListaNegraSat.Core.Application.Contribuyentes.Interfaces;

public interface IContribuyenteContabilidadRepository
{
    Task<IEnumerable<ContribuyenteContabilidadDto>> BuscarTodosAsync(CancellationToken cancellationToken);
}
