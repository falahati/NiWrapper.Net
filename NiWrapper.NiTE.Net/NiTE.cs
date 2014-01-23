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
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using OpenNIWrapper;

    #endregion

    // ReSharper disable once InconsistentNaming
    public static class NiTE
    {
        #region Enums

        public enum Status
        {
            Ok, 

            Error, 

            BadUserId, 
        }

        #endregion

        #region Public Properties

        public static Version Version
        {
            get
            {
                OniVersion ver = NiTE_getVersion();
                return new Version(ver.Major, ver.Minor, ver.Maintenance, ver.Build);
            }
        }

        #endregion

        #region Public Methods and Operators

        public static Status Initialize()
        {
            return NiTE_initialize();
        }

        public static void Shutdown()
        {
            NiTE_shutdown();
        }

        #endregion

        #region Methods

        [DebuggerStepThrough]
        internal static void ThrowIfError(Status status)
        {
            switch (status)
            {
                case Status.Error:
                    throw new OpenNIException(OpenNI.LastError);
                case Status.BadUserId:
                    throw new ArgumentOutOfRangeException(OpenNI.LastError, new Exception("NiTE: Bad User ID"));
                default:
                    return;
            }
        }

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OniVersion NiTE_getVersion();

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Status NiTE_initialize();

        [DllImport("NiWrapper.NiTE.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NiTE_shutdown();

        #endregion
    }
}