using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Empresas.Models;
using ListaNegraSat.Core.Application.Empresas.Queries.BuscarEmpresasContabilidad;
using MediatR;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Empresas
{
    public class SeleccionarEmpresaContabilidadViewModel : Screen
    {
        private readonly IMediator _mediator;
        private EmpresaContabilidadDto _empresaSeleccionada;
        private string _filtro;

        public SeleccionarEmpresaContabilidadViewModel(IMediator mediator)
        {
            _mediator = mediator;
            DisplayName = "Seleccionar Empresa Contabilidad";
            EmpresasView = CollectionViewSource.GetDefaultView(Empresas);
            EmpresasView.Filter = EmpresasView_Filter;
        }

        public string Filtro
        {
            get => _filtro;
            set
            {
                if (value == _filtro)
                {
                    return;
                }

                _filtro = value;
                NotifyOfPropertyChange(() => Filtro);
                EmpresasView.Refresh();
            }
        }

        public BindableCollection<EmpresaContabilidadDto> Empresas { get; } = new BindableCollection<EmpresaContabilidadDto>();

        public ICollectionView EmpresasView { get; }

        public EmpresaContabilidadDto EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set
            {
                if (Equals(value, _empresaSeleccionada))
                {
                    return;
                }

                _empresaSeleccionada = value;
                NotifyOfPropertyChange(() => EmpresaSeleccionada);
                RaiseGuards();
            }
        }

        public bool SeleccionoEmpresa { get; private set; }

        public bool CanSeleccionar => EmpresaSeleccionada != null;

        public async Task InicializarAsync()
        {
            await CargarEmpresasAsync();
        }

        private async Task CargarEmpresasAsync()
        {
            Empresas.Clear();
            Empresas.AddRange(await _mediator.Send(new BuscarEmpresasContabilidadQuery()));
        }

        public void Seleccionar()
        {
            SeleccionoEmpresa = true;
            TryClose();
        }

        public void Cancelar()
        {
            SeleccionoEmpresa = false;
            TryClose();
        }

        public void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanSeleccionar);
        }

        private bool EmpresasView_Filter(object obj)
        {
            var empresa = obj as EmpresaContabilidadDto;
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa));
            }

            return string.IsNullOrWhiteSpace(Filtro) ||
                   empresa.Nombre.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}