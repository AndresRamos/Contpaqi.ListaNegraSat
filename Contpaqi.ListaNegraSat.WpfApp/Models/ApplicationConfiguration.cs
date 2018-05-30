using Contpaqi.Sdk.Extras.Interfaces;
using Contpaqi.Sdk.Extras.Modelos;
using Contpaqi.Sdk.Extras.Repositorios;
using GalaSoft.MvvmLight;
using SDKCONTPAQNGLib;

namespace Contpaqi.ListaNegraSat.WpfApp.Models
{
    public class ApplicationConfiguration : ViewModelBase
    {
        private IContpaqiSdk _contpaqiSdk;
        private ContpaqiSdkUnidadTrabajo _contpaqiSdkUnidadTrabajo;
        private Empresa _empresa;
        private bool _empresaAbierta;
        private bool _sdkInicializado;
        private SistemaContpaqEnum _sistemaElegido;

        public IContpaqiSdk ContpaqiSdk
        {
            get => _contpaqiSdk;
            set => Set(() => ContpaqiSdk, ref _contpaqiSdk, value);
        }

        public ContpaqiSdkUnidadTrabajo ContpaqiSdkUnidadTrabajo
        {
            get => _contpaqiSdkUnidadTrabajo;
            set => Set(() => ContpaqiSdkUnidadTrabajo, ref _contpaqiSdkUnidadTrabajo, value);
        }

        public Empresa Empresa
        {
            get => _empresa;
            set => Set(() => Empresa, ref _empresa, value);
        }

        public bool SdkInicializado
        {
            get => _sdkInicializado;
            set => Set(() => SdkInicializado, ref _sdkInicializado, value);
        }

        public bool EmpresaAbierta
        {
            get => _empresaAbierta;
            set => Set(() => EmpresaAbierta, ref _empresaAbierta, value);
        }

        public SistemaContpaqEnum SistemaElegido
        {
            get => _sistemaElegido;
            set => Set(() => SistemaElegido, ref _sistemaElegido, value);
        }

        private TSdkSesion _sdkSesion;

        public TSdkSesion SdkSesion
        {
            get => _sdkSesion;
            set => Set(() => SdkSesion, ref _sdkSesion, value);
        }
    }
}