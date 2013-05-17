using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    public class Skeleton : NiTEBase
    {
        public enum SkeletonState
        {
            NONE,
            CALIBRATING,
            TRACKED,
            CALIBRATION_ERROR_NOT_IN_POSE,
            CALIBRATION_ERROR_HANDS,
            CALIBRATION_ERROR_HEAD,
            CALIBRATION_ERROR_LEGS,
            CALIBRATION_ERROR_TORSO
        }

        internal Skeleton(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern SkeletonState Skeleton_getState(IntPtr objectHandler);
        SkeletonState? _State;
        public SkeletonState State
        {
            get
            {
                if (_State == null)
                    _State = Skeleton_getState(this.Handle);
                return _State.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr Skeleton_getJoint(IntPtr objectHandler, SkeletonJoint.JointType type);
        Dictionary<SkeletonJoint.JointType, SkeletonJoint> _states = new Dictionary<SkeletonJoint.JointType,SkeletonJoint>();
        public SkeletonJoint getJoint(SkeletonJoint.JointType type)
        {
            if (!_states.ContainsKey(type))
                _states[type] = new SkeletonJoint(Skeleton_getJoint(this.Handle, type));
            return _states[type];
        }
    }
}
