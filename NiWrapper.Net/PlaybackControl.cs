using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    public class PlaybackControl : OpenNIBase
    {
        internal PlaybackControl(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool PlaybackControl_isValid(IntPtr objectHandler);
        public new bool isValid
        {
            get
            {
                return base.isValid && PlaybackControl_isValid(this.Handle);
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int PlaybackControl_getNumberOfFrames(IntPtr objectHandler, IntPtr videoStream);
        public int getNumberOfFrames(VideoStream stream)
        {
            return PlaybackControl_getNumberOfFrames(this.Handle, stream.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status PlaybackControl_seek(IntPtr objectHandler, IntPtr videoStream, int frameIndex);
        public OpenNI.Status seek(VideoStream stream, int frameIndex)
        {
            return PlaybackControl_seek(this.Handle, stream.Handle, frameIndex);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool PlaybackControl_getRepeatEnabled(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status PlaybackControl_setRepeatEnabled(IntPtr objectHandler, bool isEnable);
        public bool Repeat
        {
            get
            {
                return PlaybackControl_getRepeatEnabled(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(PlaybackControl_setRepeatEnabled(this.Handle, value));
            }
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float PlaybackControl_getSpeed(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern OpenNI.Status PlaybackControl_setSpeed(IntPtr objectHandler, float speed);
        public float Speed
        {
            get
            {
                return PlaybackControl_getSpeed(this.Handle);
            }
            set
            {
                OpenNI.throwIfError(PlaybackControl_setSpeed(this.Handle, value));
            }
        }
    }
}
