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
using CsvHelper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OfficeOpenXml;
using SDKCONTPAQNGLib;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class Articulo69BViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IDialogCoordinator _dialogCoordinator;
        private RelayCommand _abrirArchivoCsvCommand;
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

        public Articulo69BViewModel(IDialogCoordinator dialogCoordinator, ApplicationConfiguration applicationConfiguration)
        {
            _dialogCoordinator = dialogCoordinator;
            _applicationConfiguration = applicationConfiguration;
            _contribuyentesCollectionView = new ListCollectionView(_contribuyentes)
            {
                Filter = ContribuyentesCollectionViewFilter
            };
            _contribuyentesContpaqCollectionView = new ListCollectionView(_contribuyentesContpaq)
            {
                Filter = ContribuyentesContpaqCollectionViewFilter
            };
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
                return _cargarCommand ?? (_cargarCommand = new RelayCommand(async () =>
                           {
                               var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Cargando Contribuyentes SAT", "Cargando contribuyentes SAT.");
                               await Task.Delay(1000);

                               LoadFile();

                               _cargoContribuyentesSat = true;

                               await progressDialogController.CloseAsync();
                           },
                           () => true));
            }
        }

        public RelayCommand AbrirArchivoCsvCommand
        {
            get
            {
                return _abrirArchivoCsvCommand ?? (_abrirArchivoCsvCommand = new RelayCommand(
                           async () =>
                           {
                               try
                               {
                                   Process.Start(_applicationConfiguration.RutaArchivo69B);
                               }
                               catch (Exception exception)
                               {
                                   await _dialogCoordinator.ShowMessageAsync(this, "Error", exception.Message);
                               }
                           },
                           () => true));
            }
        }

        public RelayCommand CargarContpaqCommand
        {
            get
            {
                return _cargarContpaqCommand ?? (_cargarContpaqCommand = new RelayCommand(async () =>
                           {
                               var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, "Cargando Contribuyentes CONTPAQI", "Cargando contribuyentes CONTPAQI.");
                               await Task.Delay(1000);

                               LoadClientesContpaq();

                               await progressDialogController.CloseAsync();
                           },
                           () => _applicationConfiguration.SdkInicializado && _applicationConfiguration.EmpresaAbierta && _cargoContribuyentesSat));
            }
        }

        public RelayCommand ExportarContribuyentesContpaqCommand
        {
            get
            {
                return _exportarContribuyentesContpaqCommand ?? (_exportarContribuyentesContpaqCommand = new RelayCommand(async () =>
                           {
                               var messageDialogResult = await _dialogCoordinator.ShowMessageAsync(this,
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
            get { return _copiarRfcContribuyenteSatCommand ?? (_copiarRfcContribuyenteSatCommand = new RelayCommand(() => { Clipboard.SetText(ContribuyenteSatSeleccionado.Rfc); }, () => ContribuyenteSatSeleccionado != null)); }
        }

        public RelayCommand CopiarRfcContribuyenteContpaqCommand
        {
            get { return _copiarRfcContribuyenteContpaqCommand ?? (_copiarRfcContribuyenteContpaqCommand = new RelayCommand(() => { Clipboard.SetText(ContribuyenteContpaqSeleccionado.Rfc); }, () => ContribuyenteContpaqSeleccionado != null)); }
        }

        private void LoadFile()
        {
            Contribuyentes.Clear();

            using (TextReader reader = File.OpenText(_applicationConfiguration.RutaArchivo69B))
            {
                var csvReader = new CsvReader(reader);

                csvReader.Read();
                csvReader.Read();

                while (csvReader.Read())
                {
                    if (string.IsNullOrEmpty(csvReader.GetField(0)))
                    {
                        continue;
                    }

                    var contribuyente = new Contribuyente
                    {
                        Rfc = csvReader.GetField(1),
                        RazonSocial = csvReader.GetField(2),
                        Supuesto = csvReader.GetField(3)
                    };

                    Contribuyentes.Add(contribuyente);
                }

                ContribuyentesCollectionView.Refresh();
                RaisePropertyChanged(nameof(ContribuyentesCount));
            }
        }

        public void LoadClientesContpaq()
        {
            var rep = new ClienteContpaqRepositorio(_applicationConfiguration.ContpaqiSdk);

            List<ContribuyenteContpaq> clientesContpaq;

            if (_applicationConfiguration.SistemaElegido == SistemaContpaqEnum.Contabilidad)
            {
                clientesContpaq = GetClientesContabilidad();
            }
            else
            {
                clientesContpaq = rep.TraerClientes();
            }

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
            if (!(obj is Contribuyente contribuyente))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(Filtro))
            {
                return true;
            }

            return contribuyente.Rfc.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   contribuyente.RazonSocial.IndexOf(Filtro, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool ContribuyentesContpaqCollectionViewFilter(object obj)
        {
            if (!(obj is ContribuyenteContpaq contribuyente))
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(FiltroContpaq))
            {
                return true;
            }

            return contribuyente.Rfc.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0
                   || contribuyente.Codigo.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0
                   || contribuyente.RazonSocial.IndexOf(FiltroContpaq, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string ExportarTxt()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt | .txt";
            saveFileDialog.FileName = "ContribuyentesIncumplidosContpaq.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                var ruta = saveFileDialog.FileName;
                File.WriteAllLines(ruta, ContribuyentesContpaq.Select(c => $"{c.Rfc}|{c.Codigo}|{c.RazonSocial}|{c.Supuesto}"));
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
                    worksheet.Cells[1, 5].Value = "Tipo";
                    worksheet.Cells[1, 6].Value = "Estatus";
                    var i = 2;
                    foreach (var clienteContpaq in ContribuyentesContpaq)
                    {
                        worksheet.Cells[i, 1].Value = clienteContpaq.Rfc;
                        worksheet.Cells[i, 2].Value = clienteContpaq.Codigo;
                        worksheet.Cells[i, 3].Value = clienteContpaq.RazonSocial;
                        worksheet.Cells[i, 4].Value = clienteContpaq.Supuesto;
                        worksheet.Cells[i, 5].Value = clienteContpaq.Tipo;
                        worksheet.Cells[i, 6].Value = clienteContpaq.EstaActivo;
                        i++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    excelpackage.Save();
                }

                return saveFileDialog.FileName;
            }

            return null;
        }

        private List<ContribuyenteContpaq> GetClientesContabilidad()
        {
            var lista = new List<ContribuyenteContpaq>();
            var clientes = new TSdkCliente();
            clientes.setSesion(_applicationConfiguration.SdkSesion);
            var result = clientes.consultaPorRFC_buscaPrimero();
            if (result == 1)
            {
                var contribuyente = new ContribuyenteContpaq();
                contribuyente.Rfc = clientes.RFC;
                contribuyente.RazonSocial = clientes.Nombre;
                contribuyente.Codigo = clientes.Codigo;
                contribuyente.Id = clientes.Id;
                contribuyente.EstaActivo = clientes.EsBaja == 0;
                if (clientes.EsProveedor == 1)
                {
                    contribuyente.Tipo = TipoContribuyenteEnum.ClienteProveedor;
                }
                else
                {
                    contribuyente.Tipo = TipoContribuyenteEnum.Cliente;
                }

                lista.Add(contribuyente);
            }

            while (clientes.consultaPorRFC_buscaSiguiente() == 1)
            {
                var contribuyente = new ContribuyenteContpaq();
                contribuyente.Rfc = clientes.RFC;
                contribuyente.RazonSocial = clientes.Nombre;
                contribuyente.Codigo = clientes.Codigo;
                contribuyente.Id = clientes.Id;
                contribuyente.EstaActivo = clientes.EsBaja == 0;
                if (clientes.EsProveedor == 1)
                {
                    contribuyente.Tipo = TipoContribuyenteEnum.ClienteProveedor;
                }
                else
                {
                    contribuyente.Tipo = TipoContribuyenteEnum.Cliente;
                }

                lista.Add(contribuyente);
            }

            var proveedores = new TSdkProveedor();
            proveedores.setSesion(_applicationConfiguration.SdkSesion);
            proveedores.iniciarInfo();

            proveedores.consultaPorNumero_buscaUltimo();
            var idUltimo = proveedores.Id;
            result = proveedores.consultaPorNumero_buscaPrimero();
            if (result == 1)
            {
                var contribuyente = new ContribuyenteContpaq
                {
                    Rfc = proveedores.RFC,
                    RazonSocial = proveedores.Nombre,
                    Codigo = proveedores.Codigo,
                    Id = proveedores.Id,
                    EstaActivo = proveedores.EsBaja == 0,
                    Tipo = proveedores.EsCliente == 1 ? TipoContribuyenteEnum.ClienteProveedor : TipoContribuyenteEnum.Proveedor
                };

                lista.Add(contribuyente);
            }

            while (proveedores.consultaPorNumero_buscaSiguiente() == 1)
            {
                var contribuyente = new ContribuyenteContpaq
                {
                    Rfc = proveedores.RFC,
                    RazonSocial = proveedores.Nombre,
                    Codigo = proveedores.Codigo,
                    Id = proveedores.Id,
                    EstaActivo = proveedores.EsBaja == 0,
                    Tipo = proveedores.EsCliente == 1 ? TipoContribuyenteEnum.ClienteProveedor : TipoContribuyenteEnum.Proveedor
                };

                lista.Add(contribuyente);
                if (proveedores.Id == idUltimo)
                {
                    break;
                }
            }

            return lista;
        }
    }
}