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
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    #region

    #endregion

    public class GestureData : NiTEBase
    {
        #region Enums

        public enum GestureType
        {
            Wave,

            Click,

            HandRaise
        }

        #endregion

        #region Constants

        public const int GestureCount = 3;

        #endregion

        #region Constructors and Destructors

        internal GestureData(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Fields

        private object currentPosition;

        private GestureType? type;

        private bool? isComplete;

        private bool? isInProgress;

        #endregion

        #region Public Properties

        public Point3D CurrentPosition
        {
            get
            {
                if (currentPosition == null)
                {
                    float x = 0, y = 0, z = 0;
                    GestureData_getCurrentPosition(Handle, ref x, ref y, ref z);
                    currentPosition = new Point3D(x, y, z);
                }

                return (Point3D) currentPosition;
            }
        }

        public GestureType Type
        {
            get
            {
                if (type == null)
                {
                    type = GestureData_getType(Handle);
                }

                return type.Value;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (isComplete == null)
                {
                    isComplete = GestureData_isComplete(Handle);
                }

                return isComplete.Value;
            }
        }

        public bool IsInProgress
        {
            get
            {
                if (isInProgress == null)
                {
                    isInProgress = GestureData_isInProgress(Handle);
                }

                return isInProgress.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GestureData_getCurrentPosition(
            IntPtr objectHandler,
            ref float x,
            ref float y,
            ref float z);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern GestureType GestureData_getType(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool GestureData_isComplete(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool GestureData_isInProgress(IntPtr objectHandler);

        #endregion
    }
}