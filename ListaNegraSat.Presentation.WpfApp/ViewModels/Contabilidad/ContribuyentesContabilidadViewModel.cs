using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Articulo69B.Models;
using ListaNegraSat.Core.Application.Cfdis.Queries.BuscarComprobantesPorRfcEmitdo;
using ListaNegraSat.Core.Application.Common;
using ListaNegraSat.Core.Application.Contribuyentes.Models;
using ListaNegraSat.Core.Application.Contribuyentes.Queries.BuscarContribuyentesContabilidad;
using ListaNegraSat.Presentation.WpfApp.Properties;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Comprobantes;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Empresas;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Contabilidad;

public sealed class ContribuyentesContabilidadViewModel : Screen
{
    private readonly ConfiguracionAplicacion _configuracionAplicacion;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private readonly IWindowManager _windowManager;
    private ContribuyenteContabilidadDto _contribuyenteSeleccionado;
    private int _definitivosTotal;
    private int _desvirtuadosTotal;
    private string _filtro;
    private int _presuntosTotal;
    private int _sentenciasFavorablesTotal;
    private SituacionEnumeration _situacionFiltroSeleccionada;

    public ContribuyentesContabilidadViewModel(ConfiguracionAplicacion configuracionAplicacion, IMediator mediator,
        IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
    {
        _configuracionAplicacion = configuracionAplicacion;
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        _windowManager = windowManager;
        DisplayName = "Contribuyentes Contabilidad";
        ContribuyentesView = CollectionViewSource.GetDefaultView(Contribuyentes);
        ContribuyentesView.Filter = ContribuyentesView_Filter;
    }

    public string Filtro
    {
        get => _filtro;
        set
        {
            if (value == _filtro) return;

            _filtro = value;
            NotifyOfPropertyChange(() => Filtro);
            ContribuyentesView.Refresh();
            RaiseGuards();
        }
    }

    public BindableCollection<SituacionEnumeration> SituacionesFiltro { get; } = new(Enumeration.GetAll<SituacionEnumeration>());

    public SituacionEnumeration SituacionFiltroSeleccionada
    {
        get => _situacionFiltroSeleccionada;
        set
        {
            if (Equals(value, _situacionFiltroSeleccionada)) return;

            _situacionFiltroSeleccionada = value;
            NotifyOfPropertyChange(() => SituacionFiltroSeleccionada);
            ContribuyentesView.Refresh();
            RaiseGuards();
        }
    }

    public BindableCollection<ContribuyenteContabilidadDto> Contribuyentes { get; } = new();

    public ICollectionView ContribuyentesView { get; set; }

    public ContribuyenteContabilidadDto ContribuyenteSeleccionado
    {
        get => _contribuyenteSeleccionado;
        set
        {
            if (Equals(value, _contribuyenteSeleccionado)) return;

            _contribuyenteSeleccionado = value;
            NotifyOfPropertyChange(() => ContribuyenteSeleccionado);
            RaiseGuards();
        }
    }

    public int DefinitivosTotal
    {
        get => _definitivosTotal;
        private set
        {
            if (value == _definitivosTotal) return;

            _definitivosTotal = value;
            NotifyOfPropertyChange(() => DefinitivosTotal);
        }
    }

    public int DesvirtuadosTotal
    {
        get => _desvirtuadosTotal;
        private set
        {
            if (value == _desvirtuadosTotal) return;

            _desvirtuadosTotal = value;
            NotifyOfPropertyChange(() => DesvirtuadosTotal);
        }
    }

    public int PresuntosTotal
    {
        get => _presuntosTotal;
        private set
        {
            if (value == _presuntosTotal) return;

            _presuntosTotal = value;
            NotifyOfPropertyChange(() => PresuntosTotal);
        }
    }

    public int SentenciasFavorablesTotal
    {
        get => _sentenciasFavorablesTotal;
        private set
        {
            if (value == _sentenciasFavorablesTotal) return;

            _sentenciasFavorablesTotal = value;
            NotifyOfPropertyChange(() => SentenciasFavorablesTotal);
        }
    }

    public bool CanVerComprobantesContribuyenteSeleccionadoAsync => ContribuyenteSeleccionado != null;

    public bool CanExportarFiltroExcelAsync => ContribuyentesView.Cast<object>().Any();

    public async Task BuscarContribuyentesAsync()
    {
        ProgressDialogController progressDialogController =
            await _dialogCoordinator.ShowProgressAsync(this, "Buscando Contribuyentes", "Buscando contribuyentes");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            if (string.IsNullOrWhiteSpace(_configuracionAplicacion.ContpaqiContabilidadConnectionString))
                throw new InvalidOperationException(
                    "El connection string de Contabilidad esta vacio. Debe de asignarlo en la configuracion de aplicacion.");

            var seleccionarEmpresaContabilidadViewModel = IoC.Get<SeleccionarEmpresaContabilidadViewModel>();
            await seleccionarEmpresaContabilidadViewModel.InicializarAsync();
            await _windowManager.ShowDialogAsync(seleccionarEmpresaContabilidadViewModel);
            if (!seleccionarEmpresaContabilidadViewModel.SeleccionoEmpresa) return;

            _configuracionAplicacion.SetEmpresaContabilidad(seleccionarEmpresaContabilidadViewModel.EmpresaSeleccionada);

            BuscarContribuyentesContabilidadResult listado = await _mediator.Send(
                new BuscarContribuyentesContabilidadQuery(
                    _configuracionAplicacion.GetRutaArchivoListadoCompleto(Settings.Default.RutaArchivoListadoCompleto)));
            DisplayName = $"Contribuyentes Contabilidad - {listado.Version}";

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
        var saveFileDialog = new SaveFileDialog { Filter = "Excel | *.xlsx", FileName = "Contribuyentes.xlsx" };
        if (saveFileDialog.ShowDialog() != true) return;

        ProgressDialogController progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Exportando", "Exportando");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            using (var excelPackage = new ExcelPackage(new FileInfo(saveFileDialog.FileName)))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Contribuyentes");

                excelWorksheet.Cells.LoadFromCollection(ContribuyentesView.Cast<ContribuyenteContabilidadDto>(), true);
                excelWorksheet.Cells.AutoFitColumns(20, 100);
                excelPackage.Save();
            }

            Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
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

