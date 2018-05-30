using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Contpaqi.ListaNegraSat.WpfApp.Messages;
using Contpaqi.ListaNegraSat.WpfApp.Models;
using Contpaqi.Sdk.Extras.Modelos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using SDKCONTPAQNGLib;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class SeleccionarEmpresaViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private RelayCommand _cancelarCommand;
        private ObservableCollection<Empresa> _empresas = new ObservableCollection<Empresa>();
        private Empresa _empresaSeleccionada;
        private RelayCommand _seleccionarEmpresaCommand;

        public SeleccionarEmpresaViewModel(ApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
            LoadEmpresas();
        }

        public ObservableCollection<Empresa> Empresas
        {
            get => _empresas;
            set => Set(() => Empresas, ref _empresas, value);
        }

        public Empresa EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set => Set(() => EmpresaSeleccionada, ref _empresaSeleccionada, value);
        }

        public RelayCommand SeleccionarEmpresaCommand
        {
            get
            {
                return _seleccionarEmpresaCommand ?? (_seleccionarEmpresaCommand = new RelayCommand(
                           () =>
                           {
                               MessengerInstance.Send(new NotificationMessage<Empresa>(this, EmpresaSeleccionada, "EmpresaSeleccionada"));
                               CloseView();
                           },
                           () => EmpresaSeleccionada != null));
            }
        }

        public RelayCommand CancelarCommand
        {
            get
            {
                return _cancelarCommand ?? (_cancelarCommand = new RelayCommand(
                           () => { CloseView(); },
                           () => true));
            }
        }

        public void ShowView()
        {
            MessengerInstance.Send(new ShowViewMessage(this));
        }

        public void CloseView()
        {
            MessengerInstance.Send(new CloseViewMessage(this));
        }

        private void LoadEmpresas()
        {
            Empresas.Clear();

            List<Empresa> empresas;
            if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
            {
                empresas = GetEmpresasContaqbilidad();
            }
            else
            {
                empresas = _applicationConfiguration.ContpaqiSdkUnidadTrabajo.EmpresaRepositorio.TraerEmpresas();
            }

            foreach (var empresa in empresas.OrderBy(e => e.Nombre))
            {
                Empresas.Add(empresa);
            }
        }

        private List<Empresa> GetEmpresasContaqbilidad()
        {
            List<Empresa> e = new List<Empresa>();
            TSdkListaEmpresas empresas = new TSdkListaEmpresas();
            var result = empresas.buscaPrimero();
            if (result == 1)
            {
                var empresa = new Empresa();
                empresa.Nombre = empresas.Nombre;
                empresa.Ruta = empresas.NombreBDD;
                e.Add(empresa);
            }

            while (empresas.buscaSiguiente() == 1)
            {
                var empresa = new Empresa();
                empresa.Nombre = empresas.Nombre;
                empresa.Ruta = empresas.NombreBDD;
                e.Add(empresa);
            }

            return e;
        }
    }
}