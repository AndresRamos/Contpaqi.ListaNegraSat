﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contpaqi.ListaNegraSat.WpfApp.Models
{
    public class ClienteContpaq
    {
        public string Codigo { get; set; }
        public string Rfc { get; set; }

        public string RazonSocial { get; set; }

        public int Tipo { get; set; }

        public int Estatus { get; set; }

        public int Id { get; set; }
    }
}