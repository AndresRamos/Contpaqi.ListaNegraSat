using GalaSoft.MvvmLight;

namespace Contpaqi.ListaNegraSat.WpfApp.Models
{
    public class Contribuyente : ViewModelBase
    {
        private string _razonSocial;
        private string _rfc;
        private string _supuesto;
        private string _tipoPersona;

        public string Rfc
        {
            get => _rfc;
            set => Set(() => Rfc, ref _rfc, value);
        }

        public string RazonSocial
        {
            get => _razonSocial;
            set => Set(() => RazonSocial, ref _razonSocial, value);
        }

        public string TipoPersona
        {
            get => _tipoPersona;
            set => Set(() => TipoPersona, ref _tipoPersona, value);
        }

        public string Supuesto
        {
            get => _supuesto;
            set => Set(() => Supuesto, ref _supuesto, value);
        }
    }
}