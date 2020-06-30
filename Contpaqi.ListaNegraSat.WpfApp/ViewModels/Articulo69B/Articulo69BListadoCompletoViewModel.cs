using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Articulo69B.Models;
using ListaNegraSat.Core.Application.Articulo69B.Queries.BuscarContribuyentes69B;
using ListaNegraSat.Core.Application.Common;
using ListaNegraSat.Presentation.WpfApp.Models;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Articulo69B
{
    public sealed class Articulo69BListadoCompletoViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ConfiguracionAplicacion _configuracionAplicacion;
        private readonly IMediator _mediator;
        private Contribuyente69BDto _contribuyenteSeleccionado;
        private int _definitivosTotal;
        private int _desvirtuadosTotal;
        private string _filtro;
        private int _presuntosTotal;
        private int _sentenciasFavorablesTotal;
        private SituacionEnumeration _situacionFiltroSeleccionada;

        public Articulo69BListadoCompletoViewModel(ConfiguracionAplicacion configuracionAplicacion,  IMediator mediator, IDialogCoordinator dialogCoordinator)
        {
            _configuracionAplicacion = configuracionAplicacion;
            _mediator = mediator;
            _dialogCoordinator = dialogCoordinator;
            DisplayName = "Articulo 69-B Listado Completo";
            ContribuyentesView = CollectionViewSource.GetDefaultView(Contribuyentes);
            ContribuyentesView.Filter = ContribuyentesView_Filter;
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
                ContribuyentesView.Refresh();
                RaiseGuards();
            }
        }

        public BindableCollection<SituacionEnumeration> SituacionesFiltro { get; } = new BindableCollection<SituacionEnumeration>(Enumeration.GetAll<SituacionEnumeration>());

        public SituacionEnumeration SituacionFiltroSeleccionada
        {
            get => _situacionFiltroSeleccionada;
            set
            {
                if (value == _situacionFiltroSeleccionada)
                {
                    return;
                }

                _situacionFiltroSeleccionada = value;
                NotifyOfPropertyChange(() => SituacionFiltroSeleccionada);
                ContribuyentesView.Refresh();
                RaiseGuards();
            }
        }

        public BindableCollection<Contribuyente69BDto> Contribuyentes { get; } = new BindableCollection<Contribuyente69BDto>();

        public ICollectionView ContribuyentesView { get; set; }

        public Contribuyente69BDto ContribuyenteSeleccionado
        {
            get => _contribuyenteSeleccionado;
            set
            {
                if (Equals(value, _contribuyenteSeleccionado))
                {
                    return;
                }

                _contribuyenteSeleccionado = value;
                NotifyOfPropertyChange(() => ContribuyenteSeleccionado);
            }
        }

        public int DefinitivosTotal
        {
            get => _definitivosTotal;
            private set
            {
                if (value == _definitivosTotal)
                {
                    return;
                }

                _definitivosTotal = value;
                NotifyOfPropertyChange(() => DefinitivosTotal);
            }
        }

        public int DesvirtuadosTotal
        {
            get => _desvirtuadosTotal;
            private set
            {
                if (value == _desvirtuadosTotal)
                {
                    return;
                }

                _desvirtuadosTotal = value;
                NotifyOfPropertyChange(() => DesvirtuadosTotal);
            }
        }

        public int PresuntosTotal
        {
            get => _presuntosTotal;
            private set
            {
                if (value == _presuntosTotal)
                {
                    return;
                }

                _presuntosTotal = value;
                NotifyOfPropertyChange(() => PresuntosTotal);
            }
        }

        public int SentenciasFavorablesTotal
        {
            get => _sentenciasFavorablesTotal;
            private set
            {
                if (value == _sentenciasFavorablesTotal)
                {
                    return;
                }

                _sentenciasFavorablesTotal = value;
                NotifyOfPropertyChange(() => SentenciasFavorablesTotal);
            }
        }

        public bool CanExportarFiltroExcelAsync => ContribuyentesView.Cast<object>().Any();

        public async Task BuscarContribuyentesAsync()
        {
            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Buscando Contribuyentes", "Buscando contribuyentes");
            progressDialogController.SetIndeterminate();
            await Task.Delay(1000);
            try
            {
                var listado = await _mediator.Send(new BuscarContribuyentes69BQuery(_configuracionAplicacion.RutaArchivoListadoCompleto));
                DisplayName = $"Articulo 69-B Listado Completo - {listado.Version}";

                Contribuyentes.Clear();
                Contribuyentes.AddRange(listado.Contribuyentes);

                DefinitivosTotal = listado.DefinitivosTotal;
                DesvirtuadosTotal = listado.DesvirtuadosTotal;
                PresuntosTotal = listado.PresuntosTotal;
                SentenciasFavorablesTotal = listado.SentenciasFavorablesTotal;

                SituacionFiltroSeleccionada = SituacionesFiltro.FirstOrDefault();
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                await progressDialogController.CloseAsync();
                RaiseGuards();
            }
        }

        public async Task ExportarFiltroExcelAsync()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel | *.xlsx";
            saveFileDialog.FileName = "Contribuyentes.xlsx";
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Exportando", "Exportando");

            try
            {
                using (var excelPackage = new ExcelPackage(new FileInfo(saveFileDialog.FileName)))
                {
                    var excelWorksheet = excelPackage.Workbook.Worksheets.Add("Contribuyentes");

                    excelWorksheet.Cells.LoadFromCollection(ContribuyentesView.Cast<Contribuyente69BDto>(), true);
                    excelWorksheet.Cells.AutoFitColumns(20, 100);
                    excelPackage.Save();
                }

                Process.Start(saveFileDialog.FileName);
            }
            catch (Exception e)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            }
            finally
            {
                await progressDialogController.CloseAsync();
                RaiseGuards();
            }
        }

        private void RaiseGuards()
        {
            NotifyOfPropertyChange(() => CanExportarFiltroExcelAsync);
        }

        private bool ContribuyentesView_Filter(object obj)
        {
            if (!(obj is Contribuyente69BDto contribuyente))
            {
                throw new ArgumentNullException(nameof(contribuyente));
            }

            var stringFilterResult = string.IsNullOrWhiteSpace(Filtro) ||
                                     contribuyente.Numero.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     contribuyente.NombreContribuyente.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     contribuyente.Situacion.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;

            bool senteciaResult;
            if (SituacionFiltroSeleccionada == SituacionEnumeration.Todo)
            {
                senteciaResult = true;
            }
            else if (SituacionFiltroSeleccionada == SituacionEnumeration.Definitivos)
            {
                senteciaResult = contribuyente.Situacion == SituacionEnumeration.Definitivos.Name;
            }
            else if (SituacionFiltroSeleccionada == SituacionEnumeration.Desvirtuados)
            {
                senteciaResult = contribuyente.Situacion == SituacionEnumeration.Desvirtuados.Name;
            }
            else if (SituacionFiltroSeleccionada == SituacionEnumeration.Presuntos)
            {
                senteciaResult = contribuyente.Situacion == SituacionEnumeration.Presuntos.Name;
            }
            else if (SituacionFiltroSeleccionada == SituacionEnumeration.SentenciaFavorable)
            {
                senteciaResult = contribuyente.Situacion == SituacionEnumeration.SentenciaFavorable.Name;
            }
            else
            {
                senteciaResult = true;
            }

            return stringFilterResult && senteciaResult;
        }
    }
}