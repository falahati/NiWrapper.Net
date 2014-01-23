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

    public class PoseData : NiTEBase
    {
        #region Constants

        public const int PoseCount = 2;

        #endregion

        #region Fields

        private PoseType? type;

        private bool? isEntered;

        private bool? isExited;

        private bool? isHeld;

        #endregion

        #region Constructors and Destructors

        internal PoseData(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Enums

        public enum PoseType
        {
            Psi, 

            CrossedHands
        }

        #endregion

        #region Public Properties

        public PoseType Type
        {
            get
            {
                if (this.type == null)
                {
                    this.type = PoseData_getType(this.Handle);
                }

                return this.type.Value;
            }
        }

        public bool IsEntered
        {
            get
            {
                if (this.isEntered == null)
                {
                    this.isEntered = PoseData_isEntered(this.Handle);
                }

                return this.isEntered.Value;
            }
        }

        public bool IsExited
        {
            get
            {
                if (this.isExited == null)
                {
                    this.isExited = PoseData_isExited(this.Handle);
                }

                return this.isExited.Value;
            }
        }

        public bool IsHeld
        {
            get
            {
                if (this.isHeld == null)
                {
                    this.isHeld = PoseData_isHeld(this.Handle);
                }

                return this.isHeld.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern PoseType PoseData_getType(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isEntered(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isExited(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PoseData_isHeld(IntPtr objectHandler);

        #endregion
    }
}