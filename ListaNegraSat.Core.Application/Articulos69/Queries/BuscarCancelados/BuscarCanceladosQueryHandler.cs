using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ListaNegraSat.Core.Application.Articulos69.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulos69.Queries.BuscarCancelados
{
    public class BuscarCanceladosQueryHandler : IRequestHandler<BuscarCanceladosQuery, IEnumerable<CanceladoDto>>
    {
        public Task<IEnumerable<CanceladoDto>> Handle(BuscarCanceladosQuery request, CancellationToken cancellationToken)
        {
            using (var reader = new StreamReader(request.FileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<CanceladoDtoCsvMap>();

                var records = csv.GetRecords<CanceladoDto>();
                return Task.FromResult(records);
            }
        }
    }
}