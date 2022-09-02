using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ListaNegraSat.Core.Application.Empresas.Models;

namespace ListaNegraSat.Presentation.WpfApp.Models
{
    public sealed class ConfiguracionAplicacion : INotifyPropertyChanged
    {
        private static readonly string RutaArchivoListadoCompleto =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(),
                "Listado_Completo_69-B.csv");

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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CargarConfiguracion()
        {
            // todo cargar configuracion
            //ContpaqiContabilidadConnectionString = Settings.Default.ContpaqiContabilidadConnectionString;
            //ContpaqiAddConnetionString = Settings.Default.ContpaqiAddConnetionString;
        }

        public void SetEmpresaContabilidad(EmpresaContabilidadDto empresaContabilidad)
        {
            EmpresaContabilidad = empresaContabilidad;
        }

        public string GetRutaArchivoListadoCompleto()
        {
            // todo cargar ruta
            //var ruta = Settings.Default.RutaArchivoListadoCompleto;

            //if (!string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta))
            //{
            //    return ruta;
            //}

            return RutaArchivoListadoCompleto;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
