using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Expedientes.Models;
using ListaNegraSat.Core.Application.Expedientes.Queries.BuscarExpedientesPorGuidPerteneciente;
using MahApps.Metro.Controls.Dialogs;
using MediatR;
using Microsoft.Win32;
using OfficeOpenXml;

namespace ListaNegraSat.Presentation.WpfApp.ViewModels.Expedientes;

public sealed class ExpedientesListaViewModel : Screen
{
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IMediator _mediator;
    private ExpedienteAddDto _expedienteSeleccionado;

    public ExpedientesListaViewModel(IMediator mediator, IDialogCoordinator dialogCoordinator)
    {
        _mediator = mediator;
        _dialogCoordinator = dialogCoordinator;
        DisplayName = "Expedientes";
    }

    public BindableCollection<ExpedienteAddDto> Expedientes { get; } = new();

    public ExpedienteAddDto ExpedienteSeleccionado
    {
        get => _expedienteSeleccionado;
        set
        {
            if (Equals(value, _expedienteSeleccionado))
            {
                return;
            }

            _expedienteSeleccionado = value;
            NotifyOfPropertyChange(() => ExpedienteSeleccionado);
        }
    }

    public async Task CargarExpedientesPorGuidPertenecienteAsync(Guid guid)
    {
        Expedientes.Clear();
        Expedientes.AddRange(await _mediator.Send(new BuscarExpedientesPorGuidPertenecienteQuery(guid)));
    }

    public async Task ExportarExcelAsync()
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Excel | *.xlsx";
        saveFileDialog.FileName = "Expedientes.xlsx";
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
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Expedientes");

                excelWorksheet.Cells.LoadFromCollection(Expedientes, true);
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
        }
    }
}
