using GalaSoft.MvvmLight;

namespace Contpaqi.ListaNegraSat.WpfApp.Models
{
    public class ContribuyenteContpaq : ViewModelBase
    {
        private string _codigo;
        private int _estatus;
        private int _id;
        private string _razonSocial;
        private string _rfc;
        private string _supuesto;
        private int _tipo;
        private string _tipoPersona;

        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }

        public string Codigo
        {
            get => _codigo;
            set => Set(() => Codigo, ref _codigo, value);
        }

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

        public int Tipo
        {
            get => _tipo;
            set => Set(() => Tipo, ref _tipo, value);
        }

        public int Estatus
        {
            get => _estatus;
            set => Set(() => Estatus, ref _estatus, value);
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