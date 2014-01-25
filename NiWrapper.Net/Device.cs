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
namespace OpenNIWrapper
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    #endregion

    public class Device : OpenNIBase, IDisposable
    {
        #region Constants

        public const string AnyDevice = null;

        #endregion

        #region Fields

        private readonly Dictionary<SensorType, VideoStream> videoStreamsCache =
            new Dictionary<SensorType, VideoStream>();

        private DeviceInfo deviceInfo;

        private PlaybackControl playbackControl;

        private bool isDisposed;

        private bool? isFile;

        #endregion

        #region Constructors and Destructors

        internal Device(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~Device()
        {
            try
            {
                this.Dispose();
                Common.DeleteObject(this);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Enums

        public enum ImageRegistrationMode
        {
            Off = 0, 
            DepthToColor = 1, 
        }

        public enum SensorType
        {
            Ir = 1, 
            Color = 2, 
            Depth = 3, 
        }

        #endregion

        #region Public Properties

        public bool DepthColorSyncEnabled
        {
            get
            {
                return Device_getDepthColorSyncEnabled(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(Device_setDepthColorSyncEnabled(this.Handle, value));
            }
        }

        public DeviceInfo DeviceInfo
        {
            get
            {
                if (this.deviceInfo != null)
                {
                    return this.deviceInfo;
                }

                this.deviceInfo = new DeviceInfo(Device_getDeviceInfo(this.Handle));
                return this.deviceInfo;
            }
        }

        public ImageRegistrationMode ImageRegistration
        {
            get
            {
                return Device_getImageRegistrationMode(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(Device_setImageRegistrationMode(this.Handle, value));
            }
        }

        public new bool IsValid
        {
            get
            {
                return base.IsValid && Device_isValid(this.Handle);
            }
        }

        public PlaybackControl PlaybackControl
        {
            get
            {
                if (this.playbackControl != null)
                {
                    return this.playbackControl;
                }

                this.playbackControl = new PlaybackControl(Device_getPlaybackControl(this.Handle));
                return this.playbackControl;
            }
        }

        public bool IsFile
        {
            get
            {
                if (this.isFile != null)
                {
                    return this.isFile.Value;
                }

                this.isFile = Device_isFile(this.Handle);
                return this.isFile.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static Device Open(string uri, string mode = "")
        {
            IntPtr handle;
            OpenNI.ThrowIfError(
                mode != string.Empty
                    ? Device__openEx(out handle, Marshal.StringToHGlobalAnsi(uri), Marshal.StringToHGlobalAnsi(mode))
                    : Device_open(out handle, Marshal.StringToHGlobalAnsi(uri)));

            return new Device(handle);
        }

        public void Close()
        {
            if (this.IsValid)
            {
                foreach (VideoStream stream in this.videoStreamsCache.Values)
                {
                    stream.Destroy();
                }
            }

            Device_close(this.Handle);
            this.Handle = IntPtr.Zero;
        }

        public VideoStream CreateVideoStream(SensorType sensorType)
        {
            if (!this.IsValid)
            {
                return null;
            }

            if (!this.videoStreamsCache.ContainsKey(sensorType))
            {
                this.videoStreamsCache[sensorType] = VideoStream.InternalCreate(this, sensorType);
            }

            return this.videoStreamsCache[sensorType];
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public OpenNI.Status GetProperty(int propertyId, IntPtr data, out int dataSize)
        {
            return Device_getProperty(this.Handle, propertyId, data, out dataSize);
        }

        public SensorInfo GetSensorInfo(SensorType sensorType)
        {
            return new SensorInfo(Device_getSensorInfo(this.Handle, sensorType));
        }

        public bool HasSensor(SensorType sensorType)
        {
            return Device_hasSensor(this.Handle, sensorType);
        }

        public OpenNI.Status Invoke(int commandId, IntPtr data, int dataSize)
        {
            return Device_invoke(this.Handle, commandId, data, dataSize);
        }

        public bool IsCommandSupported(int commandId)
        {
            return Device_isCommandSupported(this.Handle, commandId);
        }

        public bool IsImageRegistrationModeSupported(ImageRegistrationMode mode)
        {
            return Device_isImageRegistrationModeSupported(this.Handle, mode);
        }

        public bool IsPropertySupported(int propertyId)
        {
            return Device_isPropertySupported(this.Handle, propertyId);
        }

        public OpenNI.Status SetProperty(int propertyId, IntPtr data, int dataSize)
        {
            return Device_setProperty(this.Handle, propertyId, data, dataSize);
        }

        public override string ToString()
        {
            return this.DeviceInfo.Name;
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing && this.IsValid)
                {
                    this.Close();
                }

                this.Handle = IntPtr.Zero;
                this.isDisposed = true;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device__openEx(out IntPtr objectHandler, IntPtr uri, IntPtr mode);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Device_close(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_getDepthColorSyncEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Device_getDeviceInfo(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ImageRegistrationMode Device_getImageRegistrationMode(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Device_getPlaybackControl(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_getProperty(
            IntPtr objectHandler, 
            int propertyId, 
            IntPtr data, 
            out int dataSize);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Device_getSensorInfo(IntPtr objectHandler, SensorType sensorType);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_hasSensor(IntPtr objectHandler, SensorType sensorType);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_invoke(
            IntPtr objectHandler, 
            int commandId, 
            IntPtr data, 
            int dataSize);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_isCommandSupported(IntPtr objectHandler, int commandId);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_isFile(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_isImageRegistrationModeSupported(
            IntPtr objectHandler, 
            ImageRegistrationMode mode);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_isPropertySupported(IntPtr objectHandler, int propertyId);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Device_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_open(out IntPtr objectHandler, IntPtr uri);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_setDepthColorSyncEnabled(IntPtr objectHandler, bool enable);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_setImageRegistrationMode(
            IntPtr objectHandler, 
            ImageRegistrationMode mode);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Device_setProperty(
            IntPtr objectHandler, 
            int propertyId, 
            IntPtr data, 
            int dataSize);

        #endregion
    }
}