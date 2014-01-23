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
    using System.Windows.Media.Media3D;

    #endregion

    public class GestureData : NiTEBase
    {
        #region Constants

        public const int GestureCount = 3;

        #endregion

        #region Fields

        private object currentPosition;

        private GestureType? type;

        private bool? isComplete;

        private bool? isInProgress;

        #endregion

        #region Constructors and Destructors

        internal GestureData(IntPtr handle)
        {
            this.Handle = handle;
        }

        #endregion

        #region Enums

        public enum GestureType
        {
            Wave, 

            Click, 

            HandRaise
        }

        #endregion

        #region Public Properties

        public Point3D CurrentPosition
        {
            get
            {
                if (this.currentPosition == null)
                {
                    float x = 0, y = 0, z = 0;
                    GestureData_getCurrentPosition(this.Handle, ref x, ref y, ref z);
                    this.currentPosition = new Point3D(x, y, z);
                }

                return (Point3D)this.currentPosition;
            }
        }

        public GestureType Type
        {
            get
            {
                if (this.type == null)
                {
                    this.type = GestureData_getType(this.Handle);
                }

                return this.type.Value;
            }
        }

        public bool IsComplete
        {
            get
            {
                if (this.isComplete == null)
                {
                    this.isComplete = GestureData_isComplete(this.Handle);
                }

                return this.isComplete.Value;
            }
        }

        public bool IsInProgress
        {
            get
            {
                if (this.isInProgress == null)
                {
                    this.isInProgress = GestureData_isInProgress(this.Handle);
                }

                return this.isInProgress.Value;
            }
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GestureData_getCurrentPosition(
            IntPtr objectHandler, 
            ref float x, 
            ref float y, 
            ref float z);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern GestureType GestureData_getType(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool GestureData_isComplete(IntPtr objectHandler);

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool GestureData_isInProgress(IntPtr objectHandler);

        #endregion
    }
}