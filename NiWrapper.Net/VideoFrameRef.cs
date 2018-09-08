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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    #region

    #endregion

    public sealed class VideoFrameRef : OpenNIBase, IDisposable
    {
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

            DepthFillShadow = 32
        }

        #endregion

        #region Fields

        private Point? croppingOrigin;

        private bool croppingOriginIsChached;

        private IntPtr? data;

        private int? dataSize;

        private int? dataStrideBytes;

        private int? frameIndex;

        private Size? frameSize;

        private Device.SensorType? sensorType;

        private ulong? timestamp;

        private VideoMode videoMode;

        #endregion

        #region Constructors and Destructors

        public VideoFrameRef(IntPtr handle)
        {
            Handle = handle;
        }

        ~VideoFrameRef()
        {
            ReleaseUnmanagedResources();
        }

        #endregion

        #region Public Properties

        public Point? CroppingOrigin
        {
            get
            {
                if (croppingOriginIsChached)
                {
                    return croppingOrigin;
                }

                croppingOriginIsChached = true;

                croppingOrigin = null;
                int originX = 0, originY = 0;
                var isEnable = VideoFrameRef_getCroppingOrigin(Handle, ref originX, ref originY);

                if (isEnable)
                {
                    croppingOrigin = new Point(originX, originY);
                }

                return croppingOrigin;
            }
        }

        public IntPtr Data
        {
            get
            {
                if (data != null)
                {
                    return data.Value;
                }

                data = VideoFrameRef_getData(Handle);

                return data.Value;
            }
        }

        public int DataSize
        {
            get
            {
                if (dataSize != null)
                {
                    return dataSize.Value;
                }

                dataSize = VideoFrameRef_getDataSize(Handle);

                return dataSize.Value;
            }
        }

        public int DataStrideBytes
        {
            get
            {
                if (dataStrideBytes != null)
                {
                    return dataStrideBytes.Value;
                }

                dataStrideBytes = VideoFrameRef_getStrideInBytes(Handle);

                return dataStrideBytes.Value;
            }
        }

        public int FrameIndex
        {
            get
            {
                if (frameIndex != null)
                {
                    return frameIndex.Value;
                }

                frameIndex = VideoFrameRef_getFrameIndex(Handle);

                return frameIndex.Value;
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
                VideoFrameRef_getSize(Handle, ref w, ref h);
                frameSize = new Size(w, h);

                return frameSize.Value;
            }
        }

        public Device.SensorType SensorType
        {
            get
            {
                if (sensorType != null)
                {
                    return sensorType.Value;
                }

                sensorType = VideoFrameRef_getSensorType(Handle);

                return sensorType.Value;
            }
        }

        public ulong Timestamp
        {
            get
            {
                if (timestamp != null)
                {
                    return timestamp.Value;
                }

                timestamp = VideoFrameRef_getTimestamp(Handle);

                return timestamp.Value;
            }
        }

        public VideoMode VideoMode
        {
            get
            {
                if (videoMode != null)
                {
                    return videoMode;
                }

                videoMode = new VideoMode(VideoFrameRef_getVideoMode(Handle), true);

                return videoMode;
            }
        }

        #endregion

        #region Public Methods and Operators

        public Bitmap ToBitmap(CopyBitmapOptions options = CopyBitmapOptions.None)
        {
            PixelFormat format;

            switch (VideoMode.DataPixelFormat)
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

            var destination = new Bitmap(FrameSize.Width, FrameSize.Height, format);

            if (format == PixelFormat.Format8bppIndexed)
            {
                for (var i = 0; i < 256; i++)
                {
                    destination.Palette.Entries[i] = Color.FromArgb(i, i, i);
                }
            }

            UpdateBitmap(destination, options);

            return destination;
        }

        public void UpdateBitmap(Bitmap image, CopyBitmapOptions options = CopyBitmapOptions.None)
        {
            if (image.Width != FrameSize.Width || image.Height != FrameSize.Height)
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

            switch (VideoMode.DataPixelFormat)
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

            if ((options & CopyBitmapOptions.Force24BitRgb) != CopyBitmapOptions.Force24BitRgb &&
                desiredFormat != image.PixelFormat)
            {
                throw new ArgumentException("Requested bitmap pixel format is not suitable for this data.");
            }

            var destBits = image.LockBits(
                new Rectangle(new Point(0, 0), image.Size),
                ImageLockMode.WriteOnly,
                image.PixelFormat);
            VideoFrameRef_copyDataTo(Handle, destBits.Scan0, destBits.Stride, options);
            image.UnlockBits(destBits);
        }

        private void ReleaseUnmanagedResources()
        {
            if (IsValid)
            {
                VideoFrameRef_release(Handle);
                Handle = IntPtr.Zero;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_copyDataTo(
            IntPtr objectHandler,
            IntPtr dstData,
            int dstStride,
            CopyBitmapOptions options);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool VideoFrameRef_getCroppingOrigin(
            IntPtr objectHandler,
            ref int originX,
            ref int originY);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoFrameRef_getData(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getDataSize(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getFrameIndex(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern Device.SensorType VideoFrameRef_getSensorType(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_getSize(IntPtr objectHandler, ref int w, ref int h);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int VideoFrameRef_getStrideInBytes(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong VideoFrameRef_getTimestamp(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr VideoFrameRef_getVideoMode(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VideoFrameRef_release(IntPtr objectHandler);

        #endregion
    }
}