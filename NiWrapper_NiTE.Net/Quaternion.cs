using System.Runtime.InteropServices;

namespace NiTEWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    public class Quaternion
    {
        public float W;
        public float X;
        public float Y;
        public float Z;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}