using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNIWrapper
{
    public struct Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int width, int height) : this()
        {
            Width = width;
            Height = height;
        }
    }
}
