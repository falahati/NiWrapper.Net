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

namespace NiTEWrapper
{
    #region

    #endregion

    public class UserData : NiTEBase
    {
        #region Constructors and Destructors

        internal UserData(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Public Methods and Operators

        public PoseData GetPose(PoseData.PoseType type)
        {
            if (!poses.ContainsKey(type))
            {
                poses[type] = new PoseData(UserData_getPose(Handle, type));
            }

            return poses[type];
        }

        #endregion

        #region Fields

        private readonly Dictionary<PoseData.PoseType, PoseData> poses = new Dictionary<PoseData.PoseType, PoseData>();

        private object boundingBox;

        private object centerOfMass;

        private Skeleton skeleton;

        private short? userId;

        private bool? isLost;

        private bool? isNew;

        private bool? isVisible;

        #endregion

        #region Public Properties

        public Rect3D BoundingBox
        {
            get
            {
                if (boundingBox == null)
                {
                    float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
                    UserData_getBoundingBox(Handle, ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ);
                    boundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
                }

                return (Rect3D) boundingBox;
            }
        }

        public Point3D CenterOfMass
        {
            get
            {
                if (centerOfMass == null)
                {
                    float x = 0, y = 0, z = 0;
                    UserData_getCenterOfMass(Handle, ref x, ref y, ref z);
                    centerOfMass = new Point3D(x, y, z);
                }

                return (Point3D) centerOfMass;
            }
        }

        public Skeleton Skeleton
        {
            get => skeleton ?? (skeleton = new Skeleton(UserData_getSkeleton(Handle)));
        }

        public short UserId
        {
            get
            {
                if (userId == null)
                {
                    userId = UserData_getId(Handle);
                }

                return userId.Value;
            }
        }

        public bool IsLost
        {
            get
            {
                if (isLost == null)
                {
                    isLost = UserData_isLost(Handle);
                }

                return isLost.Value;
            }
        }

        public bool IsNew
        {
            get
            {
                if (isNew == null)
                {
                    isNew = UserData_isNew(Handle);
                }

                return isNew.Value;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (isVisible == null)
                {
                    isVisible = UserData_isVisible(Handle);
                }

                return isVisible.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserData_getBoundingBox(
            IntPtr objectHandler,
            ref float minX,
            ref float minY,
            ref float minZ,
            ref float maxX,
            ref float maxY,
            ref float maxZ);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserData_getCenterOfMass(
            IntPtr objectHandler,
            ref float x,
            ref float y,
            ref float z);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern short UserData_getId(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserData_getPose(IntPtr objectHandler, PoseData.PoseType type);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserData_getSkeleton(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isLost(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isNew(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isVisible(IntPtr objectHandler);

        #endregion
    }
}