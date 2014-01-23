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
    using System.Drawing;
    using System.Runtime.InteropServices;

    #endregion

    public class VideoMode : OpenNIBase
    {
        #region Fields

        private readonly bool isConst;

        #endregion

        #region Constructors and Destructors

        public VideoMode()
        {
            this.Handle = VideoMode_new();
        }

        internal VideoMode(IntPtr handle, bool isConst)
        {
            this.Handle = handle;
            this.isConst = isConst;
        }

        ~VideoMode()
        {
            try
            {
                if (!this.isConst)
                {
                    Common.DeleteObject(this);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

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

            Yuyv = 205, 
        }

        #endregion

        #region Public Properties

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

        public int Fps
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

        public Size Resolution
        {
            get
            {
                return new Size(VideoMode_getResolutionX(this.Handle), VideoMode_getResolutionY(this.Handle));
            }

            set
            {
                VideoMode_setResolution(this.Handle, value.Width, value.Height);
            }
        }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.Resolution.Width + "x" + this.Resolution.Height + "@" + this.Fps + " " + this.DataPixelFormat;
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getFps(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern PixelFormat VideoMode_getPixelFormat(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getResolutionX(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoMode_getResolutionY(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoMode_new();

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setFps(IntPtr objectHandler, int fps);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setPixelFormat(IntPtr objectHandler, PixelFormat pf);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoMode_setResolution(IntPtr objectHandler, int x, int y);

        #endregion
    }
}