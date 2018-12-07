using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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
        private List<Empresa> _empresas = new List<Empresa>();
        private ICollectionView _empresasCollectionView;
        private Empresa _empresaSeleccionada;
        private string _filtro;
        private RelayCommand _seleccionarEmpresaCommand;

        public SeleccionarEmpresaViewModel(ApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
            _empresasCollectionView = CollectionViewSource.GetDefaultView(_empresas);
            _empresasCollectionView.Filter = EmpresasCollectionViewFilter;
            LoadEmpresas();
        }

        public string Filtro
        {
            get => _filtro;
            set
            {
                Set(() => Filtro, ref _filtro, value);
                EmpresasCollectionView.Refresh();
            }
        }

        public ICollectionView EmpresasCollectionView
        {
            get => _empresasCollectionView;
            set => Set(() => EmpresasCollectionView, ref _empresasCollectionView, value);
        }

        public List<Empresa> Empresas
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

        private bool EmpresasCollectionViewFilter(object obj)
        {
            return true;
        }

        public void ShowView()
        {
            MessengerInstance.Send(new ShowViewMessage(this));
        }

        private void CloseView()
        {
            MessengerInstance.Send(new CloseViewMessage(this));
        }

        private void LoadEmpresas()
        {
            Empresas.Clear();

            var empresas = _applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad
                ? GetEmpresasContaqbilidad()
                : _applicationConfiguration.ContpaqiSdkUnidadTrabajo.EmpresaRepositorio.TraerEmpresas();

            Empresas.AddRange(empresas.OrderBy(e => e.Nombre));
            EmpresasCollectionView.Refresh();
        }

        private List<Empresa> GetEmpresasContaqbilidad()
        {
            var empresas = new List<Empresa>();

            var sdkListaEmpresas = new TSdkListaEmpresas();

            var result = sdkListaEmpresas.buscaPrimero();
            if (result == 1)
            {
                var empresa = new Empresa
                {
                    Nombre = sdkListaEmpresas.Nombre,
                    Ruta = sdkListaEmpresas.NombreBDD
                };
                empresas.Add(empresa);

                while (sdkListaEmpresas.buscaSiguiente() == 1)
                {
                    empresa = new Empresa
                    {
                        Nombre = sdkListaEmpresas.Nombre,
                        Ruta = sdkListaEmpresas.NombreBDD
                    };
                    empresas.Add(empresa);
                }
            }

            return empresas;
        }
    }
}