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

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IDialogCoordinator _dialogCoordinator;

        private RelayCommand _abrirEmpresaCommand;

        private RelayCommand _cerrarEmpresaCommand;
        private RelayCommand _iniciarSdkCommand;
        private RelayCommand _terminarSdkCommand;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator, ApplicationConfiguration applicationConfiguration)
        {
            _dialogCoordinator = dialogCoordinator;
            _applicationConfiguration = applicationConfiguration;
            RegisterMessages();
            CurrentMainView = SimpleIoc.Default.GetInstance<ContribuyentesIncumplidosViewModel>();
        }

        private ViewModelBase _currentMainView;

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
                                       SecondAuxiliaryButtonText = "Cancelar",
                                       DefaultButtonFocus = MessageDialogResult.SecondAuxiliary
                                   });

                               switch (messageDialogResult)
                               {
                                   case MessageDialogResult.Affirmative:
                                       _applicationConfiguration.ContpaqiSdk = new FacturaElectronicaSdkExtended();
                                       break;
                                   case MessageDialogResult.Negative:
                                       _applicationConfiguration.ContpaqiSdk = new AdminpaqSdkExtended();
                                       break;
                                   case MessageDialogResult.FirstAuxiliary:
                                       _applicationConfiguration.ContpaqiSdk = new ComercialSdkExtended();
                                       break;
                                   case MessageDialogResult.SecondAuxiliary:
                                       _applicationConfiguration.ContpaqiSdk = null;
                                       return;
                               }

                               _applicationConfiguration.ContpaqiSdkUnidadTrabajo = new ContpaqiSdkUnidadTrabajo(_applicationConfiguration.ContpaqiSdk);
                               _applicationConfiguration.ContpaqiSdkUnidadTrabajo.ErrorContpaqiSdkRepositorio.ResultadoSdk = _applicationConfiguration.ContpaqiSdk.InicializarSDK();
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
                               _applicationConfiguration.ContpaqiSdk.fTerminaSDK();
                               _applicationConfiguration.SdkInicializado = false;
                               if (CerrarEmpresaCommand.CanExecute(null))
                               {
                                   CerrarEmpresaCommand.Execute(null);
                               }
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
                               _applicationConfiguration.ContpaqiSdk.fCierraEmpresa();
                               _applicationConfiguration.EmpresaAbierta = false;
                               _applicationConfiguration.Empresa = null;
                           },
                           () => _applicationConfiguration.SdkInicializado && _applicationConfiguration.EmpresaAbierta));
            }
        }

        private void RegisterMessages()
        {
            MessengerInstance.Register<NotificationMessage<Empresa>>(this, messege => AbrirEmpresaContpaq(messege.Content));
        }

        private void AbrirEmpresaContpaq(Empresa empresa)
        {
            _applicationConfiguration.ContpaqiSdkUnidadTrabajo.ErrorContpaqiSdkRepositorio.ResultadoSdk = _applicationConfiguration.ContpaqiSdk.fAbreEmpresa(empresa.Ruta);
            _applicationConfiguration.Empresa = empresa;
            _applicationConfiguration.EmpresaAbierta = true;
        }
    }
}