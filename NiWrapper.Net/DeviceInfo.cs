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

    public class DeviceInfo : OpenNIBase
    {
        #region Fields

        private string name;

        private string uri;

        private ushort? usbProductId;

        private ushort? usbVendorId;

        private string vendor;

        #endregion

        #region Constructors and Destructors

        internal DeviceInfo(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }

                IntPtr e = DeviceInfo_getName(this.Handle);
                string nameString = Marshal.PtrToStringAnsi(e);
                if (nameString != null)
                {
                    this.name = (string)nameString.Clone();
                }

                return this.name;
            }
        }

        public string Uri
        {
            get
            {
                if (this.uri != null)
                {
                    return this.uri;
                }

                IntPtr e = DeviceInfo_getUri(this.Handle);
                string uriString = Marshal.PtrToStringAnsi(e);
                if (uriString != null)
                {
                    this.uri = (string)uriString.Clone();
                }

                return this.uri;
            }
        }

        public ushort UsbProductId
        {
            get
            {
                if (this.usbProductId != null)
                {
                    return this.usbProductId.Value;
                }

                this.usbProductId = DeviceInfo_getUsbProductId(this.Handle);
                return this.usbProductId.Value;
            }
        }

        public ushort UsbVendorId
        {
            get
            {
                if (this.usbVendorId != null)
                {
                    return this.usbVendorId.Value;
                }

                this.usbVendorId = DeviceInfo_getUsbVendorId(this.Handle);
                return this.usbVendorId.Value;
            }
        }

        public string Vendor
        {
            get
            {
                if (this.vendor != null)
                {
                    return this.vendor;
                }

                IntPtr e = DeviceInfo_getVendor(this.Handle);
                string vendorString = Marshal.PtrToStringAnsi(e);
                if (vendorString != null)
                {
                    this.vendor = (string)vendorString.Clone();
                }

                return this.vendor;
            }
        }

        #endregion

        #region Public Methods and Operators

        public Device OpenDevice()
        {
            if (!this.IsValid)
            {
                return null;
            }

            return Device.Open(this.Uri);
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getName(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getUri(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ushort DeviceInfo_getUsbProductId(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ushort DeviceInfo_getUsbVendorId(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr DeviceInfo_getVendor(IntPtr objectHandler);

        #endregion
    }
}