﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;
using ListaNegraSat.Presentation.WpfApp.Models;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Actualizaciones;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Articulo69B;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Configuracion;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Contabilidad;
using MahApps.Metro.Controls.Dialogs;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels
{
    public sealed class ShellViewModel : Conductor<Screen>
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IWindowManager _windowManager;

        public ShellViewModel(ConfiguracionAplicacion configuracionAplicacion, IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
        {
            ConfiguracionAplicacion = configuracionAplicacion;
            _dialogCoordinator = dialogCoordinator;
            _windowManager = windowManager;
            DisplayName = "AR Software - Lista Negra SAT";
        }

        public ConfiguracionAplicacion ConfiguracionAplicacion { get; }

        public async Task VerArticulo69BListadoCompletoViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<Articulo69BListadoCompletoViewModel>();
                ActivateItem(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerContribuyentesContabilidadViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<ContribuyentesContabilidadViewModel>();
                ActivateItem(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task VerEditarConfiguracionAplicacionViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<EditarConfiguracionAplicacionViewModel>();
                viewModel.Inicializar();
                _windowManager.ShowDialog(viewModel);
                ConfiguracionAplicacion.CargarConfiguracion();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        public async Task IniciarSoporteRemotoAsync()
        {
            try
            {
                Process.Start(@"https://get.teamviewer.com/ar_software_quick_support");
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.Message);
            }
        }

        public async Task BuscarActualizacionesAsync()
        {
            try
            {
                var viewModel = IoC.Get<ActualizacionAplicacionViewModel>();
                await viewModel.ChecarActualizacionDisponibleAsync();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
            }
        }

        public async Task VerAcercaDeViewAsync()
        {
            try
            {
                var viewModel = IoC.Get<AcercaDeViewModel>();
                _windowManager.ShowDialog(viewModel);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ConfiguracionAplicacion.CargarConfiguracion();
            var viewModel = IoC.Get<ActualizacionAplicacionViewModel>();
            await viewModel.ChecarActualizacionDisponibleAsync();
            if (viewModel.ActualizacionAplicacion.ActualizacionDisponible)
            {
                _windowManager.ShowWindow(viewModel);
            }
        }
    }
}