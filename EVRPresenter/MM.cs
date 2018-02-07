using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace EVRPresenter
{
    class Winmm
    {
        [DllImport("Winmm.dll", PreserveSig = false)]
        public extern static int timeBeginPeriod(int x);

        [DllImport("Winmm.dll", PreserveSig = false)]
        public extern static int timeEndPeriod(int uPeriod);

    }
}
