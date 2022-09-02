using ListaNegraSat.Core.Application.Contribuyentes.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Contribuyentes.Queries.BuscarContribuyentesContabilidad;

public class BuscarContribuyentesContabilidadQuery : IRequest<BuscarContribuyentesContabilidadResult>
{
    public BuscarContribuyentesContabilidadQuery(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
