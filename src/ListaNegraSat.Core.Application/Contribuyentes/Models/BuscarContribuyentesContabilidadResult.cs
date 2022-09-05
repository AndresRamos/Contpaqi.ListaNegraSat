using System.Collections.Generic;
using System.Linq;
using ListaNegraSat.Core.Application.Articulo69B.Models;

namespace ListaNegraSat.Core.Application.Contribuyentes.Models;

public class BuscarContribuyentesContabilidadResult
{
    public string Version { get; set; }
    public IEnumerable<ContribuyenteContabilidadDto> Contribuyentes { get; set; }

    public int DefinitivosTotal => Contribuyentes.Count(c => c.ListadoSituacion == SituacionEnumeration.Definitivos.Name);
    public int DesvirtuadosTotal => Contribuyentes.Count(c => c.ListadoSituacion == SituacionEnumeration.Desvirtuados.Name);
    public int PresuntosTotal => Contribuyentes.Count(c => c.ListadoSituacion == SituacionEnumeration.Presuntos.Name);
    public int SentenciasFavorablesTotal => Contribuyentes.Count(c => c.ListadoSituacion == SituacionEnumeration.SentenciaFavorable.Name);
}
