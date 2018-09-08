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
using System.Runtime.InteropServices;
using OpenNIWrapper;

namespace NiTEWrapper
{
    #region

    #endregion

    public sealed class HandTrackerFrameRef : NiTEBase, IDisposable
    {
        #region Fields

        private VideoFrameRef depthFrame;

        private int? frameIndex;

        private GestureData[] gestures;

        private HandData[] hands;

        private ulong? timestamp;

        #endregion

        #region Constructors and Destructors

        internal HandTrackerFrameRef(IntPtr handle)
        {
            Handle = handle;
        }

        ~HandTrackerFrameRef()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public VideoFrameRef DepthFrame
        {
            get => depthFrame ?? (depthFrame = new VideoFrameRef(HandTrackerFrameRef_getDepthFrame(Handle)));
        }

        public int FrameIndex
        {
            get
            {
                if (frameIndex != null)
                {
                    return frameIndex.Value;
                }

                frameIndex = HandTrackerFrameRef_getFrameIndex(Handle);

                return frameIndex.Value;
            }
        }

        public IEnumerable<GestureData> Gestures
        {
            get
            {
                if (gestures == null)
                {
                    var csa = HandTrackerFrameRef_getGestures(Handle);
                    var array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    gestures = new GestureData[csa.Size];

                    for (var i = 0; i < csa.Size; i++)
                    {
                        gestures[i] = new GestureData(array[i]);
                    }

                    HandTrackerFrameRef_destroyGesturesArray(csa);
                }

                return gestures;
            }
        }

        public HandData[] Hands
        {
            get
            {
                if (hands == null)
                {
                    var csa = HandTrackerFrameRef_getHands(Handle);
                    var array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    hands = new HandData[csa.Size];

                    for (var i = 0; i < csa.Size; i++)
                    {
                        hands[i] = new HandData(array[i]);
                    }

                    HandTrackerFrameRef_destroyHandsArray(csa);
                }

                return hands;
            }
        }

        public new bool IsValid
        {
            get => base.IsValid && HandTrackerFrameRef_isValid(Handle);
        }

        public ulong Timestamp
        {
            get
            {
                if (timestamp != null)
                {
                    return timestamp.Value;
                }

                timestamp = HandTrackerFrameRef_getTimestamp(Handle);

                return timestamp.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        private void ReleaseUnmanagedResources()
        {
            if (IsValid)
            {
                HandTrackerFrameRef_release(Handle);
                Handle = IntPtr.Zero;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            gestures = null;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr HandTrackerFrameRef_destroyGesturesArray(WrapperArray array);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr HandTrackerFrameRef_destroyHandsArray(WrapperArray array);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr HandTrackerFrameRef_getDepthFrame(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern int HandTrackerFrameRef_getFrameIndex(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrapperArray HandTrackerFrameRef_getGestures(IntPtr vf);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrapperArray HandTrackerFrameRef_getHands(IntPtr vf);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong HandTrackerFrameRef_getTimestamp(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandTrackerFrameRef_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void HandTrackerFrameRef_release(IntPtr objectHandler);

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();

            if (disposing)
            {
                depthFrame?.Dispose();
            }
        }

        #endregion
    }
}