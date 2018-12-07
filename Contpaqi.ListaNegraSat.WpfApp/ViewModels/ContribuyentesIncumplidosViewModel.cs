using Contpaqi.ListaNegraSat.WpfApp.Models;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class ContribuyentesIncumplidosViewModel : ViewModelBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        private readonly IDialogCoordinator _dialogCoordinator;
        private ViewModelBase _articulo69BViewModel;
        private ViewModelBase _articulo69ViewModel;

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

        public ViewModelBase Articulo69BViewModel
        {
            get => _articulo69BViewModel;
            set => Set(() => Articulo69BViewModel, ref _articulo69BViewModel, value);
        }

        public ViewModelBase Articulo69ViewModel
        {
            get => _articulo69ViewModel;
            set => Set(() => Articulo69ViewModel, ref _articulo69ViewModel, value);
        }
    }
}