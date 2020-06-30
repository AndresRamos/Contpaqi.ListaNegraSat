using CsvHelper.Configuration;

namespace ListaNegraSat.Core.Application.Articulos69.Models
{
    public sealed class CanceladoDtoCsvMap : ClassMap<CanceladoDto>
    {
        public CanceladoDtoCsvMap()
        {
            Map(m => m.Rfc).Index(0);
            Map(m => m.RazonSocial).Index(1);
            Map(m => m.TipoPersona).Index(2);
            Map(m => m.Supuesto).Index(3);
            Map(m => m.FechaPrimeraPublicacion).Index(4);
            Map(m => m.Monto).Index(5);
            Map(m => m.FechaPublicacion).Index(6);
            Map(m => m.Entidad).Index(7);
        }
    }
}