﻿using Contpaqi.ListaNegraSat.WpfApp.Models;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register(() => new ApplicationConfiguration());
            SimpleIoc.Default.Register(() => DialogCoordinator.Instance);
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<SeleccionarEmpresaViewModel>();
            SimpleIoc.Default.Register<ContribuyentesIncumplidosViewModel>();
        }

        public MainWindowViewModel MainWindowVm => SimpleIoc.Default.GetInstance<MainWindowViewModel>();

        public ApplicationConfiguration ApplicationConfiguration => SimpleIoc.Default.GetInstance<ApplicationConfiguration>();
    }
}