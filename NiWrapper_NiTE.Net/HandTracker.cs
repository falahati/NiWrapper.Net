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
using System.Runtime.InteropServices;
using OpenNIWrapper;

namespace NiTEWrapper
{
    #region

    #endregion

    public class HandTracker : NiTEBase, IDisposable
    {
        #region Public Events

        public event HandTrackerListenerDelegate OnNewData;

        #endregion

        #region Constructors and Destructors

        private HandTracker(IntPtr handle)
        {
            Handle = handle;
            internalListener = PrivateNewData;
        }

        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            if (IsValid)
            {
                HandTracker_destroy(Handle);
                Common.DeleteObject(this);
                Handle = IntPtr.Zero;
            }
        }

        #region Fields

        private readonly HandTrackerListenerUnmanagedDelegate internalListener;

        // ReSharper disable once NotAccessedField.Local
#pragma warning disable 414
        private IntPtr handlerEvents;
#pragma warning restore 414

        #endregion

        #region Delegates

        public delegate void HandTrackerListenerDelegate(HandTracker handTracker);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void HandTrackerListenerUnmanagedDelegate(IntPtr handTracker);

        #endregion

        #region Public Properties

        public float SmoothingFactor
        {
            get => HandTracker_getSmoothingFactor(Handle);

            set => NiTE.ThrowIfError(HandTracker_setSmoothingFactor(Handle, value));
        }

        public new bool IsValid
        {
            get => base.IsValid && HandTracker_isValid(Handle);
        }

        #endregion

        #region Public Methods and Operators

        public static HandTracker Create(Device device = null)
        {
            var deviceHandle = IntPtr.Zero;

            if (device != null && device.IsValid)
            {
                deviceHandle = device.Handle;
            }

            IntPtr handle;
            NiTE.ThrowIfError(HandTracker_create(out handle, deviceHandle));
            var ut = new HandTracker(handle);
            ut.handlerEvents = HandTracker_RegisterListener(handle, ut.internalListener);

            return ut;
        }

        public NiTE.Status ConvertDepthCoordinatesToHand(int x, int y, int z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;

            return HandTracker_convertDepthCoordinatesToHand(Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertDepthCoordinatesToHand(Point3D depth)
        {
            float pX, pY;
            NiTE.ThrowIfError(
                ConvertDepthCoordinatesToHand((int) depth.X, (int) depth.Y, (int) depth.Z, out pX, out pY));

            return new PointF(pX, pY);
        }

        public NiTE.Status ConvertHandCoordinatesToDepth(float x, float y, float z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;

            return HandTracker_convertHandCoordinatesToDepth(Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertHandCoordinatesToDepth(Point3D depth)
        {
            float pX, pY;
            NiTE.ThrowIfError(
                ConvertHandCoordinatesToDepth(depth.X, depth.Y, depth.Z, out pX, out pY));

            return new PointF(pX, pY);
        }


        public NiTE.Status StartGestureDetection(GestureData.GestureType type)
        {
            return HandTracker_startGestureDetection(Handle, type);
        }

        public void StopGestureDetection(GestureData.GestureType type)
        {
            HandTracker_stopGestureDetection(Handle, type);
        }

        public void StopHandTracking(short handId)
        {
            HandTracker_stopHandTracking(Handle, handId);
        }

        public HandTrackerFrameRef ReadFrame()
        {
            IntPtr newFrame;
            NiTE.ThrowIfError(HandTracker_readFrame(Handle, out newFrame));

            return new HandTrackerFrameRef(newFrame);
        }

        public NiTE.Status StartHandTracking(Point3D position, out short handId)
        {
            handId = 0;

            return HandTracker_startHandTracking(
                Handle,
                position.X,
                position.Y,
                position.Z,
                ref handId);
        }

        public NiTE.Status StartHandTracking(Point3D position)
        {
            short handId;

            return StartHandTracking(position, out handId);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr HandTracker_RegisterListener(
            IntPtr objectHandler,
            [MarshalAs(UnmanagedType.FunctionPtr)] HandTrackerListenerUnmanagedDelegate listener);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_convertDepthCoordinatesToHand(
            IntPtr objectHandler,
            int x,
            int y,
            int z,
            ref float pX,
            ref float pY);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_convertHandCoordinatesToDepth(
            IntPtr objectHandler,
            float x,
            float y,
            float z,
            ref float pX,
            ref float pY);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_create(out IntPtr objectHandler, IntPtr device);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void HandTracker_destroy(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern float HandTracker_getSmoothingFactor(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandTracker_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_readFrame(IntPtr objectHandler, out IntPtr newFrame);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_setSmoothingFactor(IntPtr objectHandler, float value);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_startGestureDetection(
            IntPtr objectHandler,
            GestureData.GestureType type);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status HandTracker_startHandTracking(
            IntPtr objectHandler,
            float x,
            float y,
            float z,
            ref short handId);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void HandTracker_stopGestureDetection(IntPtr objectHandler, GestureData.GestureType type);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void HandTracker_stopHandTracking(IntPtr objectHandler, short handId);

        private void PrivateNewData(IntPtr handTracker)
        {
            var ev = OnNewData;

            if (ev != null)
            {
                ev(this);
            }
        }

        #endregion
    }
}