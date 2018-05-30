using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using Contpaqi.ListaNegraSat.WpfApp.Helpers;
using Contpaqi.ListaNegraSat.WpfApp.Models;
using Contpaqi.Sdk.Extras;
using Contpaqi.Sdk.Extras.Modelos;
using Contpaqi.Sdk.Extras.Repositorios;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using SDKCONTPAQNGLib;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IDialogCoordinator _dialogCoordinator;
        private RelayCommand _abrirAcercaDeCommand;
        private RelayCommand _abrirEmpresaCommand;
        private RelayCommand _abrirManualDeUsuarioCommand;
        private RelayCommand _buscarActualizacionCommand;
        private RelayCommand _cerrarEmpresaCommand;
        private ViewModelBase _currentMainView;
        private RelayCommand _iniciarSdkCommand;
        private RelayCommand _terminarSdkCommand;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator, ApplicationConfiguration applicationConfiguration)
        {
            _dialogCoordinator = dialogCoordinator;
            _applicationConfiguration = applicationConfiguration;
            RegisterMessages();
            CurrentMainView = SimpleIoc.Default.GetInstance<ContribuyentesIncumplidosViewModel>();
        }

        public ViewModelBase CurrentMainView
        {
            get => _currentMainView;
            set => Set(() => CurrentMainView, ref _currentMainView, value);
        }

        public RelayCommand IniciarSdkCommand
        {
            get
            {
                return _iniciarSdkCommand ?? (_iniciarSdkCommand = new RelayCommand(
                           async () =>
                           {
                               var messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this,
                                   "Iniciar SDK",
                                   "Con que sistema quiere trabajar?",
                                   MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary,
                                   new MetroDialogSettings
                                   {
                                       AffirmativeButtonText = "Factura Electronica",
                                       NegativeButtonText = "Adminpaq",
                                       FirstAuxiliaryButtonText = "Comercial Premium",
                                       SecondAuxiliaryButtonText = "Contabilidad",
                                       DefaultButtonFocus = MessageDialogResult.SecondAuxiliary
                                   });

                               switch (messageDialogResult)
                               {
                                   case MessageDialogResult.Affirmative:
                                       _applicationConfiguration.SistemaElegido = SistemaContpaqEnum.FacturaElectronica;
                                       _applicationConfiguration.ContpaqiSdk = new FacturaElectronicaSdkExtended();
                                       break;
                                   case MessageDialogResult.Negative:
                                       _applicationConfiguration.SistemaElegido = SistemaContpaqEnum.Adminpaq;
                                       _applicationConfiguration.ContpaqiSdk = new AdminpaqSdkExtended();
                                       break;
                                   case MessageDialogResult.FirstAuxiliary:
                                       _applicationConfiguration.SistemaElegido = SistemaContpaqEnum.Comercial;
                                       _applicationConfiguration.ContpaqiSdk = new ComercialSdkExtended();
                                       break;
                                   case MessageDialogResult.SecondAuxiliary:
                                       _applicationConfiguration.SistemaElegido = SistemaContpaqEnum.Contabilidad;
                                       _applicationConfiguration.SdkSesion = new TSdkSesion();
                                       break;
                               }

                               if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
                               {
                                   _applicationConfiguration.SdkSesion.iniciaConexion();
                                   _applicationConfiguration.SdkSesion.firmaUsuario();
                               }
                               else
                               {
                                   _applicationConfiguration.ContpaqiSdkUnidadTrabajo = new ContpaqiSdkUnidadTrabajo(_applicationConfiguration.ContpaqiSdk);
                                   _applicationConfiguration.ContpaqiSdkUnidadTrabajo.ErrorContpaqiSdkRepositorio.ResultadoSdk = _applicationConfiguration.ContpaqiSdk.InicializarSDK();
                               }

                               _applicationConfiguration.SdkInicializado = true;
                               FloatingPointReset.Action();

                               if (AbrirEmpresaCommand.CanExecute(null))
                               {
                                   AbrirEmpresaCommand.Execute(null);
                               }
                           },
                           () => !_applicationConfiguration.SdkInicializado));
            }
        }

        public RelayCommand TerminarSdkCommand
        {
            get
            {
                return _terminarSdkCommand ?? (_terminarSdkCommand = new RelayCommand(
                           () =>
                           {
                               if (CerrarEmpresaCommand.CanExecute(null))
                               {
                                   CerrarEmpresaCommand.Execute(null);
                               }

                               if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
                               {
                                   _applicationConfiguration.SdkSesion.finalizaConexion();
                               }
                               else
                               {
                                   _applicationConfiguration.ContpaqiSdk.fTerminaSDK();
                               }

                               _applicationConfiguration.SdkInicializado = false;
                           },
                           () => _applicationConfiguration.SdkInicializado));
            }
        }

        public RelayCommand AbrirEmpresaCommand
        {
            get
            {
                return _abrirEmpresaCommand ?? (_abrirEmpresaCommand = new RelayCommand(
                           () =>
                           {
                               var viewModel = SimpleIoc.Default.GetInstance<SeleccionarEmpresaViewModel>();
                               viewModel.ShowView();
                           },
                           () => _applicationConfiguration.SdkInicializado && !_applicationConfiguration.EmpresaAbierta));
            }
        }

        public RelayCommand CerrarEmpresaCommand
        {
            get
            {
                return _cerrarEmpresaCommand ?? (_cerrarEmpresaCommand = new RelayCommand(
                           () =>
                           {
                               if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
                               {
                                   _applicationConfiguration.SdkSesion.cierraEmpresa();
                               }
                               else
                               {
                                   _applicationConfiguration.ContpaqiSdk.fCierraEmpresa();
                               }

                               _applicationConfiguration.EmpresaAbierta = false;
                               _applicationConfiguration.Empresa = null;
                           },
                           () => _applicationConfiguration.SdkInicializado && _applicationConfiguration.EmpresaAbierta));
            }
        }

        public RelayCommand AbrirAcercaDeCommand
        {
            get
            {
                return _abrirAcercaDeCommand ?? (_abrirAcercaDeCommand = new RelayCommand(
                           () =>
                           {
                               var viewmodel = SimpleIoc.Default.GetInstance<AcercaDeViewModel>();
                               viewmodel.ShowView();
                           },
                           () => true));
            }
        }

        public RelayCommand BuscarActualizacionCommand
        {
            get
            {
                return _buscarActualizacionCommand ?? (_buscarActualizacionCommand = new RelayCommand(
                           async () =>
                           {
                               string currentverstion;
                               try
                               {
                                   currentverstion = new WebClient().DownloadString("https://arsoftware.blob.core.windows.net/arsoftwaredownloads/ListaNegraSat/currentVersion.txt");
                               }
                               catch (Exception ex)
                               {
                                   await _dialogCoordinator.ShowMessageAsync(this,
                                       "Error",
                                       $"Hubo un error tratando de buscar actualizaciones. Intente mas tarde. Error: {ex}");
                                   return;
                               }
                               var version1 = Assembly.GetExecutingAssembly().GetName().Version;
                               var version2 = new Version(currentverstion);
                               var result = version1.CompareTo(version2);
                               if (result > 0)
                               {
                               }
                               else if (result < 0)
                               {
                                   var dialogResult = await _dialogCoordinator.ShowMessageAsync(
                                       this,
                                       "Actualizacion Disponible",
                                       $"Hay una actualizacion ({currentverstion}) disponible. Desea descargarla?",
                                       MessageDialogStyle.AffirmativeAndNegative);
                                   if (dialogResult == MessageDialogResult.Affirmative)
                                   {
                                       var saveFileDialog = new SaveFileDialog
                                       {
                                           Filter = "zip | (*.zip)",
                                           FileName = $"ListaNegraSat_{version2}.zip"
                                       };
                                       if (saveFileDialog.ShowDialog() == true)
                                       {
                                           using (var webClient = new WebClient())
                                           {
                                               var url = "https://arsoftware.blob.core.windows.net/arsoftwaredownloads/ListaNegraSat/Release.zip";
                                               webClient.DownloadFile(url, saveFileDialog.FileName);
                                           }
                                       }
                                       await _dialogCoordinator.ShowMessageAsync(this,
                                           "Actualizacion Descargada",
                                           "Cierre la aplicacion antes de correr el instalador.");
                                   }
                               }
                               else
                               {
                                   await _dialogCoordinator.ShowMessageAsync(this,
                                       "No Hay Actualizaciones Disponibles",
                                       "Ya esta instalada la ultima version del progrma.");
                               }
                           },
                           () => true));
            }
        }

        public RelayCommand AbrirManualDeUsuarioCommand
        {
            get
            {
                return _abrirManualDeUsuarioCommand ?? (_abrirManualDeUsuarioCommand = new RelayCommand(
                           () =>
                           {
                               var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ManualListaNegraSatContpaqi.pdf");
                               Process.Start(path);
                           },
                           () => true));
            }
        }

        private void RegisterMessages()
        {
            MessengerInstance.Register<NotificationMessage<Empresa>>(this, messege => AbrirEmpresaContpaq(messege.Content));
        }

        private void AbrirEmpresaContpaq(Empresa empresa)
        {
            if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
            {
                _applicationConfiguration.SdkSesion.abreEmpresa(empresa.Ruta);
            }
            else
            {
                _applicationConfiguration.ContpaqiSdkUnidadTrabajo.ErrorContpaqiSdkRepositorio.ResultadoSdk = _applicationConfiguration.ContpaqiSdk.fAbreEmpresa(empresa.Ruta);
            }

            _applicationConfiguration.Empresa = empresa;
            _applicationConfiguration.EmpresaAbierta = true;
        }
    }
}