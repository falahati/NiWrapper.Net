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
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    #region

    #endregion

    public class CameraSettings : OpenNIBase
    {
        #region Constructors and Destructors

        internal CameraSettings(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Public Properties

        public bool AutoExposure
        {
            get => CameraSettings_getAutoExposureEnabled(Handle);

            set => OpenNI.ThrowIfError(CameraSettings_setAutoExposureEnabled(Handle, value));
        }

        public bool AutoWhiteBalance
        {
            get => CameraSettings_getAutoWhiteBalanceEnabled(Handle);

            set => OpenNI.ThrowIfError(CameraSettings_setAutoWhiteBalanceEnabled(Handle, value));
        }

        public int Exposure
        {
            get => CameraSettings_getExposure(Handle);

            set => OpenNI.ThrowIfError(CameraSettings_setExposure(Handle, value));
        }

        public int Gain
        {
            get => CameraSettings_getGain(Handle);

            set => OpenNI.ThrowIfError(CameraSettings_setGain(Handle, value));
        }

        public new bool IsValid
        {
            get => base.IsValid && CameraSettings_isValid(Handle);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_getAutoExposureEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_getAutoWhiteBalanceEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CameraSettings_getExposure(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CameraSettings_getGain(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CameraSettings_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setAutoExposureEnabled(IntPtr objectHandler, bool isEnable);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setAutoWhiteBalanceEnabled(
            IntPtr objectHandler,
            bool isEnable);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setExposure(IntPtr objectHandler, int value);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CameraSettings_setGain(IntPtr objectHandler, int value);

        #endregion
    }
}