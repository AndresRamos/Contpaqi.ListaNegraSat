using System.Collections.Generic;
using ListaNegraSat.Core.Application.Empresas.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Empresas.Queries.BuscarEmpresasContabilidad;

public class BuscarEmpresasContabilidadQuery : IRequest<IEnumerable<EmpresaContabilidadDto>>
{
}
