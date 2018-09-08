using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    public class Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}