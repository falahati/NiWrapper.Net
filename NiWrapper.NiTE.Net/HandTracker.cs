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
using OpenNIWrapper;
using System.Windows.Media.Media3D;
using System.Drawing;
namespace NiTEWrapper
{
    public class HandTracker : NiTEBase
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void handTrackerListener(IntPtr uTracker);
        public delegate void HandTrackerListener(HandTracker uTracker);
        private handTrackerListener internal_listener;
        public event HandTrackerListener onNewData;
        private HandTracker(IntPtr handle)
        {
            this.Handle = handle;
            this.internal_listener = new handTrackerListener(this.Internal_NewData);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void HandTracker_destroy(IntPtr objectHandler);
        public void Destroy()
        {
            HandTracker_destroy(this.Handle);
            Common.DeleteObject(this);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_create(out IntPtr objectHandler, IntPtr device);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HandTracker_RegisterListener(
            IntPtr objectHandler, [MarshalAs(UnmanagedType.FunctionPtr)]handTrackerListener listener);
        IntPtr handler_events;
        public static HandTracker Create(Device device = null)
        {
            IntPtr deviceHandle = IntPtr.Zero;
            if (device != null && device.IsValid)
                deviceHandle = device.Handle;
            IntPtr handle;
            NiTE.throwIfError(HandTracker_create(out handle, deviceHandle));
            HandTracker ut = new HandTracker(handle);
            ut.handler_events = HandTracker_RegisterListener(handle, ut.internal_listener);
            return ut;
        }

        private void Internal_NewData(IntPtr uTracker)
        {
            HandTrackerListener ev = onNewData;
            if (ev != null)
                //if (this.Equals(uTracker))
                    ev(this);
                //else
                //  ev(new VideoStream(uTracker));
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float HandTracker_getSmoothingFactor(IntPtr objectHandler);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_setSmoothingFactor(IntPtr objectHandler, float value);
        public float SmoothingFactor
        {
            get
            {
                return HandTracker_getSmoothingFactor(this.Handle);
            }
            set
            {
                NiTE.throwIfError(HandTracker_setSmoothingFactor(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_convertDepthCoordinatesToHand(IntPtr objectHandler,
            int x, int y, int z, ref float pX, ref float pY);
        public NiTE.Status ConvertDepthCoordinatesToHand(int x, int y, int z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return HandTracker_convertDepthCoordinatesToHand(this.Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertDepthCoordinatesToHand(Point3D depth)
        {
            float pX, pY;
            NiTE.throwIfError(ConvertDepthCoordinatesToHand((int)depth.X, (int)depth.Y, (int)depth.Z, out pX, out pY));
            return new PointF(pX, pY);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_convertHandCoordinatesToDepth(IntPtr objectHandler,
            float x, float y, float z, ref float pX, ref float pY);
        public NiTE.Status ConvertHandCoordinatesToDepth(float x, float y, float z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return HandTracker_convertHandCoordinatesToDepth(this.Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertHandCoordinatesToDepth(Point3D depth)
        {
            float pX, pY;
            NiTE.throwIfError(ConvertHandCoordinatesToDepth((float)depth.X, (float)depth.Y, (float)depth.Z, out pX, out pY));
            return new PointF(pX, pY);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_startHandTracking(IntPtr objectHandler, float x, float y, float z, ref short HandId);
        public NiTE.Status startHandTracking(Point3D position, out short HandId)
        {
            HandId = 0;
            return HandTracker_startHandTracking(this.Handle, (float)position.X, (float)position.Y, (float)position.Z, ref HandId);
        }

        public NiTE.Status startHandTracking(Point3D position)
        {
            short HandId = 0;
            return startHandTracking(position, out HandId);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void HandTracker_stopHandTracking(IntPtr objectHandler, short HandId);
        public void StopHandTracking(short HandId)
        {
            HandTracker_stopHandTracking(this.Handle, HandId);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_startGestureDetection(IntPtr objectHandler, GestureData.GestureType type);
        public NiTE.Status StartGestureDetection(GestureData.GestureType type)
        {
            return HandTracker_startGestureDetection(this.Handle, type);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void HandTracker_stopGestureDetection(IntPtr objectHandler, GestureData.GestureType type);
        public void StopGestureDetection(GestureData.GestureType type)
        {
            HandTracker_stopGestureDetection(this.Handle, type);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool HandTracker_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && HandTracker_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status HandTracker_readFrame(IntPtr objectHandler, out IntPtr newFrame);
        public HandTrackerFrameRef readFrame()
        {
            IntPtr fHandle;
            NiTE.throwIfError(HandTracker_readFrame(this.Handle, out fHandle));
            return new HandTrackerFrameRef(fHandle);
        }
    }
}
