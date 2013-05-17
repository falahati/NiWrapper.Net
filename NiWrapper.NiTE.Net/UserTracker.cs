using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenNIWrapper;

namespace NiTEWrapper
{
    public class UserTracker : NiTEBase
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void userTrackerListener(IntPtr uTracker);
        public delegate void UserTrackerListener(UserTracker uTracker);
        private userTrackerListener internal_listener;
        public event UserTrackerListener onNewData;
        public UserTracker(IntPtr handle)
        {
            this.Handle = handle;
            this.internal_listener = new userTrackerListener(this.Internal_NewData);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserTracker_destroy(IntPtr objectHandler);
        internal void Destroy()
        {
            UserTracker_destroy(this.Handle);
            Common.DeleteObject(this);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_create(out IntPtr objectHandler, IntPtr device);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserTracker_RegisterListener(
            IntPtr objectHandler, [MarshalAs(UnmanagedType.FunctionPtr)]userTrackerListener listener);
        IntPtr handler_events;
        internal static UserTracker Create(Device device)
        {
            IntPtr handle;
            NiTE.throwIfError(UserTracker_create(out handle, device.Handle));
            UserTracker ut = new UserTracker(handle);
            ut.handler_events = UserTracker_RegisterListener(handle, ut.internal_listener);
            return ut;
        }

        private void Internal_NewData(IntPtr uTracker)
        {
            UserTrackerListener ev = onNewData;
            if (ev != null)
                //if (this.Equals(uTracker))
                    ev(this);
                //else
                //  ev(new VideoStream(uTracker));
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float UserTracker_getSkeletonSmoothingFactor(IntPtr objectHandler);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_setSkeletonSmoothingFactor(IntPtr objectHandler, float value);
        public float SkeletonSmoothingFactor
        {
            get
            {
                return UserTracker_getSkeletonSmoothingFactor(this.Handle);
            }
            set
            {
                NiTE.throwIfError(UserTracker_setSkeletonSmoothingFactor(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_convertDepthCoordinatesToJoint(IntPtr objectHandler,
            int x, int y, int z, ref float pX, ref float pY);
        public NiTE.Status convertDepthCoordinatesToJoint(IntPtr objectHandler, int x, int y, int z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return UserTracker_convertDepthCoordinatesToJoint(this.Handle, x, y, z, ref pX, ref pY);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_convertJointCoordinatesToDepth(IntPtr objectHandler,
            float x, float y, float z, ref float pX, ref float pY);
        public NiTE.Status convertJointCoordinatesToDepth(IntPtr objectHandler, float x, float y, float z, out float pX, out float pY)
        {
            pX = 0;
            pY = 0;
            return UserTracker_convertJointCoordinatesToDepth(this.Handle, x, y, z, ref pX, ref pY);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_startSkeletonTracking(IntPtr objectHandler, short UserId);
        public NiTE.Status StartSkeletonTracking(short UserId)
        {
            return UserTracker_startSkeletonTracking(this.Handle, UserId);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserTracker_stopSkeletonTracking(IntPtr objectHandler, short UserId);
        public void Stop(short UserId)
        {
            UserTracker_stopSkeletonTracking(this.Handle, UserId);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_startPoseDetection(IntPtr objectHandler, short UserId, PoseData.PoseType type);
        public NiTE.Status StartSkeletonTracking(short UserId, PoseData.PoseType type)
        {
            return UserTracker_startPoseDetection(this.Handle, UserId, type);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserTracker_stopPoseDetection(IntPtr objectHandler, short UserId, PoseData.PoseType type);
        public void Stop(short UserId, PoseData.PoseType type)
        {
            UserTracker_stopPoseDetection(this.Handle, UserId, type);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool UserTracker_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && UserTracker_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern NiTE.Status UserTracker_readFrame(IntPtr objectHandler, out IntPtr newFrame);
        public UserTrackerFrameRef readFrame()
        {
            IntPtr fHandle;
            NiTE.throwIfError(UserTracker_readFrame(this.Handle, out fHandle));
            return new UserTrackerFrameRef(fHandle);
        }
    }
}
