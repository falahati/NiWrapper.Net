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
using System.Drawing.Imaging;

namespace OpenNIWrapper
{
    public class VideoFrameRef : OpenNIBase, IDisposable
    {
        public enum copyBitmapOptions
        {
            None = 0,
            Force24BitRGB = 1,
            DepthFillLeftBlack = 2,
            DepthFillRigthBlack = 4,
            DepthHistogramEqualize = 8,
            DepthInvert = 16,
            DepthFillShadow = 32,
        }

        public VideoFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }
        
        ~VideoFrameRef()
        {
            try
            {
                Dispose();
                Common.DeleteObject(this);
            }
            catch (Exception)
            { }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoFrameRef_release(IntPtr objectHandler);
        public void Release()
        {
            VideoFrameRef_release(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoFrameRef_getVideoMode(IntPtr objectHandler);
        VideoMode _VideoMode;
        public VideoMode VideoMode
        {
            get
            {
                if (_VideoMode != null)
                    return _VideoMode;
                _VideoMode = new VideoMode(VideoFrameRef_getVideoMode(this.Handle), true);
                return _VideoMode;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool VideoFrameRef_getCroppingOrigin(IntPtr objectHandler, ref int pOriginX, ref int pOriginY);
        Point? _CroppingOrigin;
        bool _CroppingOrigin_isChached = false;
        public Point? CroppingOrigin
        {
            get
            {
                if (_CroppingOrigin_isChached)
                    return _CroppingOrigin;
                _CroppingOrigin_isChached = true;

                _CroppingOrigin = null;
                int pOriginX = 0, pOriginY = 0;
                bool isEnable = VideoFrameRef_getCroppingOrigin(this.Handle, ref pOriginX, ref pOriginY);
                if (isEnable)
                    _CroppingOrigin = new Point(pOriginX, pOriginY);
                return _CroppingOrigin;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoFrameRef_getSize(IntPtr objectHandler, ref int w, ref int h);
        Size? _FrameSize;
        public Size FrameSize
        {
            get
            {
                if (_FrameSize != null)
                    return _FrameSize.Value;

                int w = 0, h = 0;
                VideoFrameRef_getSize(this.Handle, ref w, ref h);
                _FrameSize = new Size(w, h);
                return _FrameSize.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoFrameRef_getDataSize(IntPtr objectHandler);
        int? _DataSize;
        public int DataSize
        {
            get
            {
                if (_DataSize != null)
                    return _DataSize.Value;

                _DataSize = VideoFrameRef_getDataSize(this.Handle);
                return _DataSize.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VideoFrameRef_getData(IntPtr objectHandler);
        IntPtr? _Data;
        public IntPtr Data
        {
            get
            {
                if (_Data != null)
                    return _Data.Value;

                _Data = VideoFrameRef_getData(this.Handle);
                return _Data.Value;
            }
        }

        public Bitmap ToBitmap(copyBitmapOptions options = copyBitmapOptions.None)
        {
            System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            switch (this.VideoMode.DataPixelFormat)
            {
                case VideoMode.PixelFormat.RGB888:
                    format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                    break;
                case VideoMode.PixelFormat.GRAY8:
                    format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                    break;
                case VideoMode.PixelFormat.DEPTH_1MM:
                case VideoMode.PixelFormat.DEPTH_100UM:
                case VideoMode.PixelFormat.GRAY16:
                    format = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
                    break;
                default:
                    throw new BadImageFormatException("Pixel format is not acceptable for bitmap converting.");
            }
            if ((options & copyBitmapOptions.Force24BitRGB) == copyBitmapOptions.Force24BitRGB)
                format = PixelFormat.Format24bppRgb;
            Bitmap destination = new Bitmap(this.FrameSize.Width, this.FrameSize.Height, format);
            if (format == PixelFormat.Format8bppIndexed)
                for (int i = 0; i < 256; i++)
                    destination.Palette.Entries[i] = Color.FromArgb(i, i, i);
            UpdateBitmap(destination, options);
            return destination;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void VideoFrameRef_copyDataTo(IntPtr objectHandler, IntPtr dstData, int dstStride, copyBitmapOptions options);
        public void UpdateBitmap(Bitmap image, copyBitmapOptions options = copyBitmapOptions.None)
        {
                if (image.Width != this.FrameSize.Width || image.Height != this.FrameSize.Height)
                    throw new ArgumentException("Bitmap size if not acceptable.");
                if (image.PixelFormat == PixelFormat.Format24bppRgb)
                    options |= copyBitmapOptions.Force24BitRGB;
                else if ((options & copyBitmapOptions.Force24BitRGB) == copyBitmapOptions.Force24BitRGB)
                    throw new ArgumentException("Requested RGB888 operation on a non-RGB24 bitmap.");

                System.Drawing.Imaging.PixelFormat desiredFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                switch (this.VideoMode.DataPixelFormat)
                {
                    case VideoMode.PixelFormat.RGB888:
                        desiredFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        break;
                    case VideoMode.PixelFormat.GRAY8:
                        desiredFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                        break;
                    case VideoMode.PixelFormat.DEPTH_1MM:
                    case VideoMode.PixelFormat.DEPTH_100UM:
                    case VideoMode.PixelFormat.GRAY16:
                        desiredFormat = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
                        break;
                    default:
                        throw new BadImageFormatException("Pixel format is not acceptable for bitmap converting.");
                }
                if ((options & copyBitmapOptions.Force24BitRGB) != copyBitmapOptions.Force24BitRGB &&
                    desiredFormat != image.PixelFormat)
                    throw new ArgumentException("Requested bitmap pixel format is not suitable for this data.");
                BitmapData destBits = image.LockBits(new Rectangle(new Point(0, 0), image.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);
                VideoFrameRef_copyDataTo(this.Handle, destBits.Scan0, destBits.Stride, options);
                image.UnlockBits(destBits);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoFrameRef_getFrameIndex(IntPtr objectHandler);
        int? _FrameIndex;
        public int FrameIndex
        {
            get
            {
                if (_FrameIndex != null)
                    return _FrameIndex.Value;

                _FrameIndex = VideoFrameRef_getFrameIndex(this.Handle);
                return _FrameIndex.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int VideoFrameRef_getStrideInBytes(IntPtr objectHandler);
        int? _DataStrideBytes;
        public int DataStrideBytes
        {
            get
            {
                if (_DataStrideBytes != null)
                    return _DataStrideBytes.Value;

                _DataStrideBytes = VideoFrameRef_getStrideInBytes(this.Handle);
                return _DataStrideBytes.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Device.SensorType VideoFrameRef_getSensorType(IntPtr objectHandler);
        Device.SensorType? _SensorType;
        public Device.SensorType SensorType
        {
            get
            {
                if (_SensorType != null)
                    return _SensorType.Value;

                _SensorType = VideoFrameRef_getSensorType(this.Handle);
                return _SensorType.Value;
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt64 VideoFrameRef_getTimestamp(IntPtr objectHandler);
        UInt64? _Timestamp;
        public UInt64 Timestamp
        {
            get
            {
                if (_Timestamp != null)
                    return _Timestamp.Value;

                _Timestamp = VideoFrameRef_getTimestamp(this.Handle);
                return _Timestamp.Value;
            }
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
                if (disposing && this.IsValid)
                    this.Release();

                this.Handle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
