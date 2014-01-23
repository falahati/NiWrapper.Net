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
    using System.Runtime.InteropServices;

    #endregion

    public class CameraSettings : OpenNIBase
    {
        #region Constructors and Destructors

        internal CameraSettings(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Public Properties

        public bool AutoExposure
        {
            get
            {
                return CameraSettings_getAutoExposureEnabled(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(CameraSettings_setAutoExposureEnabled(this.Handle, value));
            }
        }

        public bool AutoWhiteBalance
        {
            get
            {
                return CameraSettings_getAutoWhiteBalanceEnabled(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(CameraSettings_setAutoWhiteBalanceEnabled(this.Handle, value));
            }
        }

        public int Exposure
        {
            get
            {
                return CameraSettings_getExposure(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(CameraSettings_setExposure(this.Handle, value));
            }
        }

        public int Gain
        {
            get
            {
                return CameraSettings_getGain(this.Handle);
            }

            set
            {
                OpenNI.ThrowIfError(CameraSettings_setGain(this.Handle, value));
            }
        }

        public new bool IsValid
        {
            get
            {
                return base.IsValid && CameraSettings_isValid(this.Handle);
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_getAutoExposureEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_getAutoWhiteBalanceEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CameraSettings_getExposure(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CameraSettings_getGain(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setAutoExposureEnabled(IntPtr objectHandler, bool isEnable);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setAutoWhiteBalanceEnabled(
            IntPtr objectHandler, 
            bool isEnable);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setExposure(IntPtr objectHandler, int value);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setGain(IntPtr objectHandler, int value);

        #endregion
    }
}