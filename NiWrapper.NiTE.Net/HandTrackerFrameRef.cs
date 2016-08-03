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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using OpenNIWrapper;

    #endregion

    public sealed class HandTrackerFrameRef : NiTEBase, IDisposable
    {
        #region Fields

        private VideoFrameRef depthFrame;

        private int? frameIndex;

        private GestureData[] gestures;

        private HandData[] hands;

        private bool isDisposed;

        private ulong? timestamp;

        #endregion

        #region Constructors and Destructors

        internal HandTrackerFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~HandTrackerFrameRef()
        {
            try
            {
                this.Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Public Properties

        public VideoFrameRef DepthFrame
        {
            get
            {
                return this.depthFrame
                       ?? (this.depthFrame = new VideoFrameRef(HandTrackerFrameRef_getDepthFrame(this.Handle)));
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

                this.frameIndex = HandTrackerFrameRef_getFrameIndex(this.Handle);
                return this.frameIndex.Value;
            }
        }

        public IEnumerable<GestureData> Gestures
        {
            get
            {
                if (this.gestures == null)
                {
                    WrapperArray csa = HandTrackerFrameRef_getGestures(this.Handle);
                    IntPtr[] array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    this.gestures = new GestureData[csa.Size];
                    for (int i = 0; i < csa.Size; i++)
                    {
                        this.gestures[i] = new GestureData(array[i]);
                    }

                    HandTrackerFrameRef_destroyGesturesArray(csa);
                }

                return this.gestures;
            }
        }

        public HandData[] Hands
        {
            get
            {
                if (this.hands == null)
                {
                    WrapperArray csa = HandTrackerFrameRef_getHands(this.Handle);
                    IntPtr[] array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    this.hands = new HandData[csa.Size];
                    for (int i = 0; i < csa.Size; i++)
                    {
                        this.hands[i] = new HandData(array[i]);
                    }

                    HandTrackerFrameRef_destroyHandsArray(csa);
                }

                return this.hands;
            }
        }

        public new bool IsValid
        {
            get
            {
                return base.IsValid && HandTrackerFrameRef_isValid(this.Handle);
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

                this.timestamp = HandTrackerFrameRef_getTimestamp(this.Handle);
                return this.timestamp.Value;
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
            HandTrackerFrameRef_release(this.Handle);
            this.gestures = null;
            this.Handle = IntPtr.Zero;
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