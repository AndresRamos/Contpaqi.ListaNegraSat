namespace ListaNegraSat.Core.Application.Contribuyentes.Models
{
    public class ContribuyenteContabilidadDto
    {
        public string Codigo { get; set; }
        public string RazonSocial { get; set; }
        public string Rfc { get; set; }
        public ContribuyenteContabilidadTipoEnum Tipo { get; set; }
        public string ListadoNumero { get; set; }
        public string ListadoRfc { get; set; }
        public string ListadoNombreContribuyente { get; set; }
        public string ListadoSituacion { get; set; }
        public string ListadoNumeroYFechaDeOficioGlobalDePresuncion { get; set; }
        public string ListadoPublicacionPaginaSatPresuntos { get; set; }
        public string ListadoPublicacionDofPresuntos { get; set; }
        public string ListadoPublicacionPaginaSatDesvirtuados { get; set; }
        public string ListadoNumeroYFechaDeOficioGlobalDeContribuyentesQueDesvirtuaron { get; set; }
        public string ListadoPublicacionDofDesvirtuados { get; set; }
        public string ListadoNumeroYFechaDeOficioGlobalDeDefinitivos { get; set; }
        public string ListadoPublicacionPaginaSatDefinitivos { get; set; }
        public string ListadoPublicacionDofDefinitivos { get; set; }
        public string ListadoNumeroYFechaDeOficioGlobalDeSentenciaFavorable { get; set; }
        public string ListadoPublicacionPaginaSatSentenciaFavorable { get; set; }
        public string ListadoPublicacionDofSentenciaFavorable { get; set; }
    }
}