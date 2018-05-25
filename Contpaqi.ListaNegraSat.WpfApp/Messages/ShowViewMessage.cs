using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Contpaqi.ListaNegraSat.WpfApp.Messages
{
    public class ShowViewMessage : MessageBase
    {
        public ShowViewMessage(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
        }

        public ViewModelBase ViewModel { get; set; }
    }
}