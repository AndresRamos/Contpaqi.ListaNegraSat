using System.Collections.Generic;
using System.Linq;

namespace ListaNegraSat.Core.Application.Articulo69B.Models
{
    public class Articulo69BListadoCompleto
    {
        public string Version { get; set; }
        public IEnumerable<Contribuyente69BDto> Contribuyentes { get; set; }

        public int DefinitivosTotal => Contribuyentes.Count(c => c.Situacion == SituacionEnumeration.Definitivos.Name);
        public int DesvirtuadosTotal => Contribuyentes.Count(c => c.Situacion == SituacionEnumeration.Desvirtuados.Name);
        public int PresuntosTotal => Contribuyentes.Count(c => c.Situacion == SituacionEnumeration.Presuntos.Name);
        public int SentenciasFavorablesTotal => Contribuyentes.Count(c => c.Situacion == SituacionEnumeration.SentenciaFavorable.Name);
    }
}