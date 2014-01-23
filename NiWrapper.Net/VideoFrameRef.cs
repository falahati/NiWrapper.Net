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
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    #endregion

    public sealed class VideoFrameRef : OpenNIBase, IDisposable
    {
        #region Fields

        private Point? croppingOrigin;

        private bool croppingOriginIsChached;

        private IntPtr? data;

        private int? dataSize;

        private int? dataStrideBytes;

        private int? frameIndex;

        private Size? frameSize;

        private bool isDisposed;

        private Device.SensorType? sensorType;

        private ulong? timestamp;

        private VideoMode videoMode;

        #endregion

        #region Constructors and Destructors

        public VideoFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~VideoFrameRef()
        {
            try
            {
                this.Dispose();
                Common.DeleteObject(this);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Enums

        [Flags]
        public enum CopyBitmapOptions
        {
            None = 0, 

            Force24BitRgb = 1, 

            DepthFillLeftBlack = 2, 

            DepthFillRigthBlack = 4, 

            DepthHistogramEqualize = 8, 

            DepthInvert = 16, 

            DepthFillShadow = 32, 
        }

        #endregion

        #region Public Properties

        public Point? CroppingOrigin
        {
            get
            {
                if (this.croppingOriginIsChached)
                {
                    return this.croppingOrigin;
                }

                this.croppingOriginIsChached = true;

                this.croppingOrigin = null;
                int originX = 0, originY = 0;
                bool isEnable = VideoFrameRef_getCroppingOrigin(this.Handle, ref originX, ref originY);
                if (isEnable)
                {
                    this.croppingOrigin = new Point(originX, originY);
                }

                return this.croppingOrigin;
            }
        }

        public IntPtr Data
        {
            get
            {
                if (this.data != null)
                {
                    return this.data.Value;
                }

                this.data = VideoFrameRef_getData(this.Handle);
                return this.data.Value;
            }
        }

        public int DataSize
        {
            get
            {
                if (this.dataSize != null)
                {
                    return this.dataSize.Value;
                }

                this.dataSize = VideoFrameRef_getDataSize(this.Handle);
                return this.dataSize.Value;
            }
        }

        public int DataStrideBytes
        {
            get
            {
                if (this.dataStrideBytes != null)
                {
                    return this.dataStrideBytes.Value;
                }

                this.dataStrideBytes = VideoFrameRef_getStrideInBytes(this.Handle);
                return this.dataStrideBytes.Value;
            }
        }

        public int FrameIndex
        {
            get
            {
                if (this.frameIndex != null)
                {
                    return this.frameIndex.Value;
                }

                this.frameIndex = VideoFrameRef_getFrameIndex(this.Handle);
                return this.frameIndex.Value;
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
                VideoFrameRef_getSize(this.Handle, ref w, ref h);
                this.frameSize = new Size(w, h);
                return this.frameSize.Value;
            }
        }

        public Device.SensorType SensorType
        {
            get
            {
                if (this.sensorType != null)
                {
                    return this.sensorType.Value;
                }

                this.sensorType = VideoFrameRef_getSensorType(this.Handle);
                return this.sensorType.Value;
            }
        }

        public ulong Timestamp
        {
            get
            {
                if (this.timestamp != null)
                {
                    return this.timestamp.Value;
                }

                this.timestamp = VideoFrameRef_getTimestamp(this.Handle);
                return this.timestamp.Value;
            }
        }

        public VideoMode VideoMode
        {
            get
            {
                if (this.videoMode != null)
                {
                    return this.videoMode;
                }

                this.videoMode = new VideoMode(VideoFrameRef_getVideoMode(this.Handle), true);
                return this.videoMode;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Release()
        {
            VideoFrameRef_release(this.Handle);
        }

        public Bitmap ToBitmap(CopyBitmapOptions options = CopyBitmapOptions.None)
        {
            PixelFormat format;
            switch (this.VideoMode.DataPixelFormat)
            {
                case VideoMode.PixelFormat.Rgb888:
                    format = PixelFormat.Format24bppRgb;
                    break;
                case VideoMode.PixelFormat.Gray8:
                    format = PixelFormat.Format8bppIndexed;
                    break;
                case VideoMode.PixelFormat.Depth1Mm:
                case VideoMode.PixelFormat.Depth100Um:
                case VideoMode.PixelFormat.Gray16:
                    format = PixelFormat.Format16bppGrayScale;
                    break;
                default:
                    throw new InvalidOperationException("Pixel format is not acceptable for bitmap converting.");
            }

            if ((options & CopyBitmapOptions.Force24BitRgb) == CopyBitmapOptions.Force24BitRgb)
            {
                format = PixelFormat.Format24bppRgb;
            }

            Bitmap destination = new Bitmap(this.FrameSize.Width, this.FrameSize.Height, format);
            if (format == PixelFormat.Format8bppIndexed)
            {
                for (int i = 0; i < 256; i++)
                {
                    destination.Palette.Entries[i] = Color.FromArgb(i, i, i);
                }
            }

            this.UpdateBitmap(destination, options);
            return destination;
        }

        public void UpdateBitmap(Bitmap image, CopyBitmapOptions options = CopyBitmapOptions.None)
        {
            if (image.Width != this.FrameSize.Width || image.Height != this.FrameSize.Height)
            {
                throw new ArgumentException("Bitmap size if not acceptable.");
            }

            if (image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                options |= CopyBitmapOptions.Force24BitRgb;
            }
            else if ((options & CopyBitmapOptions.Force24BitRgb) == CopyBitmapOptions.Force24BitRgb)
            {
                throw new ArgumentException("Requested RGB888 operation on a non-RGB24 bitmap.");
            }

            PixelFormat desiredFormat;
            switch (this.VideoMode.DataPixelFormat)
            {
                case VideoMode.PixelFormat.Rgb888:
                    desiredFormat = PixelFormat.Format24bppRgb;
                    break;
                case VideoMode.PixelFormat.Gray8:
                    desiredFormat = PixelFormat.Format8bppIndexed;
                    break;
                case VideoMode.PixelFormat.Depth1Mm:
                case VideoMode.PixelFormat.Depth100Um:
                case VideoMode.PixelFormat.Gray16:
                    desiredFormat = PixelFormat.Format16bppGrayScale;
                    break;
                default:
                    throw new InvalidOperationException("Pixel format is not acceptable for bitmap converting.");
            }

            if ((options & CopyBitmapOptions.Force24BitRgb) != CopyBitmapOptions.Force24BitRgb
                && desiredFormat != image.PixelFormat)
            {
                throw new ArgumentException("Requested bitmap pixel format is not suitable for this data.");
            }

            BitmapData destBits = image.LockBits(
                new Rectangle(new Point(0, 0), image.Size), 
                ImageLockMode.WriteOnly, 
                image.PixelFormat);
            VideoFrameRef_copyDataTo(this.Handle, destBits.Scan0, destBits.Stride, options);
            image.UnlockBits(destBits);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_copyDataTo(
            IntPtr objectHandler, 
            IntPtr dstData, 
            int dstStride, 
            CopyBitmapOptions options);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoFrameRef_getCroppingOrigin(
            IntPtr objectHandler, 
            ref int originX, 
            ref int originY);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoFrameRef_getData(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getDataSize(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getFrameIndex(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Device.SensorType VideoFrameRef_getSensorType(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_getSize(IntPtr objectHandler, ref int w, ref int h);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getStrideInBytes(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong VideoFrameRef_getTimestamp(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoFrameRef_getVideoMode(IntPtr objectHandler);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_release(IntPtr objectHandler);

        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing && this.IsValid)
                {
                    this.Release();
                }

                this.Handle = IntPtr.Zero;
                this.isDisposed = true;
            }
        }

        #endregion
    }
}