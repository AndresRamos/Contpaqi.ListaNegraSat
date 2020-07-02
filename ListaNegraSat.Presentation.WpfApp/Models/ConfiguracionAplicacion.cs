using System;
using System.IO;
using System.Reflection;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Empresas.Models;
using ListaNegraSat.Presentation.WpfApp.Properties;

namespace ListaNegraSat.Presentation.WpfApp.Models
{
    public class ConfiguracionAplicacion : PropertyChangedBase
    {
        private static readonly string RutaArchivoListadoCompleto = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "Listado_Completo_69-B.csv");
        private string _contpaqiAddConnetionString;
        private string _contpaqiContabilidadConnectionString;
        private EmpresaContabilidadDto _empresaContabilidad;

        public string ContpaqiContabilidadConnectionString
        {
            get => _contpaqiContabilidadConnectionString;
            private set
            {
                if (value == _contpaqiContabilidadConnectionString)
                {
                    return;
                }

                _contpaqiContabilidadConnectionString = value;
                NotifyOfPropertyChange(() => ContpaqiContabilidadConnectionString);
            }
        }

        public string ContpaqiAddConnetionString
        {
            get => _contpaqiAddConnetionString;
            private set
            {
                if (value == _contpaqiAddConnetionString)
                {
                    return;
                }

                _contpaqiAddConnetionString = value;
                NotifyOfPropertyChange(() => ContpaqiAddConnetionString);
            }
        }

        public EmpresaContabilidadDto EmpresaContabilidad
        {
            get => _empresaContabilidad;
            private set
            {
                if (Equals(value, _empresaContabilidad))
                {
                    return;
                }

                _empresaContabilidad = value;
                NotifyOfPropertyChange(() => EmpresaContabilidad);
            }
        }

        public void CargarConfiguracion()
        {
            ContpaqiContabilidadConnectionString = Settings.Default.ContpaqiContabilidadConnectionString;
            ContpaqiAddConnetionString = Settings.Default.ContpaqiAddConnetionString;
        }

        public void SetEmpresaContabilidad(EmpresaContabilidadDto empresaContabilidad)
        {
            EmpresaContabilidad = empresaContabilidad;
        }

        public string GetRutaArchivoListadoCompleto()
        {
            var ruta = Settings.Default.RutaArchivoListadoCompleto;

            if (!string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta))
            {
                return ruta;
            }

            return RutaArchivoListadoCompleto;
        }
    }
}