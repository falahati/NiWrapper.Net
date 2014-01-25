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
    using System.Windows.Media.Media3D;

    using OpenNIWrapper;

    #endregion

    public sealed class UserTrackerFrameRef : NiTEBase, IDisposable
    {
        #region Fields

        private readonly Dictionary<int, UserData> usersById = new Dictionary<int, UserData>();

        private VideoFrameRef depthFrame;

        private object floor;

        private float? floorConfidence;

        private int? frameIndex;

        private bool isDisposed;

        private ulong? timestamp;

        private UserMap userMap;

        private UserData[] users;

        #endregion

        #region Constructors and Destructors

        internal UserTrackerFrameRef(IntPtr handle)
        {
            this.Handle = handle;
        }

        ~UserTrackerFrameRef()
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
                       ?? (this.depthFrame = new VideoFrameRef(UserTrackerFrameRef_getDepthFrame(this.Handle)));
            }
        }

        public Plane Floor
        {
            get
            {
                if (this.floor == null)
                {
                    float px = 0, py = 0, pz = 0, nx = 0, ny = 0, nz = 0;
                    UserTrackerFrameRef_getFloor(this.Handle, ref px, ref py, ref pz, ref nx, ref ny, ref nz);
                    this.floor = new Plane { Normal = new Vector3D(nx, ny, nz), Point = new Point3D(px, py, pz) };
                }

                return (Plane)this.floor;
            }
        }

        public float FloorConfidence
        {
            get
            {
                if (this.floorConfidence == null)
                {
                    this.floorConfidence = UserTrackerFrameRef_getFloorConfidence(this.Handle);
                }

                return this.floorConfidence.Value;
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

                this.frameIndex = UserTrackerFrameRef_getFrameIndex(this.Handle);
                return this.frameIndex.Value;
            }
        }

        public new bool IsValid
        {
            get
            {
                return base.IsValid && UserTrackerFrameRef_isValid(this.Handle);
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

                this.timestamp = UserTrackerFrameRef_getTimestamp(this.Handle);
                return this.timestamp.Value;
            }
        }

        public UserMap UserMap
        {
            get
            {
                if (this.userMap == null)
                {
                    return this.userMap = new UserMap(UserTrackerFrameRef_getUserMap(this.Handle));
                }

                return this.userMap;
            }
        }

        public IEnumerable<UserData> Users
        {
            get
            {
                if (this.users == null)
                {
                    WrapperArray csa = UserTrackerFrameRef_getUsers(this.Handle);
                    IntPtr[] array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    this.users = new UserData[csa.Size];
                    for (int i = 0; i < csa.Size; i++)
                    {
                        this.users[i] = new UserData(array[i]);
                    }

                    UserTrackerFrameRef_destroyUsersArray(csa);
                }

                return this.users;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public UserData GetUserById(short userId)
        {
            if (!this.usersById.ContainsKey(userId))
            {
                this.usersById[userId] = new UserData(UserTrackerFrameRef_getUserById(this.Handle, userId));
            }

            return this.usersById[userId];
        }

        public void Release()
        {
            UserTrackerFrameRef_release(this.Handle);
            this.users = null;
            this.Handle = IntPtr.Zero;
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_destroyUsersArray(WrapperArray array);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getDepthFrame(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTrackerFrameRef_getFloor(
            IntPtr objectHandler, 
            ref float px, 
            ref float py, 
            ref float pz, 
            ref float nx, 
            ref float ny, 
            ref float nz);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float UserTrackerFrameRef_getFloorConfidence(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UserTrackerFrameRef_getFrameIndex(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong UserTrackerFrameRef_getTimestamp(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getUserById(IntPtr objectHandler, short userId);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getUserMap(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrapperArray UserTrackerFrameRef_getUsers(IntPtr vf);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserTrackerFrameRef_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTrackerFrameRef_release(IntPtr objectHandler);

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