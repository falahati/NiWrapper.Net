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
