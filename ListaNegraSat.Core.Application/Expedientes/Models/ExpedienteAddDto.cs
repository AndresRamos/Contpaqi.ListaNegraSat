using System;

namespace ListaNegraSat.Core.Application.Expedientes.Models
{
    public class ExpedienteAddDto
    {
        public Guid GuidRelacionado { get; set; }

        public Guid? GuidPertenece { get; set; }

        public string ApplicationTypeExp { get; set; }

        public string TypeExp { get; set; }

        public string CommentExp { get; set; }
    }
}