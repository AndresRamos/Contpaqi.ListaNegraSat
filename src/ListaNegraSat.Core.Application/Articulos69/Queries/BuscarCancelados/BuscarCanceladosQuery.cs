using System.Collections.Generic;
using ListaNegraSat.Core.Application.Articulos69.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulos69.Queries.BuscarCancelados;

public class BuscarCanceladosQuery : IRequest<IEnumerable<CanceladoDto>>
{
    public BuscarCanceladosQuery(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
