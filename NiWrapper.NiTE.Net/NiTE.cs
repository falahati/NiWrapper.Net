using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    public class NiTE
    {
        [StructLayout(LayoutKind.Sequential)]
        struct OniVersion
        {
            public int major;
            public int minor;
            public int maintenance;
            public int build;
        }

        public enum Status
        {
            OK,
            ERROR,
            BAD_USER_ID,
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OniVersion NiTE_getVersion();
        public static Version Version
        {
            get
            {
                OniVersion ver = NiTE_getVersion();
                return new Version(ver.major, ver.minor, ver.maintenance, ver.build);
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Status NiTE_initialize();
        public static Status Initialize()
        {
            
            return NiTE_initialize();
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void NiTE_shutdown();
        public static void Shutdown()
        {
            NiTE_shutdown();
        }
    }
}
