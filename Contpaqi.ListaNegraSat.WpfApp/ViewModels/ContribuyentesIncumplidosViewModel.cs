using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Contpaqi.ListaNegraSat.WpfApp.DAL;
using Contpaqi.ListaNegraSat.WpfApp.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OfficeOpenXml;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class ContribuyentesIncumplidosViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IDialogCoordinator _dialogCoordinator;
        private RelayCommand _cargarCommand;
        private RelayCommand _cargarContpaqCommand;
        private bool _cargoContribuyentesSat;
        private ContribuyenteContpaq _contribuyenteContpaqSeleccionado;
        private List<Contribuyente> _contribuyentes = new List<Contribuyente>();
        private Contribuyente _contribuyenteSatSeleccionado;
        private ListCollectionView _contribuyentesCollectionView;
        private List<ContribuyenteContpaq> _contribuyentesContpaq = new List<ContribuyenteContpaq>();
        private ListCollectionView _contribuyentesContpaqCollectionView;
        private RelayCommand _copiarRfcContribuyenteContpaqCommand;
        private RelayCommand _copiarRfcContribuyenteSatCommand;
        private RelayCommand _exportarContribuyentesContpaqCommand;
        private string _filtro;
        private string _filtroContpaq;

        public ContribuyentesIncumplidosViewModel(IDialogCoordinator dialogCoordinator, ApplicationConfiguration applicationConfiguration)
        {
            _dialogCoordinator = dialogCoordinator;
            _applicationConfiguration = applicationConfiguration;
            _contribuyentesCollectionView = new ListCollectionView(_contribuyentes);
            _contribuyentesCollectionView.Filter = ContribuyentesCollectionViewFilter;
            _contribuyentesContpaqCollectionView = new ListCollectionView(_contribuyentesContpaq);
            _contribuyentesContpaqCollectionView.Filter = ContribuyentesContpaqCollectionViewFilter;
        }

        public string Filtro
        {
            get => _filtro;
            set
            {
                Set(() => Filtro, ref _filtro, value);
                ContribuyentesCollectionView.Refresh();
            }
        }

        public string FiltroContpaq
        {
            get => _filtroContpaq;
            set
            {
                ContribuyentesContpaqCollectionView.Refresh();
                Set(() => FiltroContpaq, ref _filtroContpaq, value);
            }
        }

        public ListCollectionView ContribuyentesCollectionView
        {
            get => _contribuyentesCollectionView;
            set => Set(() => ContribuyentesCollectionView, ref _contribuyentesCollectionView, value);
        }

        public ListCollectionView ContribuyentesContpaqCollectionView
        {
            get => _contribuyentesContpaqCollectionView;
            set => Set(() => ContribuyentesContpaqCollectionView, ref _contribuyentesContpaqCollectionView, value);
        }

        public int ContribuyentesCount => Contribuyentes.Count;

        public int ContribuyentesContpaqCount => ContribuyentesContpaq.Count;

        public List<Contribuyente> Contribuyentes
        {
            get => _contribuyentes;
            set => Set(() => Contribuyentes, ref _contribuyentes, value);
        }

        public List<ContribuyenteContpaq> ContribuyentesContpaq
        {
            get => _contribuyentesContpaq;
            set => Set(() => ContribuyentesContpaq, ref _contribuyentesContpaq, value);
        }

        public Contribuyente ContribuyenteSatSeleccionado
        {
            get => _contribuyenteSatSeleccionado;
            set => Set(() => ContribuyenteSatSeleccionado, ref _contribuyenteSatSeleccionado, value);
        }

        public ContribuyenteContpaq ContribuyenteContpaqSeleccionado
        {
            get => _contribuyenteContpaqSeleccionado;
            set => Set(() => ContribuyenteContpaqSeleccionado, ref _contribuyenteContpaqSeleccionado, value);
        }

        public RelayCommand CargarCommand
        {
            get
            {
                return _cargarCommand ?? (_cargarCommand = new RelayCommand(
                           async () =>
                           {
                               var progressDialogController = await _dialogCoordinator.ShowProgressAsync(
                                   this,
                                   "Cargando Contribuyentes SAT",
                                   "Cargando contribuyentes SAT.");
                               await Task.Delay(1000);

                               Load();
                               _cargoContribuyentesSat = true;

                               await progressDialogController.CloseAsync();
                           },
                           () => true));
            }
        }

        public RelayCommand CargarContpaqCommand
        {
            get
            {
                return _cargarContpaqCommand ?? (_cargarContpaqCommand = new RelayCommand(
                           async () =>
                           {
                               var progressDialogController = await _dialogCoordinator.ShowProgressAsync(
                                   this,
                                   "Cargando Contribuyentes CONTPAQI",
                                   "Cargando contribuyentes CONTPAQI.");
                               await Task.Delay(1000);

                               LoadClientesContpaq();

                               await progressDialogController.CloseAsync();
                           },
                           () => _applicationConfiguration.SdkInicializado &&
                                 _applicationConfiguration.EmpresaAbierta &&
                                 _cargoContribuyentesSat));
            }
        }

        public RelayCommand ExportarContribuyentesContpaqCommand
        {
            get
            {
                return _exportarContribuyentesContpaqCommand ?? (_exportarContribuyentesContpaqCommand = new RelayCommand(
                           async () =>
                           {
                               var messageDialogResult = await _dialogCoordinator.ShowMessageAsync(
                                   this,
                                   "Exportar Contribuyentes Incumplidos Contpaq",
                                   "En que formato desea exportar?",
                                   MessageDialogStyle.AffirmativeAndNegative,
                                   new MetroDialogSettings
                                   {
                                       AffirmativeButtonText = "TXT",
                                       NegativeButtonText = "EXCEL"
                                   });
                               string ruta = null;
                               switch (messageDialogResult)
                               {
                                   case MessageDialogResult.Affirmative:
                                       ruta = ExportarTxt();
                                       break;
                                   case MessageDialogResult.Negative:
                                       ruta = ExportarExcel();
                                       break;
                               }
                               if (ruta != null)
                               {
                                   Process.Start(ruta);
                               }
                           },
                           () => ContribuyentesContpaq.Count > 0));
            }
        }

        public RelayCommand CopiarRfcContribuyenteSatCommand
        {
            get
            {
                return _copiarRfcContribuyenteSatCommand ?? (_copiarRfcContribuyenteSatCommand = new RelayCommand(
                           () => { Clipboard.SetText(ContribuyenteSatSeleccionado.Rfc); },
                           () => ContribuyenteSatSeleccionado != null));
            }
        }

        public RelayCommand CopiarRfcContribuyenteContpaqCommand
        {
            get
            {
                return _copiarRfcContribuyenteContpaqCommand ?? (_copiarRfcContribuyenteContpaqCommand = new RelayCommand(
                           () => { Clipboard.SetText(ContribuyenteContpaqSeleccionado.Rfc); },
                           () => ContribuyenteContpaqSeleccionado != null));
            }
        }

        public void Load()
        {
            Contribuyentes.Clear();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Listado_Completo_69.xlsx");
            using (var excelPackage = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = excelPackage.Workbook.Worksheets.First();
                var rows = worksheet.Dimension.End.Row;
                for (var i = 2; i <= rows; i++)
                {
                    if (worksheet.Cells[i, 1].Value == null)
                    {
                        continue;
                    }

                    var contribuyente = new Contribuyente
                    {
                        Rfc = worksheet.Cells[i, 1].Value.ToString(),
                        RazonSocial = worksheet.Cells[i, 2].Value.ToString(),
                        TipoPersona = worksheet.Cells[i, 3].Value.ToString(),
                        Supuesto = worksheet.Cells[i, 4].Value.ToString()
                    };

                    Contribuyentes.Add(contribuyente);
                }
            }

            ContribuyentesCollectionView.Refresh();
            RaisePropertyChanged(nameof(ContribuyentesCount));
        }

        public void LoadClientesContpaq()
        {
            var rep = new ClienteContpaqRepositorio(_applicationConfiguration.ContpaqiSdk);
            var clientesContpaq = rep.TraerClientes();
            ContribuyentesContpaq.AddRange(clientesContpaq.Where(c => Contribuyentes.Any(con => con.Rfc == c.Rfc)));
            foreach (var contribuyenteContpaq in ContribuyentesContpaq)
            {
                var contribuyenteSat = Contribuyentes.First(c => c.Rfc == contribuyenteContpaq.Rfc);
                contribuyenteContpaq.Supuesto = contribuyenteSat.Supuesto;
                contribuyenteContpaq.TipoPersona = contribuyenteSat.TipoPersona;
            }
            ContribuyentesContpaqCollectionView.Refresh();
            RaisePropertyChanged(nameof(ContribuyentesContpaqCount));
        }

        private bool ContribuyentesCollectionViewFilter(object obj)
        {
            if (string.IsNullOrEmpty(Filtro))
            {
                return true;
            }

            var contribuyente = obj as Contribuyente;

            return contribuyente.Rfc.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   contribuyente.RazonSocial.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool ContribuyentesContpaqCollectionViewFilter(object obj)
        {
            if (string.IsNullOrEmpty(FiltroContpaq))
            {
                return true;
            }

            var contribuyente = obj as ContribuyenteContpaq;

            return contribuyente.Rfc.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   contribuyente.Codigo.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   contribuyente.RazonSocial.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string ExportarTxt()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt | .txt";
            saveFileDialog.FileName = "ContribuyentesIncumplidosContpaq.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                var ruta = saveFileDialog.FileName;
                File.WriteAllLines(ruta, ContribuyentesContpaq.Select(c => $"{c.Rfc}|{c.Codigo}|{c.RazonSocial}|{c.Supuesto}|{c.TipoPersona}"));
                return saveFileDialog.FileName;
            }
            return null;
        }

        private string ExportarExcel()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel | .xlsx";
            saveFileDialog.FileName = "ContribuyentesIncumplidosContpaq.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                var ruta = saveFileDialog.FileName;
                using (var excelpackage = new ExcelPackage(new FileInfo(ruta)))
                {
                    var worksheet = excelpackage.Workbook.Worksheets.Add("ContribuyentesIncumplidosContpaq");
                    worksheet.Cells[1, 1].Value = "RFC";
                    worksheet.Cells[1, 2].Value = "Codigo";
                    worksheet.Cells[1, 3].Value = "Razon Social";
                    worksheet.Cells[1, 4].Value = "Supuesto";
                    worksheet.Cells[1, 5].Value = "Tipo Persona";
                    worksheet.Cells[1, 6].Value = "Tipo";
                    worksheet.Cells[1, 7].Value = "Estatus";
                    var i = 2;
                    foreach (var clienteContpaq in ContribuyentesContpaq)
                    {
                        worksheet.Cells[i, 1].Value = clienteContpaq.Rfc;
                        worksheet.Cells[i, 2].Value = clienteContpaq.Codigo;
                        worksheet.Cells[i, 3].Value = clienteContpaq.RazonSocial;
                        worksheet.Cells[i, 4].Value = clienteContpaq.Supuesto;
                        worksheet.Cells[i, 5].Value = clienteContpaq.TipoPersona;
                        worksheet.Cells[i, 6].Value = clienteContpaq.Tipo;
                        worksheet.Cells[i, 7].Value = clienteContpaq.Estatus;
                        i++;
                    }
                    worksheet.Cells.AutoFitColumns();
                    excelpackage.Save();
                }
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}