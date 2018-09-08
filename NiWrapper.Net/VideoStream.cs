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
using System.Drawing;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    #region

    #endregion

    public class VideoStream : OpenNIBase, IDisposable
    {
        #region Public Events

        public event VideoStreamNewFrame OnNewFrame;

        #endregion

        #region Constructors and Destructors

        internal VideoStream(IntPtr handle)
        {
            ParentDevice = null;
            Handle = handle;
            internalNewFrame = PrivateNewFrame;
        }

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsValid)
            {
                VideoStream_destroy(Handle);
                Common.DeleteObject(this);
                Handle = IntPtr.Zero;
            }
        }

        #region Fields

        private readonly VideoStreamNewFrameDelegate internalNewFrame;

        private CameraSettings cameraSettings;

        // ReSharper disable once NotAccessedField.Local
        // Keeping event address
#pragma warning disable 414
        private IntPtr handlerEvents;
#pragma warning restore 414

        private bool isStarted;

        #endregion

        #region Delegates

        public delegate void VideoStreamNewFrame(VideoStream vStream);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VideoStreamNewFrameDelegate(IntPtr vStream);

        #endregion

        #region Public Properties

        public CameraSettings CameraSettings
        {
            get
            {
                if (cameraSettings != null)
                {
                    return cameraSettings;
                }

                cameraSettings = new CameraSettings(VideoStream_getCameraSettings(Handle));

                return cameraSettings;
            }
        }

        public Rectangle? Cropping
        {
            get
            {
                int x = 0, y = 0, w = 0, h = 0;

                if (VideoStream_getCropping(Handle, ref x, ref y, ref w, ref h))
                {
                    return new Rectangle(x, y, w, h);
                }

                return null;
            }

            set
            {
                if (value == null)
                {
                    OpenNI.ThrowIfError(VideoStream_resetCropping(Handle));
                }
                else
                {
                    OpenNI.ThrowIfError(
                        VideoStream_setCropping(
                            Handle,
                            value.Value.X,
                            value.Value.Y,
                            value.Value.Width,
                            value.Value.Height));
                }
            }
        }

        public float HorizontalFieldOfView
        {
            get => VideoStream_getHorizontalFieldOfView(Handle);
        }

        public bool IsCroppingSupported
        {
            get => VideoStream_isCroppingSupported(Handle);
        }

        public new bool IsValid
        {
            get => base.IsValid && VideoStream_isValid(Handle);
        }

        public int MaxPixelValue
        {
            get => VideoStream_getMaxPixelValue(Handle);
        }

        public int MinPixelValue
        {
            get => VideoStream_getMinPixelValue(Handle);
        }

        public bool Mirroring
        {
            get => VideoStream_getMirroringEnabled(Handle);

            set => OpenNI.ThrowIfError(VideoStream_setMirroringEnabled(Handle, value));
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Device ParentDevice { get; private set; }

        public SensorInfo SensorInfo
        {
            get => new SensorInfo(VideoStream_getSensorInfo(Handle));
        }

        public float VerticalFieldOfView
        {
            get => VideoStream_getVerticalFieldOfView(Handle);
        }

        public VideoMode VideoMode
        {
            get => new VideoMode(VideoStream_getVideoMode(Handle), false);

            set => OpenNI.ThrowIfError(VideoStream_setVideoMode(Handle, value.Handle));
        }

        #endregion

        #region Public Methods and Operators

        public static VideoStream Create(Device device, Device.SensorType sensorType)
        {
            return device.CreateVideoStream(sensorType);
        }

        public OpenNI.Status Invoke(int commandId, IntPtr data, int dataSize)
        {
            return VideoStream_invoke(Handle, commandId, data, dataSize);
        }

        public bool IsCommandSupported(int commandId)
        {
            return VideoStream_isCommandSupported(Handle, commandId);
        }

        public bool IsPropertySupported(int propertyId)
        {
            return VideoStream_isPropertySupported(Handle, propertyId);
        }

        public OpenNI.Status Start()
        {
            var status = VideoStream_start(Handle);
            isStarted = status == OpenNI.Status.Ok;

            return status;
        }

        public void Stop()
        {
            isStarted = false;
            VideoStream_stop(Handle);
        }

        public override string ToString()
        {
            return SensorInfo.GetSensorType().ToString();
        }

        public OpenNI.Status GetProperty(int propertyId, IntPtr data, out int dataSize)
        {
            return VideoStream_getProperty(Handle, propertyId, data, out dataSize);
        }

        public bool IsFrameAvailable()
        {
            return OpenNI.WaitForStream(this, OpenNI.TimeoutNone) == OpenNI.Status.Ok;
        }

        public VideoFrameRef ReadFrame()
        {
            IntPtr newFrame;
            OpenNI.ThrowIfError(VideoStream_readFrame(Handle, out newFrame));

            return new VideoFrameRef(newFrame);
        }

        public OpenNI.Status SetProperty(int propertyId, IntPtr data, int dataSize)
        {
            return VideoStream_setProperty(Handle, propertyId, data, dataSize);
        }

        #endregion

        #region Methods

        internal static VideoStream InternalCreate(Device device, Device.SensorType sensorType)
        {
            IntPtr handle;
            OpenNI.ThrowIfError(VideoStream_create(out handle, device.Handle, sensorType));
            var vs = new VideoStream(handle) {ParentDevice = device};
            vs.handlerEvents = VideoStream_RegisterListener(handle, vs.internalNewFrame);

            return vs;
        }

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoStream_RegisterListener(
            IntPtr objectHandler,
            [MarshalAs(UnmanagedType.FunctionPtr)] VideoStreamNewFrameDelegate newFrame);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_create(
            out IntPtr objectHandler,
            IntPtr device,
            Device.SensorType sensorType);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoStream_destroy(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoStream_getCameraSettings(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_getCropping(
            IntPtr objectHandler,
            ref int originX,
            ref int originY,
            ref int width,
            ref int height);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern float VideoStream_getHorizontalFieldOfView(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoStream_getMaxPixelValue(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoStream_getMinPixelValue(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_getMirroringEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_getProperty(
            IntPtr objectHandler,
            int propertyId,
            IntPtr data,
            out int dataSize);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoStream_getSensorInfo(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern float VideoStream_getVerticalFieldOfView(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoStream_getVideoMode(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_invoke(
            IntPtr objectHandler,
            int commandId,
            IntPtr data,
            int dataSize);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_isCommandSupported(IntPtr objectHandler, int commandId);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_isCroppingSupported(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_isPropertySupported(IntPtr objectHandler, int propertyId);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoStream_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_readFrame(IntPtr objectHandler, out IntPtr newFrame);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_resetCropping(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_setCropping(
            IntPtr objectHandler,
            int originX,
            int originY,
            int width,
            int height);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_setMirroringEnabled(IntPtr objectHandler, bool isEnable);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_setProperty(
            IntPtr objectHandler,
            int propertyId,
            IntPtr data,
            int dataSize);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_setVideoMode(IntPtr objectHandler, IntPtr vmod);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status VideoStream_start(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoStream_stop(IntPtr objectHandler);

        private void PrivateNewFrame(IntPtr stream)
        {
            var ev = OnNewFrame;

            if (ev != null && isStarted)
            {
                ev(this);
            }
        }

        #endregion
    }
}