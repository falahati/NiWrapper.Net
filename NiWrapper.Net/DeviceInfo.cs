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
    public class DeviceInfo : OpenNIBase
    {
        internal DeviceInfo(IntPtr handle)
        {
            this.Handle = handle;
        }

        public Device OpenDevice()
        {
            if (!this.isValid)
                return null;
            return Device.Open(this.URI);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr DeviceInfo_getName(IntPtr objectHandler);
        string _Name = null;
        public String Name
        {
            get
            {
                if (_Name != null) return _Name;
                IntPtr e = DeviceInfo_getName(this.Handle);
                _Name = (string)Marshal.PtrToStringAnsi(e).Clone();
                //Marshal.FreeBSTR(e);
                return _Name;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr DeviceInfo_getUri(IntPtr objectHandler);
        string _Uri = null;
        public String URI
        {
            get
            {
                if (_Uri != null) return _Uri;
                IntPtr e = DeviceInfo_getUri(this.Handle);
                _Uri = (string)Marshal.PtrToStringAnsi(e).Clone();
                return _Uri;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr DeviceInfo_getVendor(IntPtr objectHandler);
        string _Vendor = null;
        public String Vendor
        {
            get
            {
                if (_Vendor != null) return _Vendor;
                IntPtr e = DeviceInfo_getVendor(this.Handle);
                _Vendor = (string)Marshal.PtrToStringAnsi(e).Clone();
                //Marshal.FreeBSTR(e);
                return _Vendor;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt16 DeviceInfo_getUsbProductId(IntPtr objectHandler);
        UInt16? _UsbProductId = null;
        public UInt16 UsbProductId
        {
            get
            {
                if (_UsbProductId != null) return _UsbProductId.Value;
                _UsbProductId = DeviceInfo_getUsbProductId(this.Handle);
                return _UsbProductId.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt16 DeviceInfo_getUsbVendorId(IntPtr objectHandler);
        UInt16? _UsbVendorId = null;
        public UInt16 UsbVendorId
        {
            get
            {
                if (_UsbVendorId != null) return _UsbVendorId.Value;
                _UsbVendorId = DeviceInfo_getUsbVendorId(this.Handle);
                return _UsbVendorId.Value;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
