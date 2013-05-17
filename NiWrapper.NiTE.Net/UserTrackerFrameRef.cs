using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    class UserTrackerFrameRef : NiTEBase, IDisposable
    {
        internal UserTrackerFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~UserTrackerFrameRef()
        {
            try
            {
                Dispose();
                Common.DeleteObject(this);
            }
            catch (Exception)
            { }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserTrackerFrameRef_release(IntPtr objectHandler);
        public void Release()
        {
            UserTrackerFrameRef_release(this.Handle);
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserTrackerFrameRef_getDepthFrame(IntPtr objectHandler);
        OpenNIWrapper.VideoStream _DepthFrame = null;
        public OpenNIWrapper.VideoStream DepthFrame
        {
            get
            {
                if (_DepthFrame == null)
                _DepthFrame = new OpenNIWrapper.VideoStream(
                                UserTrackerFrameRef_getDepthFrame(this.Handle));
                return _DepthFrame;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void UserTrackerFrameRef_getFloor(IntPtr objectHandler,
            ref float Px, ref float Py, ref float Pz, ref float Nx, ref float Ny, ref float Nz);
        Plane? _Floor;
        public Plane Floor
        {
            get
            {
                if (_Floor == null)
                {
                    float Px = 0, Py = 0, Pz = 0, Nx = 0, Ny = 0, Nz = 0;
                    UserTrackerFrameRef_getFloor(this.Handle, ref Px, ref Py, ref Pz, ref Nx, ref Ny, ref Nz);
                    _Floor = new Plane()
                    {
                        normal = new System.Windows.Media.Media3D.Vector3D(Nx, Ny, Nz),
                        point = new System.Windows.Media.Media3D.Point3D(Px, Py, Pz)
                    };
                }
                return _Floor.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float UserTrackerFrameRef_getFloorConfidence(IntPtr objectHandler);
        float? _FloorConfidence;
        public float FloorConfidence
        {
            get
            {
                if (_FloorConfidence == null)
                    _FloorConfidence = UserTrackerFrameRef_getFloorConfidence(this.Handle);
                return _FloorConfidence.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool UserTrackerFrameRef_isValid(IntPtr objectHandler);
        bool? _isValid;
        public new bool isValid
        {
            get
            {
                if (_isValid == null)
                    _isValid = UserTrackerFrameRef_isValid(this.Handle);
                return _isValid.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int UserTrackerFrameRef_getFrameIndex(IntPtr objectHandler);
        int? _FrameIndex;
        public int FrameIndex
        {
            get
            {
                if (_FrameIndex != null)
                    return _FrameIndex.Value;

                _FrameIndex = UserTrackerFrameRef_getFrameIndex(this.Handle);
                return _FrameIndex.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt64 UserTrackerFrameRef_getTimestamp(IntPtr objectHandler);
        UInt64? _Timestamp;
        public UInt64 Timestamp
        {
            get
            {
                if (_Timestamp != null)
                    return _Timestamp.Value;

                _Timestamp = UserTrackerFrameRef_getTimestamp(this.Handle);
                return _Timestamp.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserTrackerFrameRef_getUserById(IntPtr objectHandler, short UserId);
        Dictionary<int, UserData> _usersById = new Dictionary<int,UserData>();
        public UserData getUserById(short userId)
        {
            if (!_usersById.ContainsKey(userId))
                _usersById[userId] = new UserData(
                    UserTrackerFrameRef_getUserById(this.Handle, userId));

            return _usersById[userId];
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserTrackerFrameRef_getUserMap(IntPtr objectHandler);
        UserMap _UserMap;
        public UserMap UserMap
        {
            get
            {
                if (_UserMap == null)
                    return _UserMap = new UserMap(UserTrackerFrameRef_getUserMap(this.Handle));

                return _UserMap;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern WrapperArray UserTrackerFrameRef_getUsers(IntPtr vf);
        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr UserTrackerFrameRef_destroyUsersArray(WrapperArray array);
        List<UserData> _users = null;
        public UserData[] Users()
        {
            if (_users == null)
            {
                WrapperArray csa = UserTrackerFrameRef_getUsers(this.Handle);
                IntPtr[] array = new IntPtr[csa.Size];
                Marshal.Copy(csa.Data, array, 0, csa.Size);
                UserData[] arrayObjects = new UserData[csa.Size];
                for (int i = 0; i < csa.Size; i++)
                    arrayObjects[i] = new UserData(array[i]);
                UserTrackerFrameRef_destroyUsersArray(csa);
                _users = new List<UserData>(arrayObjects);
            }
            return _users.ToArray();
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
