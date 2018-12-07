using System.Windows;
using Contpaqi.ListaNegraSat.WpfApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;

namespace Contpaqi.ListaNegraSat.WpfApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<ShowViewMessage>(this, message => { OpenView(message.ViewModel); });
        }

        private void OpenView(ViewModelBase viewModel)
        {
            var window = new MetroWindow {Content = viewModel};
            Messenger.Default.Register<CloseViewMessage>(window,
                message =>
                {
                    if (message.Sender == window.Content as ViewModelBase)
                    {
                        window.Close();
                    }
                });
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = this;
            window.ShowDialog();
        }
    }
}