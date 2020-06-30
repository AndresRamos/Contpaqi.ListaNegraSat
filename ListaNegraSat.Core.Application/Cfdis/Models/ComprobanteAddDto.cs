using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListaNegraSat.Core.Application.Cfdis.Models
{
    public class ComprobanteAddDto
    {
        public Guid GuidDocument { get; set; }
        public DateTime? Fecha { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string RfcEmisor { get; set; }
        public string NombreEmisor { get; set; }
        public string Moneda { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? Total { get; set; }
        public Guid? Uuid { get; set; }
    }
}
