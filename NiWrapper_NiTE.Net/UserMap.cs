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
using System.Drawing;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    #region

    #endregion

    public class UserMap : NiTEBase
    {
        #region Constructors and Destructors

        internal UserMap(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Fields

        private int? dataStrideBytes;

        private Size? frameSize;

        private IntPtr? pixels;

        #endregion

        #region Public Properties

        public int DataStrideBytes
        {
            get
            {
                if (dataStrideBytes != null)
                {
                    return dataStrideBytes.Value;
                }

                dataStrideBytes = UserMap_getStride(Handle);

                return dataStrideBytes.Value;
            }
        }

        public Size FrameSize
        {
            get
            {
                if (frameSize != null)
                {
                    return frameSize.Value;
                }

                int w = 0, h = 0;
                UserMap_getSize(Handle, ref w, ref h);
                frameSize = new Size(w, h);

                return frameSize.Value;
            }
        }

        public IntPtr Pixels
        {
            get
            {
                if (pixels != null)
                {
                    return pixels.Value;
                }

                pixels = UserMap_getPixels(Handle);

                return pixels.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserMap_getPixels(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserMap_getSize(IntPtr objectHandler, ref int w, ref int h);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UserMap_getStride(IntPtr objectHandler);

        #endregion
    }
}