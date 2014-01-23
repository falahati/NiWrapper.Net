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
namespace OpenNIWrapper
{
    #region

    using System;
    using System.Runtime.InteropServices;

    #endregion

    public static class CoordinateConverter
    {
        #region Public Methods and Operators

        public static OpenNI.Status ConvertDepthToColor(
            VideoStream depthStream, 
            VideoStream colorStream, 
            int depthX, 
            int depthY, 
            ushort depthZ, 
            out int colorX, 
            out int colorY)
        {
            colorX = 0;
            colorY = 0;
            return CoordinateConverter_convertDepthToColor(
                depthStream.Handle, 
                colorStream.Handle, 
                depthX, 
                depthY, 
                depthZ, 
                ref colorX, 
                ref colorY);
        }

        public static OpenNI.Status ConvertDepthToWorld(
            VideoStream depthStream, 
            float depthX, 
            float depthY, 
            float depthZ, 
            out float worldX, 
            out float worldY, 
            out float worldZ)
        {
            worldX = 0;
            worldY = 0;
            worldZ = 0;
            return CoordinateConverter_convertDepthToWorld_Float(
                depthStream.Handle, 
                depthX, 
                depthY, 
                depthZ, 
                ref worldX, 
                ref worldY, 
                ref worldZ);
        }

        public static OpenNI.Status ConvertWorldToDepth(
            VideoStream depthStream, 
            float worldX, 
            float worldY, 
            float worldZ, 
            out int depthX, 
            out int depthY, 
            out ushort depthZ)
        {
            depthX = 0;
            depthY = 0;
            depthZ = 0;
            return CoordinateConverter_convertWorldToDepth(
                depthStream.Handle, 
                worldX, 
                worldY, 
                worldZ, 
                ref depthX, 
                ref depthY, 
                ref depthZ);
        }

        public static OpenNI.Status ConvertWorldToDepth(
            VideoStream depthStream, 
            float worldX, 
            float worldY, 
            float worldZ, 
            out float depthX, 
            out float depthY, 
            out float depthZ)
        {
            depthX = 0;
            depthY = 0;
            depthZ = 0;
            return CoordinateConverter_convertWorldToDepth_Float(
                depthStream.Handle, 
                worldX, 
                worldY, 
                worldZ, 
                ref depthX, 
                ref depthY, 
                ref depthZ);
        }

        public static OpenNI.Status ConvertDepthToWorld(
            VideoStream depthStream, 
            int depthX, 
            int depthY, 
            ushort depthZ, 
            out float worldX, 
            out float worldY, 
            out float worldZ)
        {
            worldX = 0;
            worldY = 0;
            worldZ = 0;
            return CoordinateConverter_convertDepthToWorld(
                depthStream.Handle, 
                depthX, 
                depthY, 
                depthZ, 
                ref worldX, 
                ref worldY, 
                ref worldZ);
        }

        #endregion

        #region Methods

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CoordinateConverter_convertDepthToColor(
            IntPtr depthStream, 
            IntPtr colorStream, 
            int depthX, 
            int depthY, 
            ushort depthZ, 
            ref int colorX, 
            ref int colorY);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CoordinateConverter_convertDepthToWorld(
            IntPtr depthStream, 
            int depthX, 
            int depthY, 
            ushort depthZ, 
            ref float worldX, 
            ref float worldY, 
            ref float worldZ);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CoordinateConverter_convertDepthToWorld_Float(
            IntPtr depthStream, 
            float depthX, 
            float depthY, 
            float depthZ, 
            ref float worldX, 
            ref float worldY, 
            ref float worldZ);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CoordinateConverter_convertWorldToDepth(
            IntPtr depthStream, 
            float worldX, 
            float worldY, 
            float worldZ, 
            ref int depthX, 
            ref int depthY, 
            ref ushort depthZ);

        [DllImport("NiWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpenNI.Status CoordinateConverter_convertWorldToDepth_Float(
            IntPtr depthStream, 
            float worldX, 
            float worldY, 
            float worldZ, 
            ref float depthX, 
            ref float depthY, 
            ref float depthZ);

        #endregion
    }
}