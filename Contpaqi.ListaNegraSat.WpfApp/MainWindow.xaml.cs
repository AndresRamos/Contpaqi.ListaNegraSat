using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Contpaqi.ListaNegraSat.WpfApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;

namespace Contpaqi.ListaNegraSat.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<ShowViewMessage>(this, message =>
            {
                OpenView(message.ViewModel);
            });
        }

        private void OpenView(ViewModelBase viewModel)
        {
            var window = new MetroWindow();
            window.Content = viewModel;
            Messenger.Default.Register<CloseViewMessage>(window, message =>
            {
                if (message.Sender == viewModel)
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
