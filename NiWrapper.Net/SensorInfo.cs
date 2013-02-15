using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenNIWrapper
{
    public class SensorInfo : OpenNIBase
    {
        internal SensorInfo(IntPtr handle)
        {
            this.Handle = handle;
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern Device.SensorType SensorInfo_getSensorType(IntPtr objectHandler);
        public Device.SensorType getSensorType()
        {
            return SensorInfo_getSensorType(this.Handle);
        }

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern WrapperArray SensorInfo_getSupportedVideoModes(IntPtr objectHandler);
        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SensorInfo_destroyVideoModesArray(WrapperArray array);
        public VideoMode[] getSupportedVideoModes()
        {
            WrapperArray csa = SensorInfo_getSupportedVideoModes(this.Handle);
            IntPtr[] array = new IntPtr[csa.Size];
            Marshal.Copy(csa.Data, array, 0, csa.Size);
            VideoMode[] arrayObjects = new VideoMode[csa.Size];
            for (int i = 0; i < csa.Size; i++)
                arrayObjects[i] = new VideoMode(array[i], true);
            SensorInfo_destroyVideoModesArray(csa);
            return arrayObjects;
        }
        public override string ToString()
        {
            return this.getSensorType().ToString();
        }
    }
}
