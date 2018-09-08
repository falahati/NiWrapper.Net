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

namespace OpenNIWrapper
{
    #region

    #endregion

    public class PlaybackControl : OpenNIBase
    {
        #region Constructors and Destructors

        internal PlaybackControl(IntPtr handle)
        {
            Handle = handle;
        }

        #endregion

        #region Public Properties

        public new bool IsValid
        {
            get => base.IsValid && PlaybackControl_isValid(Handle);
        }

        public bool Repeat
        {
            get => PlaybackControl_getRepeatEnabled(Handle);

            set => OpenNI.ThrowIfError(PlaybackControl_setRepeatEnabled(Handle, value));
        }

        public float Speed
        {
            get => PlaybackControl_getSpeed(Handle);

            set => OpenNI.ThrowIfError(PlaybackControl_setSpeed(Handle, value));
        }

        #endregion

        #region Public Methods and Operators

        public int GetNumberOfFrames(VideoStream stream)
        {
            return PlaybackControl_getNumberOfFrames(Handle, stream.Handle);
        }

        public OpenNI.Status Seek(VideoStream stream, int frameIndex)
        {
            return PlaybackControl_seek(Handle, stream.Handle, frameIndex);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern int PlaybackControl_getNumberOfFrames(IntPtr objectHandler, IntPtr videoStream);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PlaybackControl_getRepeatEnabled(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern float PlaybackControl_getSpeed(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool PlaybackControl_isValid(IntPtr objectHandler);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status PlaybackControl_seek(
            IntPtr objectHandler,
            IntPtr videoStream,
            int frameIndex);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status PlaybackControl_setRepeatEnabled(IntPtr objectHandler, bool isEnable);

        [DllImport("NiWrapper", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status PlaybackControl_setSpeed(IntPtr objectHandler, float speed);

        #endregion
    }
}