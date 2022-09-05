using ListaNegraSat.Core.Application.Articulo69B.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulo69B.Queries.BuscarContribuyentes69B;

public class BuscarContribuyentes69BQuery : IRequest<Articulo69BListadoCompleto>
{
    public BuscarContribuyentes69BQuery(string rutaArchivo)
    {
        RutaArchivo = rutaArchivo;
    }

    public string RutaArchivo { get; set; }
}
