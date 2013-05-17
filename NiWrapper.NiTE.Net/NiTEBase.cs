using System;
using System.Collections.Generic;
using System.Text;

namespace NiTEWrapper
{
    public abstract class NiTEBase
    {
        public IntPtr Handle { get; protected set; }

        public bool Equals(NiTEBase obj)
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
