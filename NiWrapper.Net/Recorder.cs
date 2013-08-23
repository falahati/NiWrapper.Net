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
    public class Recorder : OpenNIBase, IDisposable
    {
        internal Recorder(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~Recorder()
        {
            try
            {
                this.Destroy();
                Common.DeleteObject(this);
            }
            catch (Exception)
            { }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Recorder_destroy(IntPtr objectHandler);
        public void Destroy()
        {
            Recorder_destroy(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Recorder_create(out IntPtr objectHandler, IntPtr fileName);
        public static Recorder Create(string fileName)
        {
            IntPtr handle;
            OpenNI.throwIfError(Recorder_create(out handle, Marshal.StringToHGlobalAnsi(fileName)));
            Recorder rec = new Recorder(handle);
            return rec;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Recorder_start(IntPtr objectHandler);
        public OpenNI.Status Start()
        {
            return Recorder_start(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Recorder_stop(IntPtr objectHandler);
        public void Stop()
        {
            Recorder_stop(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Recorder_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && Recorder_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status Recorder_attach(IntPtr objectHandler, IntPtr stream, bool allowLossyCompression = false);
        public OpenNI.Status Attach(VideoStream stream, bool allowLossyCompression = false)
        {
            return Recorder_attach(this.Handle, stream.Handle, allowLossyCompression);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && this.isValid)
                    this.Destroy();

                this.Handle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
