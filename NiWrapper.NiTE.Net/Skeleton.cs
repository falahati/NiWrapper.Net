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
