using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Caliburn.Micro;
using ListaNegraSat.Presentation.WpfApp.Models;
using ListaNegraSat.Presentation.WpfApp.Properties;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Configuracion
{
    public sealed class EditarConfiguracionAplicacionViewModel : Screen
    {
        private readonly ConfiguracionAplicacion _configuracionAplicacion;
        private readonly IDialogCoordinator _dialogCoordinator;
        private string _contpaqiAddConnetionString;
        private string _contpaqiContabilidadConnectionString;
        private string _rutaArchivoListadoCompleto;

        public EditarConfiguracionAplicacionViewModel(ConfiguracionAplicacion configuracionAplicacion, IDialogCoordinator dialogCoordinator)
        {
            _configuracionAplicacion = configuracionAplicacion;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Editar Configuracion De Aplicacion";
        }

        public string ContpaqiContabilidadConnectionString
        {
            get => _contpaqiContabilidadConnectionString;
            set
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
            set
            {
                if (value == _contpaqiAddConnetionString)
                {
                    return;
                }

                _contpaqiAddConnetionString = value;
                NotifyOfPropertyChange(() => ContpaqiAddConnetionString);
            }
        }

        public string RutaArchivoListadoCompleto
        {
            get => _rutaArchivoListadoCompleto;
            set
            {
                if (value == _rutaArchivoListadoCompleto)
                {
                    return;
                }

                _rutaArchivoListadoCompleto = value;
                NotifyOfPropertyChange(() => RutaArchivoListadoCompleto);
            }
        }

        public void Inicializar()
        {
            ContpaqiContabilidadConnectionString = Settings.Default.ContpaqiContabilidadConnectionString;
            ContpaqiAddConnetionString = Settings.Default.ContpaqiAddConnetionString;
            RutaArchivoListadoCompleto = Settings.Default.RutaArchivoListadoCompleto;
        }

        public async Task GuardarConfiguracionAsync()
        {
            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Guardando Configuracion", "Guardando configuracion.");
            progressDialogController.SetIndeterminate();
            await Task.Delay(1000);

            try
            {
                Settings.Default.ContpaqiContabilidadConnectionString = ContpaqiContabilidadConnectionString;
                Settings.Default.ContpaqiAddConnetionString = ContpaqiAddConnetionString;
                Settings.Default.RutaArchivoListadoCompleto = RutaArchivoListadoCompleto;
                Settings.Default.Save();
                _configuracionAplicacion.CargarConfiguracion();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                await progressDialogController.CloseAsync();
            }
        }

        public void Cancelar()
        {
            TryClose();
        }

        public async Task BuscarArchivoListadoCompletoAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog {Filter = "CSV | *.csv"};
                if (openFileDialog.ShowDialog() == true)
                {
                    RutaArchivoListadoCompleto = openFileDialog.FileName;
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task ProbarConnectionStringAsync(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await _dialogCoordinator.ShowMessageAsync(this, "Conexion Exitosa", "La conexion se abrio exitosamente.");
                }
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }
    }
}