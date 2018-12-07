using System.Windows;
using Contpaqi.ListaNegraSat.WpfApp.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;

namespace Contpaqi.ListaNegraSat.WpfApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var vm = SimpleIoc.Default.GetInstance<MainWindowViewModel>();
            if (vm.TerminarSdkCommand.CanExecute(null))
            {
                vm.TerminarSdkCommand.Execute(null);
            }

            base.OnExit(e);
        }
    }
}