using System.Runtime.InteropServices;

namespace Contpaqi.ListaNegraSat.WpfApp.Helpers
{
    internal class FloatingPointReset
    {
        public static void Action()
        {
            _fpreset();
        }

        [DllImport("msvcr100.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _fpreset();
    }
}