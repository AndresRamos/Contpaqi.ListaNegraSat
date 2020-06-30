using System.Collections.Generic;
using System.IO;
using ListaNegraSat.Core.Application.Articulos69.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulos69.Queries.BuscarCancelados
{
    public class BuscarCanceladosQuery : IRequest<IEnumerable<CanceladoDto>>
    {
        public string FileName { get; set; }
    }
}