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
    using System.Runtime.Serialization;

    #endregion

    // ReSharper disable once InconsistentNaming
    public class OpenNIException : Exception
    {
        #region Constructors and Destructors

        public OpenNIException()
        {
        }

        public OpenNIException(string message)
            : base(message)
        {
        }

        public OpenNIException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected OpenNIException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}