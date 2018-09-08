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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    #region

    #endregion

    // ReSharper disable once InconsistentNaming
    public static class OpenNI
    {
        #region Constants

        public const int TimeoutForever = -1;

        public const int TimeoutNone = 0;

        #endregion

        #region Static Fields

        private static readonly DeviceConnectionStateChangedDelegate InternalDeviceConnect = PrivateDeviceConnect;

        private static readonly DeviceConnectionStateChangedDelegate InternalDeviceDisconnect = PrivateDeviceDisconnect;

        private static readonly DeviceStateChangedDelegate InternalDeviceStateChanged = PrivateDeviceStateChanged;

        // ReSharper disable once NotAccessedField.Local

#pragma warning disable 414
        private static IntPtr handlerEvents;
#pragma warning restore 414

        #endregion

        #region Delegates

        public delegate void DeviceConnectionStateChanged(DeviceInfo device);

        public delegate void DeviceStateChanged(DeviceInfo device, DeviceState state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DeviceConnectionStateChangedDelegate(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DeviceStateChangedDelegate(IntPtr device, DeviceState state);

        #endregion

        #region Public Events

        public static event DeviceConnectionStateChanged OnDeviceConnected;

        public static event DeviceConnectionStateChanged OnDeviceDisconnected;

        public static event DeviceStateChanged OnDeviceStateChanged;

        #endregion

        #region Enums

        public enum DeviceState
        {
            Ok = 0,

            Error = 1,

            NotReady = 2,

            Eof = 3
        }

        public enum Status
        {
            Ok = 0,

            Error = 1,

            NotImplemented = 2,

            NotSupported = 3,

            BadParameter = 4,

            OutOfFlow = 5,

            NoDevice = 6,

            TimeOut = 102
        }

        #endregion

        #region Public Properties

        public static string LastError
        {
            get
            {
                var e = OpenNI_getExtendedError();
                var errorString = Marshal.PtrToStringAnsi(e);

                if (errorString == null)
                {
                    return null;
                }

                var r = (string) errorString.Clone();

                return r;
            }
        }

        public static Version Version
        {
            get
            {
                var ver = OpenNI_getVersion();

                return new Version(ver.Major, ver.Minor, ver.Maintenance, ver.Build);
            }
        }

        #endregion

        #region Public Methods and Operators

        public static DeviceInfo[] EnumerateDevices()
        {
            var csa = OpenNI_enumerateDevices();
            var array = new IntPtr[csa.Size];
            Marshal.Copy(csa.Data, array, 0, csa.Size);
            var arrayObjects = new DeviceInfo[csa.Size];

            for (var i = 0; i < csa.Size; i++)
            {
                arrayObjects[i] = new DeviceInfo(array[i]);
            }

            OpenNI_destroyDevicesArray(csa);

            return arrayObjects;
        }

        public static Status Initialize()
        {
            var ret = OpenNI_initialize();

            if (ret == Status.Ok)
            {
                handlerEvents = OpenNI_RegisterListener(
                    InternalDeviceConnect,
                    InternalDeviceDisconnect,
                    InternalDeviceStateChanged);
            }

            return ret;
        }

        public static void Shutdown()
        {
            OpenNI_shutdown();
        }

        public static Status WaitForAnyStream(
            VideoStream[] streams,
            out VideoStream readyStream,
            int timeout = TimeoutForever)
        {
            readyStream = null;
            var streamArray = new IntPtr[streams.Length];

            var i = 0;

            foreach (var vs in streams)
            {
                streamArray[i] = vs.Handle;
                i++;
            }

            var selectedId = -1;
            Status returnValue;
            var arrayPointer = Marshal.AllocHGlobal(IntPtr.Size * streamArray.Length);

            try
            {
                Marshal.Copy(streamArray, 0, arrayPointer, streamArray.Length);
                returnValue = OpenNI_waitForAnyStream(arrayPointer, streamArray.Length, ref selectedId, timeout);
            }
            finally
            {
                Marshal.FreeHGlobal(arrayPointer);
            }

            if (returnValue == Status.Ok)
            {
                foreach (var vs in streams)
                {
                    if (vs.Equals(streamArray[selectedId]))
                    {
                        readyStream = vs;
                    }
                }
            }

            return returnValue;
        }

        public static Status WaitForStream(VideoStream streams, int timeout = TimeoutForever)
        {
            VideoStream vs;
            var returnValue = WaitForAnyStream(new[] {streams}, out vs, timeout);

            if (returnValue == Status.Ok && !vs.Equals(streams))
            {
                return Status.Error;
            }

            return returnValue;
        }

        #endregion

        #region Methods

        [DebuggerStepThrough]
        internal static void ThrowIfError(Status status)
        {
            switch (status)
            {
                case Status.Error:

                    throw new OpenNIException(LastError);
                case Status.NotImplemented:

                    throw new NotImplementedException(LastError);
                case Status.NotSupported:

                    throw new NotSupportedException(LastError);
                case Status.BadParameter:

                    throw new ArgumentException(LastError);
                case Status.OutOfFlow:

                    throw new OverflowException(LastError);
                case Status.NoDevice:

                    throw new IOException(LastError);
                case Status.TimeOut:

                    throw new TimeoutException(LastError);
                default:

                    return;
            }
        }

        private static void PrivateDeviceConnect(IntPtr device)
        {
            var ev = OnDeviceConnected;

            if (ev != null)
            {
                ev(new DeviceInfo(device));
            }
        }

        private static void PrivateDeviceDisconnect(IntPtr device)
        {
            var ev = OnDeviceDisconnected;

            if (ev != null)
            {
                ev(new DeviceInfo(device));
            }
        }

        private static void PrivateDeviceStateChanged(IntPtr device, DeviceState state)
        {
            var ev = OnDeviceStateChanged;

            if (ev != null)
            {
                ev(new DeviceInfo(device), state);
            }
        }

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenNI_RegisterListener(
            [MarshalAs(UnmanagedType.FunctionPtr)] DeviceConnectionStateChangedDelegate connect,
            [MarshalAs(UnmanagedType.FunctionPtr)] DeviceConnectionStateChangedDelegate disconnect,
            [MarshalAs(UnmanagedType.FunctionPtr)] DeviceStateChangedDelegate statechanged);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenNI_destroyDevicesArray(WrapperArray array);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrapperArray OpenNI_enumerateDevices();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr OpenNI_getExtendedError();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OniVersion OpenNI_getVersion();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern Status OpenNI_initialize();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void OpenNI_shutdown();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern Status OpenNI_waitForAnyStream(
            IntPtr streams,
            int streamCount,
            ref int readyStreamIndex,
            int timeout);

        #endregion
    }
}