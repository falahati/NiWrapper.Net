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
using System.Windows.Media.Media3D;
using System.Drawing;
using OpenNIWrapper;

namespace NiTEWrapper
{
    public class HandTrackerFrameRef : NiTEBase, IDisposable
    {
        internal HandTrackerFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~HandTrackerFrameRef()
        {
            try
            {
                Dispose();
            }
            catch (Exception)
            { }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void HandTrackerFrameRef_release(IntPtr objectHandler);
        public void Release()
        {
            HandTrackerFrameRef_release(this.Handle);
            _gestures = null;
            base.Handle = IntPtr.Zero;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HandTrackerFrameRef_getDepthFrame(IntPtr objectHandler);
        OpenNIWrapper.VideoFrameRef _DepthFrame = null;
        public OpenNIWrapper.VideoFrameRef DepthFrame
        {
            get
            {
                if (_DepthFrame == null)
                    _DepthFrame = new OpenNIWrapper.VideoFrameRef(
                                    HandTrackerFrameRef_getDepthFrame(this.Handle));
                return _DepthFrame;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool HandTrackerFrameRef_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && HandTrackerFrameRef_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int HandTrackerFrameRef_getFrameIndex(IntPtr objectHandler);
        int? _FrameIndex;
        public int FrameIndex
        {
            get
            {
                if (_FrameIndex != null)
                    return _FrameIndex.Value;

                _FrameIndex = HandTrackerFrameRef_getFrameIndex(this.Handle);
                return _FrameIndex.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt64 HandTrackerFrameRef_getTimestamp(IntPtr objectHandler);
        UInt64? _Timestamp;
        public UInt64 Timestamp
        {
            get
            {
                if (_Timestamp != null)
                    return _Timestamp.Value;

                _Timestamp = HandTrackerFrameRef_getTimestamp(this.Handle);
                return _Timestamp.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern WrapperArray HandTrackerFrameRef_getHands(IntPtr vf);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HandTrackerFrameRef_destroyHandsArray(WrapperArray array);
        HandData[] _hands = null;
        public HandData[] Hands
        {
            get
            {
                if (_hands == null)
                {
                    WrapperArray csa = HandTrackerFrameRef_getHands(this.Handle);
                    IntPtr[] array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    _hands = new HandData[csa.Size];
                    for (int i = 0; i < csa.Size; i++)
                        _hands[i] = new HandData(array[i]);
                    HandTrackerFrameRef_destroyHandsArray(csa);
                }
                return _hands;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern WrapperArray HandTrackerFrameRef_getGestures(IntPtr vf);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HandTrackerFrameRef_destroyGesturesArray(WrapperArray array);
        GestureData[] _gestures = null;
        public GestureData[] Gestures
        {
            get
            {
                if (_gestures == null)
                {
                    WrapperArray csa = HandTrackerFrameRef_getGestures(this.Handle);
                    IntPtr[] array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    _gestures = new GestureData[csa.Size];
                    for (int i = 0; i < csa.Size; i++)
                        _gestures[i] = new GestureData(array[i]);
                    HandTrackerFrameRef_destroyGesturesArray(csa);
                }
                return _gestures;
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
                if (disposing && this.isValid)
                    this.Release();

                this.Handle = IntPtr.Zero;
                _disposed = true;
            }
        }
    }
}
