using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Contpaqi.ListaNegraSat.WpfApp.Models
{
    public class Contribuyente : ViewModelBase
    {
        private string _rfc;

        public string Rfc
        {
            get => _rfc;
            set => Set(() => Rfc, ref _rfc, value);
        }

        private string _razonSocial;

        public string RazonSocial
        {
            get => _razonSocial;
            set => Set(() => RazonSocial, ref _razonSocial, value);
        }
    }
}
