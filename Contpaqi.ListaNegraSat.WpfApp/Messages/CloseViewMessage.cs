using GalaSoft.MvvmLight.Messaging;

namespace Contpaqi.ListaNegraSat.WpfApp.Messages
{
    public class CloseViewMessage : MessageBase
    {
        public CloseViewMessage(object sender) : base(sender)
        {
        }

        public CloseViewMessage(object sender, object target) : base(sender, target)
        {
        }
    }
}