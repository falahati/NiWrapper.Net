using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace NiTEWrapper
{
    public class UserData : NiTEBase
    {
        internal UserData(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserData_getBoundingBox(IntPtr objectHandler,
            ref float minX, ref float minY, ref float minZ, ref float maxX, ref float maxY, ref float maxZ);
        object _BoundingBox;
        public Rect3D BoundingBox
        {
            get
            {
                if (_BoundingBox == null)
                {
                    float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
                    UserData_getBoundingBox(this.Handle, ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ);
                    _BoundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
                }
                return (Rect3D)_BoundingBox;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserData_getCenterOfMass(IntPtr objectHandler,
            ref float x, ref float y, ref float z);
        object _CenterOfMass;
        public Point3D CenterOfMass
        {
            get
            {
                if (_CenterOfMass == null)
                {
                    float x = 0, y = 0, z = 0;
                    UserData_getCenterOfMass(this.Handle, ref x, ref y, ref z);
                    _CenterOfMass = new Point3D(x, y, z);
                }
                return (Point3D)_CenterOfMass;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern short UserData_getId(IntPtr objectHandler);
        short? _UserId;
        public short UserId
        {
            get
            {
                if (_UserId == null)
                    _UserId = UserData_getId(this.Handle);
                return _UserId.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool UserData_isLost(IntPtr objectHandler);
        bool? _isLost;
        public bool isLost
        {
            get
            {
                if (_isLost == null)
                    _isLost = UserData_isLost(this.Handle);
                return _isLost.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool UserData_isNew(IntPtr objectHandler);
        bool? _isNew;
        public bool isNew
        {
            get
            {
                if (_isNew == null)
                    _isNew = UserData_isNew(this.Handle);
                return _isNew.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool UserData_isVisible(IntPtr objectHandler);
        bool? _isVisible;
        public bool isVisible
        {
            get
            {
                if (_isVisible == null)
                    _isVisible = UserData_isVisible(this.Handle);
                return _isVisible.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserData_getPose(IntPtr objectHandler, PoseData.PoseType type);
        Dictionary<PoseData.PoseType, PoseData> _poses = new Dictionary<PoseData.PoseType, PoseData>();
        public PoseData getPose(PoseData.PoseType type)
        {
            if (!_poses.ContainsKey(type))
                _poses[type] = new PoseData(UserData_getPose(this.Handle, type));
            return _poses[type];
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserData_getSkeleton(IntPtr objectHandler);
        Skeleton _Skeleton;
        public Skeleton Skeleton
        {
            get
            {
                if (_Skeleton == null)
                    _Skeleton = new Skeleton(UserData_getSkeleton(this.Handle));
                return _Skeleton;
            }
        }
    }
}
