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
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Media.Media3D;

    using OpenNIWrapper;

    #endregion

    public class UserTracker : NiTEBase
    {
        #region Fields

        private readonly UserTrackerListenerUnmanagedDelegate internalListener;

        // ReSharper disable once NotAccessedField.Local
        private IntPtr handlerEvents;

        #endregion

        #region Constructors and Destructors

        private UserTracker(IntPtr handle)
        {
            this.Handle = handle;
            this.internalListener = this.PrivateNewData;
        }

        #endregion

        #region Delegates

        public delegate void UserTrackerListenerDelegate(UserTracker userTracker);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UserTrackerListenerUnmanagedDelegate(IntPtr userTracker);

        #endregion

        #region Public Events

        public event UserTrackerListenerDelegate OnNewData;

        #endregion

        #region Public Properties

        public float SkeletonSmoothingFactor
        {
            get
            {
                return UserTracker_getSkeletonSmoothingFactor(this.Handle);
            }

            set
            {
                NiTE.ThrowIfError(UserTracker_setSkeletonSmoothingFactor(this.Handle, value));
            }
        }

        public new bool IsValid
        {
            get
            {
                return base.IsValid && UserTracker_isValid(this.Handle);
            }
        }

        #endregion

        #region Public Methods and Operators

        public static UserTracker Create(Device device = null)
        {
            IntPtr deviceHandle = IntPtr.Zero;
            if (device != null && device.IsValid)
            {
                deviceHandle = device.Handle;
            }

            IntPtr handle;
            NiTE.ThrowIfError(UserTracker_create(out handle, deviceHandle));
            UserTracker ut = new UserTracker(handle);
            ut.handlerEvents = UserTracker_RegisterListener(handle, ut.internalListener);
            return ut;
        }

        public NiTE.Status ConvertDepthCoordinatesToJoint(int x, int y, int z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return UserTracker_convertDepthCoordinatesToJoint(this.Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertDepthCoordinatesToJoint(Point3D depth)
        {
            float pX, pY;
            NiTE.ThrowIfError(
                this.ConvertDepthCoordinatesToJoint((int)depth.X, (int)depth.Y, (int)depth.Z, out pX, out pY));
            return new PointF(pX, pY);
        }

        public NiTE.Status ConvertJointCoordinatesToDepth(float x, float y, float z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return UserTracker_convertJointCoordinatesToDepth(this.Handle, x, y, z, ref pX, ref pY);
        }

        public PointF ConvertJointCoordinatesToDepth(Point3D depth)
        {
            float pX, pY;
            NiTE.ThrowIfError(
                this.ConvertJointCoordinatesToDepth((float)depth.X, (float)depth.Y, (float)depth.Z, out pX, out pY));
            return new PointF(pX, pY);
        }

        public void Destroy()
        {
            UserTracker_destroy(this.Handle);
            Common.DeleteObject(this);
        }

        public NiTE.Status StartPoseDetection(short userId, PoseData.PoseType type)
        {
            return UserTracker_startPoseDetection(this.Handle, userId, type);
        }

        public NiTE.Status StartSkeletonTracking(short userId)
        {
            return UserTracker_startSkeletonTracking(this.Handle, userId);
        }

        public void StopPoseDetection(short userId, PoseData.PoseType type)
        {
            UserTracker_stopPoseDetection(this.Handle, userId, type);
        }

        public void StopSkeletonTracking(short userId)
        {
            UserTracker_stopSkeletonTracking(this.Handle, userId);
        }

        public UserTrackerFrameRef ReadFrame()
        {
            IntPtr handle;
            NiTE.ThrowIfError(UserTracker_readFrame(this.Handle, out handle));
            return new UserTrackerFrameRef(handle);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTracker_RegisterListener(
            IntPtr objectHandler, 
            [MarshalAs(UnmanagedType.FunctionPtr)] UserTrackerListenerUnmanagedDelegate listener);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_convertDepthCoordinatesToJoint(
            IntPtr objectHandler, 
            int x, 
            int y, 
            int z, 
            ref float pX, 
            ref float pY);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_convertJointCoordinatesToDepth(
            IntPtr objectHandler, 
            float x, 
            float y, 
            float z, 
            ref float pX, 
            ref float pY);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_create(out IntPtr objectHandler, IntPtr device);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTracker_destroy(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float UserTracker_getSkeletonSmoothingFactor(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserTracker_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_readFrame(IntPtr objectHandler, out IntPtr newFrame);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_setSkeletonSmoothingFactor(IntPtr objectHandler, float value);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_startPoseDetection(
            IntPtr objectHandler, 
            short userId, 
            PoseData.PoseType type);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern NiTE.Status UserTracker_startSkeletonTracking(IntPtr objectHandler, short userId);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTracker_stopPoseDetection(
            IntPtr objectHandler, 
            short userId, 
            PoseData.PoseType type);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTracker_stopSkeletonTracking(IntPtr objectHandler, short userId);

        private void PrivateNewData(IntPtr usetTracker)
        {
            UserTrackerListenerDelegate ev = this.OnNewData;
            if (ev != null)
                ev(this);
        }

        #endregion
    }
}