    public async Task VerComprobantesContribuyenteSeleccionadoAsync()
    {
        ProgressDialogController progressDialogController =
            await _dialogCoordinator.ShowProgressAsync(this, "Buscando comprobantes", "Buscando");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            if (string.IsNullOrWhiteSpace(_configuracionAplicacion.ContpaqiAddConnetionString))
                throw new InvalidOperationException(
                    "El connection string del ADD esta vacio. Debe de asignarlo en la configuracion de aplicacion.");

            var comprobantesListaViewModel = IoC.Get<ComprobantesListaViewModel>();
            comprobantesListaViewModel.Inicializar(
                await _mediator.Send(new BuscarComprobantesPorRfcEmitdoQuery(ContribuyenteSeleccionado.Rfc)));
            await _windowManager.ShowWindowAsync(comprobantesListaViewModel);
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
        NotifyOfPropertyChange(() => CanVerComprobantesContribuyenteSeleccionadoAsync);
    }

    private bool ContribuyentesView_Filter(object obj)
    {
        if (!(obj is ContribuyenteContabilidadDto contribuyente)) throw new ArgumentNullException(nameof(contribuyente));

        bool stringFilterResult = string.IsNullOrWhiteSpace(Filtro) ||
                                  contribuyente.Codigo?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  contribuyente.RazonSocial?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  contribuyente.Rfc?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  contribuyente.ListadoNumero?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  contribuyente.ListadoNombreContribuyente?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                  contribuyente.ListadoSituacion?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;

        bool senteciaResult;
        if (Equals(SituacionFiltroSeleccionada, SituacionEnumeration.Todo))
            senteciaResult = true;
        else if (Equals(SituacionFiltroSeleccionada, SituacionEnumeration.Definitivos))
            senteciaResult = contribuyente.ListadoSituacion == SituacionEnumeration.Definitivos.Name;
        else if (Equals(SituacionFiltroSeleccionada, SituacionEnumeration.Desvirtuados))
            senteciaResult = contribuyente.ListadoSituacion == SituacionEnumeration.Desvirtuados.Name;
        else if (Equals(SituacionFiltroSeleccionada, SituacionEnumeration.Presuntos))
            senteciaResult = contribuyente.ListadoSituacion == SituacionEnumeration.Presuntos.Name;
        else if (Equals(SituacionFiltroSeleccionada, SituacionEnumeration.SentenciaFavorable))
            senteciaResult = contribuyente.ListadoSituacion == SituacionEnumeration.SentenciaFavorable.Name;
        else
            senteciaResult = true;

        return stringFilterResult && senteciaResult;
    }
}
