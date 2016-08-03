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
    using System.Runtime.InteropServices;

    #endregion

    public class SkeletonJoint : NiTEBase
    {
        #region Constants

        public const int JointCount = 15;

        #endregion

        #region Fields

        private object orientation;

        private float? orientationConfidence;

        private object position;

        private float? positionConfidence;

        private JointType? type;

        #endregion

        #region Constructors and Destructors

        internal SkeletonJoint(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Enums

        public enum JointType
        {
            Head, 

            Neck, 

            LeftShoulder, 

            RightShoulder, 

            LeftElbow, 

            RightElbow, 

            LeftHand, 

            RightHand, 

            Torso, 

            LeftHip, 

            RightHip, 

            LeftKnee, 

            RightKnee, 

            LeftFoot, 

            RightFoot, 
        }

        #endregion

        #region Public Properties

        public Quaternion Orientation
        {
            get
            {
                if (this.orientation == null)
                {
                    float x = 0, y = 0, z = 0, w = 0;
                    SkeletonJoint_getOrientation(this.Handle, ref x, ref y, ref z, ref w);
                    this.orientation = new Quaternion(x, y, z, w);
                }

                return (Quaternion)this.orientation;
            }
        }

        public float OrientationConfidence
        {
            get
            {
                if (this.orientationConfidence == null)
                {
                    this.orientationConfidence = SkeletonJoint_getOrientationConfidence(this.Handle);
                }

                return this.orientationConfidence.Value;
            }
        }

        public Point3D Position
        {
            get
            {
                if (this.position == null)
                {
                    float x = 0, y = 0, z = 0;
                    SkeletonJoint_getPosition(this.Handle, ref x, ref y, ref z);
                    this.position = new Point3D(x, y, z);
                }

                return (Point3D)this.position;
            }
        }

        public float PositionConfidence
        {
            get
            {
                if (this.positionConfidence == null)
                {
                    this.positionConfidence = SkeletonJoint_getPositionConfidence(this.Handle);
                }

                return this.positionConfidence.Value;
            }
        }

        public JointType Type
        {
            get
            {
                if (this.type == null)
                {
                    this.type = SkeletonJoint_getType(this.Handle);
                }

                return this.type.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SkeletonJoint_getOrientation(
            IntPtr objectHandler, 
            ref float x, 
            ref float y, 
            ref float z, 
            ref float w);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern float SkeletonJoint_getOrientationConfidence(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SkeletonJoint_getPosition(
            IntPtr objectHandler, 
            ref float x, 
            ref float y, 
            ref float z);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern float SkeletonJoint_getPositionConfidence(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern JointType SkeletonJoint_getType(IntPtr objectHandler);

        #endregion
    }
}