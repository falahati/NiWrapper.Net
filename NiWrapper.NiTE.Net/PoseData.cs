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
    public class PoseData : NiTEBase
    {
        public const int POSE_COUNT = 2;
        public enum PoseType
        {
            POSE_PSI,
            POSE_CROSSED_HANDS
        }

        internal PoseData(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern PoseType PoseData_getType(IntPtr objectHandler);
        PoseType? _Type;
        public PoseType Type
        {
            get
            {
                if (_Type == null)
                    _Type = PoseData_getType(this.Handle);
                return _Type.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool PoseData_isEntered(IntPtr objectHandler);
        bool? _isEntered;
        public bool isEntered
        {
            get
            {
                if (_isEntered == null)
                    _isEntered = PoseData_isEntered(this.Handle);
                return _isEntered.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool PoseData_isExited(IntPtr objectHandler);
        bool? _isExited;
        public bool isExited
        {
            get
            {
                if (_isExited == null)
                    _isExited = PoseData_isExited(this.Handle);
                return _isExited.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool PoseData_isHeld(IntPtr objectHandler);
        bool? _isHeld;
        public bool isHeld
        {
            get
            {
                if (_isHeld == null)
                    _isHeld = PoseData_isHeld(this.Handle);
                return _isHeld.Value;
            }
        }
    }
}
