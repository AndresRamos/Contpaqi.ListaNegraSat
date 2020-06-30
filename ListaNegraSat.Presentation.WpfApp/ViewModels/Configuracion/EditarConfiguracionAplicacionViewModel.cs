using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using ListaNegraSat.Presentation.WpfApp.Models;
using ListaNegraSat.Presentation.WpfApp.Properties;
using MahApps.Metro.Controls.Dialogs;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Configuracion
{
    public sealed class EditarConfiguracionAplicacionViewModel : Screen
    {
        private readonly ConfiguracionAplicacion _configuracionAplicacion;
        private readonly IDialogCoordinator _dialogCoordinator;
        private string _contpaqiContabilidadConnectionString;
        private string _contpaqiAddConnetionString;

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
                if (value == _contpaqiAddConnetionString) return;
                _contpaqiAddConnetionString = value;
                NotifyOfPropertyChange(() => ContpaqiAddConnetionString);
            }
        }

        public void Inicializar()
        {
            ContpaqiContabilidadConnectionString = Settings.Default.ContpaqiContabilidadConnectionString;
            ContpaqiAddConnetionString = Settings.Default.ContpaqiAddConnetionString;
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
    }
}