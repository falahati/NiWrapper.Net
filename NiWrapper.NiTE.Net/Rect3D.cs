using System;
using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    public class Rect3D
    {
        public Point3D Min;
        public Point3D Max;

        public Rect3D(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
        {
            Min = new Point3D(xMin, yMin, zMin);
            Max = new Point3D(xMax, yMax, zMax);
        }
    }
}

