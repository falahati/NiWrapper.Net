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

    public class HandData : NiTEBase
    {
        #region Constructors and Destructors

        internal HandData(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Fields

        private short? handId;

        private object position;

        private bool? isLost;

        private bool? isNew;

        private bool? isTouchingFov;

        private bool? isTracking;

        #endregion

        #region Public Properties

        public short HandId
        {
            get
            {
                if (handId == null)
                {
                    handId = HandData_getId(Handle);
                }

                return handId.Value;
            }
        }

        public Point3D Position
        {
            get
            {
                if (position == null)
                {
                    float x = 0, y = 0, z = 0;
                    HandData_getPosition(Handle, ref x, ref y, ref z);
                    position = new Point3D(x, y, z);
                }

                return (Point3D) position;
            }
        }

        public bool IsLost
        {
            get
            {
                if (isLost == null)
                {
                    isLost = HandData_isLost(Handle);
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
                    isNew = HandData_isNew(Handle);
                }

                return isNew.Value;
            }
        }

        public bool IsTouchingFov
        {
            get
            {
                if (isTouchingFov == null)
                {
                    isTouchingFov = HandData_isTouchingFov(Handle);
                }

                return isTouchingFov.Value;
            }
        }

        public bool IsTracking
        {
            get
            {
                if (isTracking == null)
                {
                    isTracking = HandData_isTracking(Handle);
                }

                return isTracking.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern short HandData_getId(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern void HandData_getPosition(IntPtr objectHandler, ref float x, ref float y, ref float z);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandData_isLost(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandData_isNew(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandData_isTouchingFov(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool HandData_isTracking(IntPtr objectHandler);

        #endregion
    }
}