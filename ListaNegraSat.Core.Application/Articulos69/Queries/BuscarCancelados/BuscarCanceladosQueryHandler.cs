using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using ListaNegraSat.Core.Application.Articulos69.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulos69.Queries.BuscarCancelados;

public class BuscarCanceladosQueryHandler : IRequestHandler<BuscarCanceladosQuery, IEnumerable<CanceladoDto>>
{
    public Task<IEnumerable<CanceladoDto>> Handle(BuscarCanceladosQuery request, CancellationToken cancellationToken)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };

        using (var reader = new StreamReader(request.FileName))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<CanceladoDtoCsvMap>();

            IEnumerable<CanceladoDto> records = csv.GetRecords<CanceladoDto>();
            return Task.FromResult(records);
        }
    }
}
