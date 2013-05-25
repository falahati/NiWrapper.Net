using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    public class Common
    {
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Common_Delete(IntPtr objectHandler);
        public static void DeleteObject(NiTEBase o)
        {
            Common_Delete(o.Handle);
        }
    }
}
