using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    internal class Common
    {
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Common_Delete(IntPtr objectHandler);
        public static void DeleteObject(OpenNIBase o)
        {
            Common_Delete(o.Handle);
        }

    }
}
