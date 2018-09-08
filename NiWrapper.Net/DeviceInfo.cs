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

    public class DeviceInfo : OpenNIBase
    {
        #region Constructors and Destructors

        internal DeviceInfo(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Fields

        private string name;

        private string uri;

        private ushort? usbProductId;

        private ushort? usbVendorId;

        private string vendor;

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                if (name != null)
                {
                    return name;
                }

                var e = DeviceInfo_getName(Handle);
                var nameString = Marshal.PtrToStringAnsi(e);

                if (nameString != null)
                {
                    name = (string) nameString.Clone();
                }

                return name;
            }
        }

        public string Uri
        {
            get
            {
                if (uri != null)
                {
                    return uri;
                }

                var e = DeviceInfo_getUri(Handle);
                var uriString = Marshal.PtrToStringAnsi(e);

                if (uriString != null)
                {
                    uri = (string) uriString.Clone();
                }

                return uri;
            }
        }

        public ushort UsbProductId
        {
            get
            {
                if (usbProductId != null)
                {
                    return usbProductId.Value;
                }

                usbProductId = DeviceInfo_getUsbProductId(Handle);

                return usbProductId.Value;
            }
        }

        public ushort UsbVendorId
        {
            get
            {
                if (usbVendorId != null)
                {
                    return usbVendorId.Value;
                }

                usbVendorId = DeviceInfo_getUsbVendorId(Handle);

                return usbVendorId.Value;
            }
        }

        public string Vendor
        {
            get
            {
                if (vendor != null)
                {
                    return vendor;
                }

                var e = DeviceInfo_getVendor(Handle);
                var vendorString = Marshal.PtrToStringAnsi(e);

                if (vendorString != null)
                {
                    vendor = (string) vendorString.Clone();
                }

                return vendor;
            }
        }

        #endregion

        #region Public Methods and Operators

        public Device OpenDevice()
        {
            if (!IsValid)
            {
                return null;
            }

            return Device.Open(Uri);
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getName(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getUri(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern ushort DeviceInfo_getUsbProductId(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern ushort DeviceInfo_getUsbVendorId(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getVendor(IntPtr objectHandler);

        #endregion
    }
}