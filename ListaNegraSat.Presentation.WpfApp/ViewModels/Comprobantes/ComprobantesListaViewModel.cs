using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Cfdis.Models;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Expedientes;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Comprobantes;

public sealed class ComprobantesListaViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IWindowManager _windowManager;
    private ComprobanteAddDto _comprobanteSeleccionado;
    private string _filtro;

    public ComprobantesListaViewModel(IDialogCoordinator dialogCoordinator, IWindowManager windowManager)
    {
        _dialogCoordinator = dialogCoordinator;
        _windowManager = windowManager;
        DisplayName = "Comprobantes";
        ComprobantesView = CollectionViewSource.GetDefaultView(Comprobantes);
        ComprobantesView.Filter = ComprbantesView_Filter;
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
            ComprobantesView.Refresh();
            RaiseGuards();
        }
    }

    public BindableCollection<ComprobanteAddDto> Comprobantes { get; } = new();

    public ICollectionView ComprobantesView { get; }

    public ComprobanteAddDto ComprobanteSeleccionado
    {
        get => _comprobanteSeleccionado;
        set
        {
            if (Equals(value, _comprobanteSeleccionado))
            {
                return;
            }

            _comprobanteSeleccionado = value;
            NotifyOfPropertyChange(() => ComprobanteSeleccionado);
            RaiseGuards();
        }
    }

    public bool CanVerExpedienteAsync => ComprobanteSeleccionado != null;

    public bool CanExportarFiltroExcelAsync => ComprobantesView.Cast<object>().Any();

    public void Inicializar(IEnumerable<ComprobanteAddDto> comprbantes)
    {
        Comprobantes.Clear();
        Comprobantes.AddRange(comprbantes);
    }

    public async Task VerExpedienteAsync()
    {
        try
        {
            var expedientesListaViewModel = IoC.Get<ExpedientesListaViewModel>();
            await expedientesListaViewModel.CargarExpedientesPorGuidPertenecienteAsync(ComprobanteSeleccionado.GuidDocument);
            await _windowManager.ShowWindowAsync(expedientesListaViewModel);
        }
        catch (Exception e)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", e.ToString());
            throw;
        }
    }

    public async Task ExportarFiltroExcelAsync()
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Excel | *.xlsx";
        saveFileDialog.FileName = "Comprobantes.xlsx";
        if (saveFileDialog.ShowDialog() != true)
        {
            return;
        }

        ProgressDialogController progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Exportando", "Exportando");
        progressDialogController.SetIndeterminate();
        await Task.Delay(1000);

        try
        {
            using (var excelPackage = new ExcelPackage(new FileInfo(saveFileDialog.FileName)))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Comprobantes");

                excelWorksheet.Cells.LoadFromCollection(ComprobantesView.Cast<ComprobanteAddDto>(), true);
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
        NotifyOfPropertyChange(() => CanVerExpedienteAsync);
        NotifyOfPropertyChange(() => CanExportarFiltroExcelAsync);
    }

    private bool ComprbantesView_Filter(object obj)
    {
        if (!(obj is ComprobanteAddDto comprobante))
        {
            throw new ArgumentNullException(nameof(comprobante));
        }

        return string.IsNullOrWhiteSpace(Filtro) ||
               comprobante.Fecha?.ToShortDateString().IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.Serie?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.Folio?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.RfcEmisor?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.NombreEmisor?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.Moneda?.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.TipoCambio?.ToString().IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.Total?.ToString().IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
               comprobante.GuidDocument.ToString().IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
