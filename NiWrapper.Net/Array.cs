using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WrapperArray
    {
        public int Size;
        public IntPtr Data;
        public IntPtr Handle;
    }
}
