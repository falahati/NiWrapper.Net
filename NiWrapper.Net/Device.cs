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

namespace OpenNIWrapper
{
    public class Device : OpenNIBase, IDisposable
    {
        public const string ANY_DEVICE = null;

        private Dictionary<SensorType, VideoStream> VideoStreams_Cache = new Dictionary<SensorType, VideoStream>();
        public enum ImageRegistrationMode
        {
            OFF = 0,
            DEPTH_TO_COLOR = 1,
        }

        public enum SensorType
        {
            IR = 1,
            COLOR = 2,
            DEPTH = 3,
        }

        internal Device(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~Device()
        {
            try
            {
                Dispose();
                Common.DeleteObject(this);
            }
            catch (Exception)
            { }
        }

        public VideoStream CreateVideoStream(SensorType sensorType)
        {
            if (!this.IsValid)
                return null;
            if (!VideoStreams_Cache.ContainsKey(sensorType))
                VideoStreams_Cache[sensorType] = VideoStream.Private_Create(this, sensorType);
            return VideoStreams_Cache[sensorType];
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Device_close(IntPtr objectHandler);
        public void Close()
        {
            if (this.IsValid)
                foreach (VideoStream stream in VideoStreams_Cache.Values)
                    stream.Destroy();
            Device_close(this.Handle);
            base.Handle = IntPtr.Zero;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_open(out IntPtr objectHandler, IntPtr uri);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device__openEx(out IntPtr objectHandler, IntPtr uri, IntPtr mode);
        public static Device Open(string uri, string mode = "")
        {
            IntPtr handle;
            if (mode != "")
                OpenNI.throwIfError(Device__openEx(out handle, Marshal.StringToHGlobalAnsi(uri), Marshal.StringToHGlobalAnsi(mode)));
            else
                OpenNI.throwIfError(Device_open(out handle, Marshal.StringToHGlobalAnsi(uri)));
            return new Device(handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr Device_getDeviceInfo(IntPtr objectHandler);
        DeviceInfo _DeviceInfo;
        public DeviceInfo DeviceInfo
        {
            get
            {
                if (_DeviceInfo != null) return _DeviceInfo;
                _DeviceInfo = new DeviceInfo(Device_getDeviceInfo(this.Handle));
                return _DeviceInfo;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr Device_getPlaybackControl(IntPtr objectHandler);
        PlaybackControl _PlaybackControl;
        public PlaybackControl PlaybackControl
        {
            get
            {
                if (_PlaybackControl != null) return _PlaybackControl;
                _PlaybackControl = new PlaybackControl(Device_getPlaybackControl(this.Handle));
                return _PlaybackControl;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_isImageRegistrationModeSupported(IntPtr objectHandler, ImageRegistrationMode mode);
        public bool IsImageRegistrationModeSupported(ImageRegistrationMode mode)
        {
            return Device_isImageRegistrationModeSupported(this.Handle, mode);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_getProperty(IntPtr objectHandler,
            int propertyId, IntPtr data, out int dataSize);
        public OpenNI.Status GetProperty(int propertyId, IntPtr data, out int dataSize)
        {
            return Device_getProperty(this.Handle, propertyId, data, out dataSize);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr Device_getSensorInfo(IntPtr objectHandler, SensorType sensorType);
        public SensorInfo GetSensorInfo(SensorType sensorType)
        {
            return new SensorInfo(Device_getSensorInfo(this.Handle, sensorType));
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_setProperty(IntPtr objectHandler,
            int propertyId, IntPtr data, int dataSize);
        public OpenNI.Status SetProperty(int propertyId, IntPtr data, int dataSize)
        {
            return Device_setProperty(this.Handle, propertyId, data, dataSize);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_hasSensor(IntPtr objectHandler, SensorType sensorType);
        public bool HasSensor(SensorType sensorType)
        {
            return Device_hasSensor(this.Handle, sensorType);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_isCommandSupported(IntPtr objectHandler, int commandId);
        public bool IsCommandSupported(int commandId)
        {
            return Device_isCommandSupported(this.Handle, commandId);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_invoke(IntPtr objectHandler,
            int commandId, IntPtr data, int dataSize);
        public OpenNI.Status Invoke(int commandId, IntPtr data, int dataSize)
        {
            return Device_invoke(this.Handle, commandId, data, dataSize);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_isPropertySupported(IntPtr objectHandler, int propertyId);
        public bool IsPropertySupported(int propertyId)
        {
            return Device_isPropertySupported(this.Handle, propertyId);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern ImageRegistrationMode Device_getImageRegistrationMode(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_setImageRegistrationMode(IntPtr objectHandler, ImageRegistrationMode mode);
        public ImageRegistrationMode ImageRegistration
        {
            get
            {
                return Device_getImageRegistrationMode(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(Device_setImageRegistrationMode(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_getDepthColorSyncEnabled(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Device_setDepthColorSyncEnabled(IntPtr objectHandler, bool enable);
        public bool DepthColorSyncEnabled
        {
            get
            {
                return Device_getDepthColorSyncEnabled(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(Device_setDepthColorSyncEnabled(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_isFile(IntPtr objectHandler);
        bool? _isFile = null;
        public bool isFile
        {
            get
            {
                if (_isFile != null) return _isFile.Value;
                _isFile = Device_isFile(this.Handle);
                return _isFile.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Device_isValid(IntPtr objectHandler);
        public new bool IsValid
        {
            get
            {
                return base.IsValid && Device_isValid(this.Handle);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && this.IsValid)
                    this.Close();

                this.Handle = IntPtr.Zero;
                _disposed = true;
            }
        }

        public override string ToString()
        {
            return this.DeviceInfo.Name;
        }
    }
}
