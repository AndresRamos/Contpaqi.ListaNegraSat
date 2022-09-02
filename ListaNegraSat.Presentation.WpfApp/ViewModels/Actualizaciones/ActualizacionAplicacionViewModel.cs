using System.Threading.Tasks;
using Caliburn.Micro;
using ListaNegraSat.Presentation.WpfApp.Models;
using Microsoft.Win32;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Actualizaciones
{
    public sealed class ActualizacionAplicacionViewModel : Screen
    {
        private ActualizacionAplicacion _actualizacionAplicacion = new ActualizacionAplicacion();
        private bool _isDescargando;

        public ActualizacionAplicacionViewModel()
        {
            DisplayName = "Actualizacion De Aplicacion";
        }

        public ActualizacionAplicacion ActualizacionAplicacion
        {
            get => _actualizacionAplicacion;
            set
            {
                if (Equals(value, _actualizacionAplicacion))
                {
                    return;
                }

                _actualizacionAplicacion = value;
                NotifyOfPropertyChange(() => ActualizacionAplicacion);
            }
        }

        public bool CanDescargarActualizacionAsync => !IsProcesando && ActualizacionAplicacion.ActualizacionDisponible;

        public bool IsProcesando => IsDescargando;

        public bool IsDescargando
        {
            get => _isDescargando;
            private set
            {
                if (value == _isDescargando)
                {
                    return;
                }

                _isDescargando = value;
                NotifyOfPropertyChange(() => IsDescargando);
                RaiseGuards();
            }
        }

        public async Task ChecarActualizacionDisponibleAsync()
        {
            await ActualizacionAplicacion.ChecarActualizacionDisponibleAsync();
            RaiseGuards();
        }

        public async Task DescargarActualizacionAsync()
        {
            IsDescargando = true;
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "zip | (*.zip)", FileName = $"Instalador_Trevino_Devolucion_Iva_{ActualizacionAplicacion.VersionNueva}.zip"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                await ActualizacionAplicacion.DescargarActualizacionAsync(saveFileDialog.FileName);
            }

            IsDescargando = false;
            await TryCloseAsync();
        }

        public async Task CerrarVistaAsync()
        {
            await TryCloseAsync();
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => IsProcesando);
            NotifyOfPropertyChange(() => CanDescargarActualizacionAsync);
        }
    }
}
