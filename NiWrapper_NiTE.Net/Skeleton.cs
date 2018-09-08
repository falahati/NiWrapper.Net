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

    public class Skeleton : NiTEBase
    {
        #region Enums

        public enum SkeletonState
        {
            None,

            Calibrating,

            Tracked,

            CalibrationErrorNotInPose,

            CalibrationErrorHands,

            CalibrationErrorHead,

            CalibrationErrorLegs,

            CalibrationErrorTorso
        }

        #endregion

        #region Constructors and Destructors

        internal Skeleton(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Public Properties

        public SkeletonState State
        {
            get
            {
                if (state == null)
                {
                    state = Skeleton_getState(Handle);
                }

                return state.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public SkeletonJoint GetJoint(SkeletonJoint.JointType type)
        {
            if (!states.ContainsKey(type))
            {
                states[type] = new SkeletonJoint(Skeleton_getJoint(Handle, type));
            }

            return states[type];
        }

        #endregion

        #region Fields

        private readonly Dictionary<SkeletonJoint.JointType, SkeletonJoint> states =
            new Dictionary<SkeletonJoint.JointType, SkeletonJoint>();

        private SkeletonState? state;

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Skeleton_getJoint(IntPtr objectHandler, SkeletonJoint.JointType type);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern SkeletonState Skeleton_getState(IntPtr objectHandler);

        #endregion
    }
}