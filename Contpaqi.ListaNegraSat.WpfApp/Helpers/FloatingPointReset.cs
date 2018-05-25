using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
