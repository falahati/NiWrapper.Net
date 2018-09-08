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

    public class PoseData : NiTEBase
    {
        #region Enums

        public enum PoseType
        {
            Psi,

            CrossedHands
        }

        #endregion

        #region Constants

        public const int PoseCount = 2;

        #endregion

        #region Constructors and Destructors

        internal PoseData(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Fields

        private PoseType? type;

        private bool? isEntered;

        private bool? isExited;

        private bool? isHeld;

        #endregion

        #region Public Properties

        public PoseType Type
        {
            get
            {
                if (type == null)
                {
                    type = PoseData_getType(Handle);
                }

                return type.Value;
            }
        }

        public bool IsEntered
        {
            get
            {
                if (isEntered == null)
                {
                    isEntered = PoseData_isEntered(Handle);
                }

                return isEntered.Value;
            }
        }

        public bool IsExited
        {
            get
            {
                if (isExited == null)
                {
                    isExited = PoseData_isExited(Handle);
                }

                return isExited.Value;
            }
        }

        public bool IsHeld
        {
            get
            {
                if (isHeld == null)
                {
                    isHeld = PoseData_isHeld(Handle);
                }

                return isHeld.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern PoseType PoseData_getType(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isEntered(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isExited(IntPtr objectHandler);

        [DllImport("NiWrapper_NiTE", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isHeld(IntPtr objectHandler);

        #endregion
    }
}