using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNIWrapper
{
    public abstract class OpenNIBase
    {
        public IntPtr Handle { get; protected set; }

        public bool Equals(OpenNIBase obj)
        {
            return this.Handle == obj.Handle;
        }

        public bool Equals(IntPtr obj)
        {
            return this.Handle == obj;
        }

        public bool isValid
        {
            get
            {
                return !this.Handle.Equals(IntPtr.Zero);
            }
        }
    }
}
