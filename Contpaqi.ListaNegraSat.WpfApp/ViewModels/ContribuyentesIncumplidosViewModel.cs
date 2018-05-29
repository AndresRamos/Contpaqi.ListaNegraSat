using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization.Advanced;
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

        public ContribuyentesIncumplidosViewModel(IDialogCoordinator dialogCoordinator,
            ApplicationConfiguration applicationConfiguration,
            Articulo69ViewModel articulo69ViewModel,
            Articulo69BViewModel articulo69BViewModel)
        {
            _dialogCoordinator = dialogCoordinator;
            _applicationConfiguration = applicationConfiguration;
            _articulo69ViewModel = articulo69ViewModel;
            _articulo69BViewModel = articulo69BViewModel;
        }

        private ViewModelBase _articulo69BViewModel;

        public ViewModelBase Articulo69BViewModel
        {
            get => _articulo69BViewModel;
            set => Set(() => Articulo69BViewModel, ref _articulo69BViewModel, value);
        }

        private ViewModelBase _articulo69ViewModel;

        public ViewModelBase Articulo69ViewModel
        {
            get => _articulo69ViewModel;
            set => Set(() => Articulo69ViewModel, ref _articulo69ViewModel, value);
        }
    }
}