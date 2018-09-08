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

    public class VideoMode : OpenNIBase
    {
        #region Enums

        public enum PixelFormat
        {
            // Depth
            Depth1Mm = 100,

            Depth100Um = 101,

            Shift92 = 102,

            Shift93 = 103,

            // Color
            Rgb888 = 200,

            Yuv422 = 201,

            Gray8 = 202,

            Gray16 = 203,

            Jpeg = 204,

            Yuyv = 205
        }

        #endregion

        #region Fields

        private readonly bool isConst;

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return Resolution.Width + "x" + Resolution.Height + "@" + Fps + " " + DataPixelFormat;
        }

        #endregion

        #region Constructors and Destructors

        public VideoMode()
        {
            Handle = VideoMode_new();
        }

        internal VideoMode(IntPtr handle, bool isConst)
        {
            Handle = handle;
            this.isConst = isConst;
        }

        ~VideoMode()
        {
            try
            {
                if (!isConst)
                {
                    Common.DeleteObject(this);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Public Properties

        public PixelFormat DataPixelFormat
        {
            get => VideoMode_getPixelFormat(Handle);

            set => VideoMode_setPixelFormat(Handle, value);
        }

        public int Fps
        {
            get => VideoMode_getFps(Handle);

            set => VideoMode_setFps(Handle, value);
        }

        public Size Resolution
        {
            get => new Size(VideoMode_getResolutionX(Handle), VideoMode_getResolutionY(Handle));

            set => VideoMode_setResolution(Handle, value.Width, value.Height);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getFps(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern PixelFormat VideoMode_getPixelFormat(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getResolutionX(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getResolutionY(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoMode_new();

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setFps(IntPtr objectHandler, int fps);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setPixelFormat(IntPtr objectHandler, PixelFormat pf);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setResolution(IntPtr objectHandler, int x, int y);

        #endregion
    }
}