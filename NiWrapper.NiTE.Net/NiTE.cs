/*
    Copyright (C) 2013 Soroush Falahati - soroush@falahati.net

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
	*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OpenNIWrapper;

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

        [DebuggerStepThrough()]
        internal static void throwIfError(Status status)
        {
            switch (status)
            {
                case Status.ERROR:
                    throw new OpenNIException(OpenNI.LastError);
                case Status.BAD_USER_ID:
                    throw new ArgumentOutOfRangeException(OpenNI.LastError,
                        new Exception("NiTE: Bad User ID"));
                default:
                    return;
            }
        }
    }
}
