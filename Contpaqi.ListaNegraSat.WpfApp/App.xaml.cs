using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Contpaqi.ListaNegraSat.WpfApp.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace Contpaqi.ListaNegraSat.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Overrides of Application

        protected override void OnExit(ExitEventArgs e)
        {
            var vm = SimpleIoc.Default.GetInstance<MainWindowViewModel>();
            if (vm.TerminarSdkCommand.CanExecute(null))
            {
                vm.TerminarSdkCommand.Execute(null);
            }

            base.OnExit(e);
        }

        #endregion
    }
}
