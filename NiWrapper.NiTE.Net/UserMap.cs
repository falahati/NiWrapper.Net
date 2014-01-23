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
namespace NiTEWrapper
{
    #region

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    #endregion

    public class UserMap : NiTEBase
    {
        #region Fields

        private int? dataStrideBytes;

        private Size? frameSize;

        private IntPtr? pixels;

        #endregion

        #region Constructors and Destructors

        internal UserMap(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Public Properties

        public int DataStrideBytes
        {
            get
            {
                if (this.dataStrideBytes != null)
                {
                    return this.dataStrideBytes.Value;
                }

                this.dataStrideBytes = UserMap_getStride(this.Handle);
                return this.dataStrideBytes.Value;
            }
        }

        public Size FrameSize
        {
            get
            {
                if (this.frameSize != null)
                {
                    return this.frameSize.Value;
                }

                int w = 0, h = 0;
                UserMap_getSize(this.Handle, ref w, ref h);
                this.frameSize = new Size(w, h);
                return this.frameSize.Value;
            }
        }

        public IntPtr Pixels
        {
            get
            {
                if (this.pixels != null)
                {
                    return this.pixels.Value;
                }

                this.pixels = UserMap_getPixels(this.Handle);
                return this.pixels.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserMap_getPixels(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserMap_getSize(IntPtr objectHandler, ref int w, ref int h);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UserMap_getStride(IntPtr objectHandler);

        #endregion
    }
}