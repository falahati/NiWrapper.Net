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
    public class CameraSettings : OpenNIBase
    {
        internal CameraSettings(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool CameraSettings_isValid(IntPtr objectHandler);
        public new bool IsValid
        {
            get
            {
                return base.IsValid && CameraSettings_isValid(this.Handle);
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
