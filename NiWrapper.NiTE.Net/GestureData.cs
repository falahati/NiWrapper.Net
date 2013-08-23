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
using System.Windows.Media.Media3D;

namespace NiTEWrapper
{
    public class GestureData : NiTEBase
    {
        public const int GESTURE_COUNT = 3;
        public enum GestureType
        {
            WAVE,
            CLICK,
            HAND_RAISE
        }

        internal GestureData(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GestureData_getCurrentPosition(IntPtr objectHandler,
            ref float x, ref float y, ref float z);
        object _CurrentPosition;
        public Point3D CurrentPosition
        {
            get
            {
                if (_CurrentPosition == null)
                {
                    float x = 0, y = 0, z = 0;
                    GestureData_getCurrentPosition(this.Handle, ref x, ref y, ref z);
                    _CurrentPosition = new Point3D(x, y, z);
                }
                return (Point3D)_CurrentPosition;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern GestureType GestureData_getType(IntPtr objectHandler);
        GestureType? _Type;
        public GestureType Type
        {
            get
            {
                if (_Type == null)
                    _Type = GestureData_getType(this.Handle);
                return _Type.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool GestureData_isComplete(IntPtr objectHandler);
        bool? _isComplete;
        public bool isComplete
        {
            get
            {
                if (_isComplete == null)
                    _isComplete = GestureData_isComplete(this.Handle);
                return _isComplete.Value;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool GestureData_isInProgress(IntPtr objectHandler);
        bool? _isInProgress;
        public bool isInProgress
        {
            get
            {
                if (_isInProgress == null)
                    _isInProgress = GestureData_isInProgress(this.Handle);
                return _isInProgress.Value;
            }
        }
    }
}
