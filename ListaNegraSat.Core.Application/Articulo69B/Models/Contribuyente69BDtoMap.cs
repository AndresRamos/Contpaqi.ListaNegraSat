using CsvHelper.Configuration;

namespace ListaNegraSat.Core.Application.Articulo69B.Models
{
    public sealed class Contribuyente69BDtoMap : ClassMap<Contribuyente69BDto>
    {
        public Contribuyente69BDtoMap()
        {
            Map(m => m.Numero).Index(0);
            Map(m => m.Rfc).Index(1);
            Map(m => m.NombreContribuyente).Index(2);
            Map(m => m.Situacion).Index(3);
            Map(m => m.NumeroYFechaDeOficioGlobalDePresuncion).Index(4);
            Map(m => m.PublicacionPaginaSatPresuntos).Index(5);
            Map(m => m.PublicacionDofPresuntos).Index(7);
            Map(m => m.PublicacionPaginaSatDesvirtuados).Index(8);
            Map(m => m.NumeroYFechaDeOficioGlobalDeContribuyentesQueDesvirtuaron).Index(9);
            Map(m => m.PublicacionDofDesvirtuados).Index(10);
            Map(m => m.NumeroYFechaDeOficioGlobalDeDefinitivos).Index(11);
            Map(m => m.PublicacionPaginaSatDefinitivos).Index(12);
            Map(m => m.PublicacionDofDefinitivos).Index(13);
            Map(m => m.NumeroYFechaDeOficioGlobalDeSentenciaFavorable).Index(14);
            Map(m => m.PublicacionPaginaSatSentenciaFavorable).Index(15);
            Map(m => m.PublicacionDofSentenciaFavorable).Index(17);
        }
    }
}