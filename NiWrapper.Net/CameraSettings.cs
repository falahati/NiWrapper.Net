using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    public class CameraSettings : OpenNIBase
    {
        internal CameraSettings(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CameraSettings_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && CameraSettings_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CameraSettings_getAutoExposureEnabled(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status CameraSettings_setAutoExposureEnabled(IntPtr objectHandler, bool isEnable);
        public bool AutoExposure
        {
            get
            {
                return CameraSettings_getAutoExposureEnabled(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(CameraSettings_setAutoExposureEnabled(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CameraSettings_getAutoWhiteBalanceEnabled(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status CameraSettings_setAutoWhiteBalanceEnabled(IntPtr objectHandler, bool isEnable);
        public bool AutoWhiteBalance
        {
            get
            {
                return CameraSettings_getAutoWhiteBalanceEnabled(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(CameraSettings_setAutoWhiteBalanceEnabled(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int CameraSettings_getExposure(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status CameraSettings_setExposure(IntPtr objectHandler, int value);
        public int Exposure
        {
            get
            {
                return CameraSettings_getExposure(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(CameraSettings_setExposure(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int CameraSettings_getGain(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status CameraSettings_setGain(IntPtr objectHandler, int value);
        public int Gain
        {
            get
            {
                return CameraSettings_getGain(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(CameraSettings_setGain(this.Handle, value));
            }
        }
    }
}
