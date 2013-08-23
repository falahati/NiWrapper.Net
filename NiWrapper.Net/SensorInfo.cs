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
