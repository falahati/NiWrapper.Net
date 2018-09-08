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

#include "Defines.h"
#include "OpenNI.h"
using namespace openni;

#ifndef UINT16
typedef uint16_t UINT16;
#endif

extern "C" {
ONI_WRAPPER_API const char* DeviceInfo_getName(DeviceInfo* di)
{
	return di->getName();
}

ONI_WRAPPER_API const char* DeviceInfo_getUri(DeviceInfo* di)
{
	return di->getUri();
}

ONI_WRAPPER_API UINT16 DeviceInfo_getUsbProductId(DeviceInfo* di)
{
	return di->getUsbProductId();
}

ONI_WRAPPER_API UINT16 DeviceInfo_getUsbVendorId(DeviceInfo* di)
{
	return di->getUsbVendorId();
}

ONI_WRAPPER_API const char* DeviceInfo_getVendor(DeviceInfo* di)
{
	return di->getVendor();
}
};
