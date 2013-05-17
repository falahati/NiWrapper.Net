using System;
using System.Collections.Generic;
using System.Linq;
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
