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

namespace OpenNIWrapper
{
    public class OpenNI
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void deviceConnectionStateChanged(IntPtr Device);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void deviceStateChanged(IntPtr Device, DeviceState state);

        public delegate void DeviceConnectionStateChanged(DeviceInfo Device);
        public delegate void DeviceStateChanged(DeviceInfo Device, DeviceState state);

        private static deviceConnectionStateChanged internal_DeviceConnect = new deviceConnectionStateChanged(Internal_DeviceConnect);
        private static deviceConnectionStateChanged internal_DeviceDisconnect = new deviceConnectionStateChanged(Internal_DeviceDisconnect);
        private static deviceStateChanged internal_DeviceStateChanged = new deviceStateChanged(Internal_DeviceStateChanged);

        public static event DeviceConnectionStateChanged onDeviceConnected;
        public static event DeviceConnectionStateChanged onDeviceDisconnected;
        public static event DeviceStateChanged onDeviceStateChanged;

        public const int TIMEOUT_FOREVER = -1;
        public const int TIMEOUT_NONE = 0;

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
            OK = 0,
            ERROR = 1,
            NOT_IMPLEMENTED = 2,
            NOT_SUPPORTED = 3,
            BAD_PARAMETER = 4,
            OUT_OF_FLOW = 5,
            NO_DEVICE = 6,
            TIME_OUT = 102,
        }

        public enum DeviceState
        {
            DEVICE_STATE_OK 	= 0,
	        DEVICE_STATE_ERROR 	= 1,
	        DEVICE_STATE_NOT_READY 	= 2,
	        DEVICE_STATE_EOF 	= 3,
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OniVersion OpenNI_getVersion();
        public static Version Version
        {
            get
            {
                OniVersion ver = OpenNI_getVersion();
                return new Version(ver.major, ver.minor, ver.maintenance, ver.build);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OpenNI_getExtendedError();
        public static String LastError
        {
            get
            {
                IntPtr e = OpenNI_getExtendedError();
                string r = (string)Marshal.PtrToStringAnsi(e).Clone();
                //Marshal.FreeBSTR(e);
                return r;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Status OpenNI_initialize();
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OpenNI_RegisterListener(
            [MarshalAs(UnmanagedType.FunctionPtr)]deviceConnectionStateChanged connect,
            [MarshalAs(UnmanagedType.FunctionPtr)]deviceConnectionStateChanged disconnect,
            [MarshalAs(UnmanagedType.FunctionPtr)]deviceStateChanged statechanged);
        static IntPtr handler_events;
        public static Status Initialize()
        {
            Status ret = OpenNI_initialize();
            if (ret == Status.OK)
            {
                handler_events = OpenNI_RegisterListener(internal_DeviceConnect,
                    internal_DeviceDisconnect, internal_DeviceStateChanged);
            }
            return ret;
        }

        private static void Internal_DeviceConnect(IntPtr device)
        {
            DeviceConnectionStateChanged ev = onDeviceConnected;
            if (ev != null)
                ev(new DeviceInfo(device));
        }

        private static void Internal_DeviceDisconnect(IntPtr device)
        {
            DeviceConnectionStateChanged ev = onDeviceDisconnected;
            if (ev != null)
                ev(new DeviceInfo(device));
        }

        private static void Internal_DeviceStateChanged(IntPtr device, DeviceState state)
        {
            DeviceStateChanged ev = onDeviceStateChanged;
            if (ev != null)
                ev(new DeviceInfo(device), state);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void OpenNI_shutdown();
        public static void Shutdown()
        {
            OpenNI_shutdown();
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern WrapperArray OpenNI_enumerateDevices();
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr OpenNI_destroyDevicesArray(WrapperArray array);
        public static DeviceInfo[] EnumerateDevices()
        {
            WrapperArray csa = OpenNI_enumerateDevices();
            IntPtr[] array = new IntPtr[csa.Size];
            Marshal.Copy(csa.Data, array, 0, csa.Size);
            DeviceInfo[] arrayObjects = new DeviceInfo[csa.Size];
            for (int i = 0; i < csa.Size; i++)
                arrayObjects[i] = new DeviceInfo(array[i]);
            OpenNI_destroyDevicesArray(csa);
            return arrayObjects;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Status OpenNI_waitForAnyStream(IntPtr pStreams, int streamCount, ref int pReadyStreamIndex, int timeout);
        public static Status WaitForAnyStream(VideoStream[] pStreams, out VideoStream pReadyStream, int timeout = OpenNI.TIMEOUT_FOREVER)
        {
            pReadyStream = null;
            IntPtr[] pStreamArray = new IntPtr[pStreams.Length];
            
            int i = 0;
            foreach (VideoStream vs in pStreams)
            {
                pStreamArray[i] = vs.Handle;
                i++;
            }
            int selectedId = -1;
            Status ret = Status.ERROR;
            IntPtr arrayPointer = Marshal.AllocHGlobal(IntPtr.Size * pStreamArray.Length);
            try
            {
                Marshal.Copy(pStreamArray, 0, arrayPointer, pStreamArray.Length);
                ret = OpenNI_waitForAnyStream(arrayPointer, pStreamArray.Length, ref selectedId, timeout);
            }
            finally
            {
                Marshal.FreeHGlobal(arrayPointer);
            }
             
            if (ret == Status.OK)
                foreach (VideoStream vs in pStreams)
                    if (vs.Equals(pStreamArray[selectedId]))
                        pReadyStream = vs;
            return ret;
        }

        public static Status WaitForStream(VideoStream pStreams, int timeout = OpenNI.TIMEOUT_FOREVER)
        {
            VideoStream vs;
            Status ret = WaitForAnyStream(new VideoStream[] { pStreams }, out vs, timeout);
            if (ret == Status.OK && !vs.Equals(pStreams))
                return Status.ERROR;
            return ret;
        }

        [DebuggerStepThrough()]
        internal static void throwIfError(Status status)
        {
            switch (status)
            {
                case Status.ERROR:
                    throw new Exception(OpenNI.LastError);
                case Status.NOT_IMPLEMENTED:
                    throw new NotImplementedException(OpenNI.LastError);
                case Status.NOT_SUPPORTED:
                    throw new NotSupportedException(OpenNI.LastError);
                case Status.BAD_PARAMETER:
                    throw new ArgumentException(OpenNI.LastError);
                case Status.OUT_OF_FLOW:
                    throw new OverflowException(OpenNI.LastError);
                case Status.NO_DEVICE:
                    throw new System.IO.IOException(OpenNI.LastError);
                case Status.TIME_OUT:
                    throw new TimeoutException(OpenNI.LastError);
                default:
                    return;
            }
        }
    }
}
