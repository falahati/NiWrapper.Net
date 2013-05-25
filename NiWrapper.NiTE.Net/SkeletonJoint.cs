using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;
namespace NiTEWrapper
{
    public class SkeletonJoint : NiTEBase
    {
        public const int JOINT_COUNT = 15;
        public enum JointType
        {
            HEAD,
            NECK,
            LEFT_SHOULDER,
            RIGHT_SHOULDER,
            LEFT_ELBOW,
            RIGHT_ELBOW,
            LEFT_HAND,
            RIGHT_HAND,
            TORSO,
            LEFT_HIP,
            RIGHT_HIP,
            LEFT_KNEE,
            RIGHT_KNEE,
            LEFT_FOOT,
            RIGHT_FOOT,
        }

        internal SkeletonJoint(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void SkeletonJoint_getOrientation(IntPtr objectHandler,
            ref float x, ref float y, ref float z, ref float w);
        object _Orientation;
        public Quaternion Orientation
        {
            get
            {
                if (_Orientation == null)
                {
                    float x = 0, y = 0, z = 0, w = 0;
                    SkeletonJoint_getOrientation(this.Handle, ref x, ref y, ref z, ref w);
                    _Orientation = new Quaternion(x, y, z, w);
                }
                return (Quaternion)_Orientation;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void SkeletonJoint_getPosition(IntPtr objectHandler,
            ref float x, ref float y, ref float z);
        object _Position;
        public Point3D Position
        {
            get
            {
                if (_Position == null)
                {
                    float x = 0, y = 0, z = 0;
                    SkeletonJoint_getPosition(this.Handle, ref x, ref y, ref z);
                    _Position = new Point3D(x, y, z);
                }
                return (Point3D)_Position;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float SkeletonJoint_getOrientationConfidence(IntPtr objectHandler);
        float? _OrientationConfidence;
        public float OrientationConfidence
        {
            get
            {
                if (_OrientationConfidence == null)
                    _OrientationConfidence = SkeletonJoint_getOrientationConfidence(this.Handle);
                return _OrientationConfidence.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float SkeletonJoint_getPositionConfidence(IntPtr objectHandler);
        float? _PositionConfidence;
        public float PositionConfidence
        {
            get
            {
                if (_PositionConfidence == null)
                    _PositionConfidence = SkeletonJoint_getPositionConfidence(this.Handle);
                return _PositionConfidence.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern JointType SkeletonJoint_getType(IntPtr objectHandler);
        JointType? _Type;
        public JointType Type
        {
            get
            {
                if (_Type == null)
                    _Type = SkeletonJoint_getType(this.Handle);
                return _Type.Value;
            }
        }
    }
}
