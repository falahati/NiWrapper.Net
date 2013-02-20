using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;

namespace OpenNIWrapper
{
    public class VideoStream : OpenNIBase
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void videoStreamNewFrame(IntPtr vStream);
        public delegate void VideoStreamNewFrame(VideoStream vStream);
        private videoStreamNewFrame internal_NewFrame;
        public event VideoStreamNewFrame onNewFrame;
        internal VideoStream(IntPtr handle)
        {
            ParentDevice = null;
            this.Handle = handle;
            this.internal_NewFrame = new videoStreamNewFrame(this.Internal_NewFrame);
        }

        public Device ParentDevice { get; private set; }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoStream_destroy(IntPtr objectHandler);
        internal void Destroy()
        {
            VideoStream_destroy(this.Handle);
            Common.DeleteObject(this);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_create(out IntPtr objectHandler, IntPtr device, Device.SensorType sensorType);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoStream_RegisterListener(
            IntPtr objectHandler, [MarshalAs(UnmanagedType.FunctionPtr)]videoStreamNewFrame newFrame);
        IntPtr handler_events;
        internal static VideoStream Private_Create(Device device, Device.SensorType sensorType)
        {
            IntPtr handle;
            OpenNI.throwIfError(VideoStream_create(out handle, device.Handle, sensorType));
            VideoStream vs = new VideoStream(handle);
            vs.ParentDevice = device;
            vs.handler_events = VideoStream_RegisterListener(handle, vs.internal_NewFrame);
            return vs;
        }

        public static VideoStream Create(Device device, Device.SensorType sensorType)
        {
            return device.CreateVideoStream(sensorType);
        }

        private void Internal_NewFrame(IntPtr vStream)
        {
            VideoStreamNewFrame ev = onNewFrame;
            if (ev != null)
                //if (this.Equals(vStream))
                    ev(this);
                //else
                //  ev(new VideoStream(vStream));
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoStream_getCameraSettings(IntPtr objectHandler);
        CameraSettings _CameraSettings = null;
        public CameraSettings CameraSettings
        {
            get
            {
                if (_CameraSettings != null)
                    return _CameraSettings;
                _CameraSettings = new CameraSettings(VideoStream_getCameraSettings(this.Handle));
                return _CameraSettings;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_getCropping(IntPtr objectHandler,
            ref int pOriginX, ref int pOriginY, ref int pWidth, ref int pHeight);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_setCropping(IntPtr objectHandler,
            int pOriginX, int pOriginY, int pWidth, int pHeight);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_resetCropping(IntPtr objectHandler);
        public Rectangle? Cropping
        {
            get
            {
                int x = 0, y = 0, w = 0, h = 0;
                if (VideoStream_getCropping(this.Handle, ref x, ref  y, ref w, ref h))
                    return new Rectangle(x, y, w, h);
                return null;
            }
            set
            {
                if (value == null)
                    OpenNI.throwIfError(VideoStream_resetCropping(this.Handle));
                else
                    OpenNI.throwIfError(VideoStream_setCropping(this.Handle,
                        value.Value.X, value.Value.Y, value.Value.Width, value.Value.Height));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_getMirroringEnabled(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_setMirroringEnabled(IntPtr objectHandler, bool isEnable);
        public bool Mirroring
        {
            get
            {
                return VideoStream_getMirroringEnabled(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(VideoStream_setMirroringEnabled(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoStream_getVideoMode(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_setVideoMode(IntPtr objectHandler, IntPtr vmod);
        public VideoMode VideoMode
        {
            get
            {
                return new VideoMode(VideoStream_getVideoMode(this.Handle), false);
            }
            set
            {
                OpenNI.throwIfError(VideoStream_setVideoMode(this.Handle, value.Handle));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoStream_getSensorInfo(IntPtr objectHandler);
        public SensorInfo SensorInfo
        {
            get
            {
                return new SensorInfo(VideoStream_getSensorInfo(this.Handle));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_isCroppingSupported(IntPtr objectHandler);
        public bool isCroppingSupported
        {
            get
            {
                return VideoStream_isCroppingSupported(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float VideoStream_getHorizontalFieldOfView(IntPtr objectHandler);
        public float HorizontalFieldOfView
        {
            get
            {
                return VideoStream_getHorizontalFieldOfView(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float VideoStream_getVerticalFieldOfView(IntPtr objectHandler);
        public float VerticalFieldOfView
        {
            get
            {
                return VideoStream_getVerticalFieldOfView(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoStream_getMaxPixelValue(IntPtr objectHandler);
        public int MaxPixelValue 
        {
            get
            {
                return VideoStream_getMaxPixelValue(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoStream_getMinPixelValue(IntPtr objectHandler);
        public int MinPixelValue
        {
            get
            {
                return VideoStream_getMinPixelValue(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_start(IntPtr objectHandler);
        public OpenNI.Status Start()
        {
            return VideoStream_start(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoStream_stop(IntPtr objectHandler);
        public void Stop()
        {
            VideoStream_stop(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_getProperty(IntPtr objectHandler,
            int propertyId, IntPtr data, out int dataSize);
        public OpenNI.Status getProperty(int propertyId, IntPtr data, out int dataSize)
        {
            return VideoStream_getProperty(this.Handle, propertyId, data, out dataSize);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_setProperty(IntPtr objectHandler,
            int propertyId, IntPtr data, int dataSize);
        public OpenNI.Status setProperty(int propertyId, IntPtr data, int dataSize)
        {
            return VideoStream_setProperty(this.Handle, propertyId, data, dataSize);
        }


        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_isCommandSupported(IntPtr objectHandler, int commandId);
        public bool isCommandSupported(int commandId)
        {
            return VideoStream_isCommandSupported(this.Handle, commandId);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_invoke(IntPtr objectHandler,
            int commandId, IntPtr data, int dataSize);
        public OpenNI.Status invoke(int commandId, IntPtr data, int dataSize)
        {
            return VideoStream_invoke(this.Handle, commandId, data, dataSize);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_isPropertySupported(IntPtr objectHandler, int propertyId);
        public bool isPropertySupported(int propertyId)
        {
            return VideoStream_isPropertySupported(this.Handle, propertyId);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoStream_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && VideoStream_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status VideoStream_readFrame(IntPtr objectHandler, out IntPtr newFrame);
        public VideoFrameRef readFrame()
        {
            IntPtr fHandle;
            OpenNI.throwIfError(VideoStream_readFrame(this.Handle, out fHandle));
            return new VideoFrameRef(fHandle);
        }

        public bool isFrameAvailable()
        {
            return (OpenNI.WaitForStream(this, OpenNI.TIMEOUT_NONE) == OpenNI.Status.OK);
        }

        public override string ToString()
        {
            return this.SensorInfo.getSensorType().ToString();
        }
    }
}
