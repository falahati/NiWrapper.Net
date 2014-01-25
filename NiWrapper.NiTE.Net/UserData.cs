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

    #endregion

    public class UserData : NiTEBase
    {
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

        #region Constructors and Destructors

        internal UserData(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Public Properties

        public Rect3D BoundingBox
        {
            get
            {
                if (this.boundingBox == null)
                {
                    float minX = 0, minY = 0, minZ = 0, maxX = 0, maxY = 0, maxZ = 0;
                    UserData_getBoundingBox(this.Handle, ref minX, ref minY, ref minZ, ref maxX, ref maxY, ref maxZ);
                    this.boundingBox = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
                }

                return (Rect3D)this.boundingBox;
            }
        }

        public Point3D CenterOfMass
        {
            get
            {
                if (this.centerOfMass == null)
                {
                    float x = 0, y = 0, z = 0;
                    UserData_getCenterOfMass(this.Handle, ref x, ref y, ref z);
                    this.centerOfMass = new Point3D(x, y, z);
                }

                return (Point3D)this.centerOfMass;
            }
        }

        public Skeleton Skeleton
        {
            get
            {
                return this.skeleton ?? (this.skeleton = new Skeleton(UserData_getSkeleton(this.Handle)));
            }
        }

        public short UserId
        {
            get
            {
                if (this.userId == null)
                {
                    this.userId = UserData_getId(this.Handle);
                }

                return this.userId.Value;
            }
        }

        public bool IsLost
        {
            get
            {
                if (this.isLost == null)
                {
                    this.isLost = UserData_isLost(this.Handle);
                }

                return this.isLost.Value;
            }
        }

        public bool IsNew
        {
            get
            {
                if (this.isNew == null)
                {
                    this.isNew = UserData_isNew(this.Handle);
                }

                return this.isNew.Value;
            }
        }

        public bool IsVisible
        {
            get
            {
                if (this.isVisible == null)
                {
                    this.isVisible = UserData_isVisible(this.Handle);
                }

                return this.isVisible.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public PoseData GetPose(PoseData.PoseType type)
        {
            if (!this.poses.ContainsKey(type))
            {
                this.poses[type] = new PoseData(UserData_getPose(this.Handle, type));
            }

            return this.poses[type];
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserData_getBoundingBox(
            IntPtr objectHandler, 
            ref float minX, 
            ref float minY, 
            ref float minZ, 
            ref float maxX, 
            ref float maxY, 
            ref float maxZ);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void UserData_getCenterOfMass(IntPtr objectHandler, ref float x, ref float y, ref float z);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short UserData_getId(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserData_getPose(IntPtr objectHandler, PoseData.PoseType type);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr UserData_getSkeleton(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isLost(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isNew(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool UserData_isVisible(IntPtr objectHandler);

        #endregion
    }
}