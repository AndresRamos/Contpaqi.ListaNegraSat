using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Cfdis.Models;

namespace ListaNegraSat.Core.Application.Cfdis.Interfaces;

public interface IComprobanteAddRepository
{
    Task<IEnumerable<ComprobanteAddDto>> BuscarPorRfcAsync(string rfc, CancellationToken cancellationToken);
}
