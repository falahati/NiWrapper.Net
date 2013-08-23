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

namespace OpenNIWrapper
{
    public class VideoMode : OpenNIBase
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct InnerVideoMode
        {
            PixelFormat pixelFormat;
            int resolutionX;
            int resolutionY;
            int fps;
        }
        public enum PixelFormat
        {
            // Depth
            DEPTH_1MM = 100,
            DEPTH_100UM = 101,
            SHIFT_92 = 102,
            SHIFT_93 = 103,
            // Color
            RGB888 = 200,
            YUV422 = 201,
            GRAY8 = 202,
            GRAY16 = 203,
            JPEG = 204,
            YUYV = 205,
        }
        bool _isConst = false;
        internal VideoMode(IntPtr handle, bool isConst)
        {
            this.Handle = handle;
            _isConst = isConst;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoMode_new();
        public VideoMode()
        {
            this.Handle = VideoMode_new();
        }

        ~VideoMode()
        {
            try
            {
                if (!_isConst)
                    Common.DeleteObject(this);
            }
            catch (Exception)
            { }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern PixelFormat VideoMode_getPixelFormat(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoMode_setPixelFormat(IntPtr objectHandler, PixelFormat pf);
        public PixelFormat DataPixelFormat
        {
            get
            {
                return VideoMode_getPixelFormat(this.Handle);
            }
            set
            {
                VideoMode_setPixelFormat(this.Handle, value);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoMode_getFps(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoMode_setFps(IntPtr objectHandler, int fps);
        public int FPS
        {
            get
            {
                return VideoMode_getFps(this.Handle);
            }
            set
            {
                VideoMode_setFps(this.Handle, value);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoMode_getResolutionX(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoMode_getResolutionY(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoMode_setResolution(IntPtr objectHandler, int x, int y);
        public Size Resolution
        {
            get
            {
                return new Size(VideoMode_getResolutionX(this.Handle),
                    VideoMode_getResolutionY(this.Handle));
            }
            set
            {
                VideoMode_setResolution(this.Handle, value.Width, value.Height);
            }
        }

        public override string ToString()
        {
            return this.Resolution.Width.ToString() + "x" + this.Resolution.Height.ToString() +
                "@" + this.FPS.ToString() + " " + this.DataPixelFormat.ToString();
        }
    }
}
