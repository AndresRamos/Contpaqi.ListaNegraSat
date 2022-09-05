using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ListaNegraSat.Core.Application.Articulo69B.Models;
using ListaNegraSat.Core.Application.Articulo69B.Queries.BuscarContribuyentes69B;
using ListaNegraSat.Core.Application.Contribuyentes.Interfaces;
using ListaNegraSat.Core.Application.Contribuyentes.Models;
using MediatR;

namespace ListaNegraSat.Core.Application.Contribuyentes.Queries.BuscarContribuyentesContabilidad;

public class BuscarContribuyentesContabilidadQueryHandler : IRequestHandler<BuscarContribuyentesContabilidadQuery,
    BuscarContribuyentesContabilidadResult>
{
    private readonly IContribuyenteContabilidadRepository _contribuyenteContabilidadRepository;
    private readonly IMediator _mediator;

    public BuscarContribuyentesContabilidadQueryHandler(IMediator mediator,
                                                        IContribuyenteContabilidadRepository contribuyenteContabilidadRepository)
    {
        _mediator = mediator;
        _contribuyenteContabilidadRepository = contribuyenteContabilidadRepository;
    }

    public async Task<BuscarContribuyentesContabilidadResult> Handle(BuscarContribuyentesContabilidadQuery request,
                                                                     CancellationToken cancellationToken)
    {
        Articulo69BListadoCompleto archivo = await _mediator.Send(new BuscarContribuyentes69BQuery(request.FileName), cancellationToken);

        IEnumerable<ContribuyenteContabilidadDto> contribuyentsContabilidad =
            await _contribuyenteContabilidadRepository.BuscarTodosAsync(cancellationToken);

        var contribuyentesContabilidad69B = new List<ContribuyenteContabilidadDto>();

        foreach (ContribuyenteContabilidadDto personaContabilidad in contribuyentsContabilidad)
        {
            Contribuyente69BDto contribuyente69B = archivo.Contribuyentes.FirstOrDefault(c => c.Rfc == personaContabilidad.Rfc);
            if (contribuyente69B == null)
            {
                continue;
            }

            personaContabilidad.ListadoNumero = contribuyente69B.Numero;
            personaContabilidad.ListadoRfc = contribuyente69B.Rfc;
            personaContabilidad.ListadoNombreContribuyente = contribuyente69B.NombreContribuyente;
            personaContabilidad.ListadoSituacion = contribuyente69B.Situacion;
            personaContabilidad.ListadoNumeroYFechaDeOficioGlobalDePresuncion = contribuyente69B.NumeroYFechaDeOficioGlobalDePresuncion;
            personaContabilidad.ListadoPublicacionPaginaSatPresuntos = contribuyente69B.PublicacionPaginaSatPresuntos;
            personaContabilidad.ListadoPublicacionDofPresuntos = contribuyente69B.PublicacionDofPresuntos;
            personaContabilidad.ListadoPublicacionPaginaSatDesvirtuados = contribuyente69B.PublicacionPaginaSatDesvirtuados;
            personaContabilidad.ListadoNumeroYFechaDeOficioGlobalDeContribuyentesQueDesvirtuaron =
                contribuyente69B.NumeroYFechaDeOficioGlobalDeContribuyentesQueDesvirtuaron;
            personaContabilidad.ListadoPublicacionDofDesvirtuados = contribuyente69B.PublicacionDofDesvirtuados;
            personaContabilidad.ListadoNumeroYFechaDeOficioGlobalDeDefinitivos = contribuyente69B.NumeroYFechaDeOficioGlobalDeDefinitivos;
            personaContabilidad.ListadoPublicacionPaginaSatDefinitivos = contribuyente69B.PublicacionPaginaSatDefinitivos;
            personaContabilidad.ListadoPublicacionDofDefinitivos = contribuyente69B.PublicacionDofDefinitivos;
            personaContabilidad.ListadoNumeroYFechaDeOficioGlobalDeSentenciaFavorable =
                contribuyente69B.NumeroYFechaDeOficioGlobalDeSentenciaFavorable;
            personaContabilidad.ListadoPublicacionPaginaSatSentenciaFavorable = contribuyente69B.PublicacionPaginaSatSentenciaFavorable;
            personaContabilidad.ListadoPublicacionDofSentenciaFavorable = contribuyente69B.PublicacionDofSentenciaFavorable;

            contribuyentesContabilidad69B.Add(personaContabilidad);
        }

        var result = new BuscarContribuyentesContabilidadResult
        {
            Version = archivo.Version, Contribuyentes = contribuyentesContabilidad69B
        };

        return result;
    }
}
