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
using System.Runtime.InteropServices;
using OpenNIWrapper;

namespace NiTEWrapper
{
    #region

    #endregion

    public sealed class UserTrackerFrameRef : NiTEBase, IDisposable
    {
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Public Methods and Operators

        public UserData GetUserById(short userId)
        {
            if (!usersById.ContainsKey(userId))
            {
                usersById[userId] = new UserData(UserTrackerFrameRef_getUserById(Handle, userId));
            }

            return usersById[userId];
        }

        #endregion

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();

            if (disposing)
            {
                depthFrame?.Dispose();
                users = null;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (IsValid)
            {
                UserTrackerFrameRef_release(Handle);
                Handle = IntPtr.Zero;
            }
        }

        #region Fields

        private readonly Dictionary<int, UserData> usersById = new Dictionary<int, UserData>();

        private VideoFrameRef depthFrame;

        private object floor;

        private float? floorConfidence;

        private int? frameIndex;

        private ulong? timestamp;

        private UserMap userMap;

        private UserData[] users;

        #endregion

        #region Constructors and Destructors

        internal UserTrackerFrameRef(IntPtr handle)
        {
            Handle = handle;
        }

        ~UserTrackerFrameRef()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public VideoFrameRef DepthFrame
        {
            get => depthFrame ?? (depthFrame = new VideoFrameRef(UserTrackerFrameRef_getDepthFrame(Handle)));
        }

        public Plane Floor
        {
            get
            {
                if (floor == null)
                {
                    float px = 0, py = 0, pz = 0, nx = 0, ny = 0, nz = 0;
                    UserTrackerFrameRef_getFloor(Handle, ref px, ref py, ref pz, ref nx, ref ny, ref nz);
                    floor = new Plane {Normal = new Vector3D(nx, ny, nz), Point = new Point3D(px, py, pz)};
                }

                return (Plane) floor;
            }
        }

        public float FloorConfidence
        {
            get
            {
                if (floorConfidence == null)
                {
                    floorConfidence = UserTrackerFrameRef_getFloorConfidence(Handle);
                }

                return floorConfidence.Value;
            }
        }

        public int FrameIndex
        {
            get
            {
                if (frameIndex != null)
                {
                    return frameIndex.Value;
                }

                frameIndex = UserTrackerFrameRef_getFrameIndex(Handle);

                return frameIndex.Value;
            }
        }

        public new bool IsValid
        {
            get => base.IsValid && UserTrackerFrameRef_isValid(Handle);
        }

        public ulong Timestamp
        {
            get
            {
                if (timestamp != null)
                {
                    return timestamp.Value;
                }

                timestamp = UserTrackerFrameRef_getTimestamp(Handle);

                return timestamp.Value;
            }
        }

        public UserMap UserMap
        {
            get
            {
                if (userMap == null)
                {
                    return userMap = new UserMap(UserTrackerFrameRef_getUserMap(Handle));
                }

                return userMap;
            }
        }

        public IEnumerable<UserData> Users
        {
            get
            {
                if (users == null)
                {
                    var csa = UserTrackerFrameRef_getUsers(Handle);
                    var array = new IntPtr[csa.Size];
                    Marshal.Copy(csa.Data, array, 0, csa.Size);
                    users = new UserData[csa.Size];

                    for (var i = 0; i < csa.Size; i++)
                    {
                        users[i] = new UserData(array[i]);
                    }

                    UserTrackerFrameRef_destroyUsersArray(csa);
                }

                return users;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_destroyUsersArray(WrapperArray array);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getDepthFrame(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTrackerFrameRef_getFloor(
            IntPtr objectHandler,
            ref float px,
            ref float py,
            ref float pz,
            ref float nx,
            ref float ny,
            ref float nz);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern float UserTrackerFrameRef_getFloorConfidence(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern int UserTrackerFrameRef_getFrameIndex(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong UserTrackerFrameRef_getTimestamp(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getUserById(IntPtr objectHandler, short userId);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserTrackerFrameRef_getUserMap(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern WrapperArray UserTrackerFrameRef_getUsers(IntPtr vf);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserTrackerFrameRef_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserTrackerFrameRef_release(IntPtr objectHandler);

        #endregion
    }
}