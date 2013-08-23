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
using System.Drawing;

namespace NiTEWrapper
{
    public class UserMap : NiTEBase
    {
        internal UserMap(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int UserMap_getStride(IntPtr objectHandler);
        int? _DataStrideBytes;
        public int DataStrideBytes
        {
            get
            {
                if (_DataStrideBytes != null)
                    return _DataStrideBytes.Value;

                _DataStrideBytes = UserMap_getStride(this.Handle);
                return _DataStrideBytes.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserMap_getPixels(IntPtr objectHandler);
        IntPtr? _Pixels;
        public IntPtr Pixels
        {
            get
            {
                if (_Pixels != null)
                    return _Pixels.Value;

                _Pixels = UserMap_getPixels(this.Handle);
                return _Pixels.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserMap_getSize(IntPtr objectHandler, ref int w, ref int h);
        Size? _FrameSize;
        public Size FrameSize
        {
            get
            {
                if (_FrameSize != null)
                    return _FrameSize.Value;

                int w = 0, h = 0;
                UserMap_getSize(this.Handle, ref w, ref h);
                _FrameSize = new Size(w, h);
                return _FrameSize.Value;
            }
        }
    }
}
