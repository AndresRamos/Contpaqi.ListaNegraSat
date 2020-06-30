﻿using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ListaNegraSat.Core.Application.Articulo69B.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Articulo69B.Queries.BuscarContribuyentes69B
{
    public class BuscarContribuyentes69BQueryHandler : IRequestHandler<BuscarContribuyentes69BQuery, Articulo69BListadoCompleto>
    {
        public Task<Articulo69BListadoCompleto> Handle(BuscarContribuyentes69BQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var reader = new StreamReader(request.RutaArchivo, Encoding.Default))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<Contribuyente69BDtoMap>();

                csv.Read();
                var version = csv.GetField(0);
                csv.Read();

                //var version = reader.ReadLine();
                //reader.ReadLine();

                var listado = new Articulo69BListadoCompleto();
                listado.Version = version;
                listado.Contribuyentes = csv.GetRecords<Contribuyente69BDto>().ToList();

                return Task.FromResult(listado);
            }
        }
    }
}