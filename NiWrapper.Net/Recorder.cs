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

    public class Recorder : OpenNIBase, IDisposable
    {
        #region Fields

        private bool isDisposed;

        #endregion

        #region Public Properties

        public new bool IsValid
        {
            get => base.IsValid && Recorder_isValid(Handle);
        }

        #endregion

        #region Constructors and Destructors

        internal Recorder(IntPtr handle)
        {
            Handle = handle;
        }

        ~Recorder()
        {
            try
            {
                Destroy();
                Common.DeleteObject(this);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Public Methods and Operators

        public static Recorder Create(string fileName)
        {
            IntPtr handle;
            OpenNI.ThrowIfError(Recorder_create(out handle, Marshal.StringToHGlobalAnsi(fileName)));
            var rec = new Recorder(handle);

            return rec;
        }

        public OpenNI.Status Attach(VideoStream stream, bool allowLossyCompression = false)
        {
            return Recorder_attach(Handle, stream.Handle, allowLossyCompression);
        }

        public void Destroy()
        {
            Recorder_destroy(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public OpenNI.Status Start()
        {
            return Recorder_start(Handle);
        }

        public void Stop()
        {
            Recorder_stop(Handle);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing && IsValid)
                {
                    Destroy();
                }

                Handle = IntPtr.Zero;
                isDisposed = true;
            }
        }

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Recorder_attach(
            IntPtr objectHandler,
            IntPtr stream,
            bool allowLossyCompression = false);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Recorder_create(out IntPtr objectHandler, IntPtr fileName);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Recorder_destroy(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Recorder_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status Recorder_start(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Recorder_stop(IntPtr objectHandler);

        #endregion
    }
}