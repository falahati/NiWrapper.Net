#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

extern "C"
{
	__declspec(dllexport) const char* DeviceInfo_getName(DeviceInfo* di)
	{
		 return di->getName();
	}
	__declspec(dllexport) const char* DeviceInfo_getUri(DeviceInfo* di)
	{
		 return di->getUri();
	}
	__declspec(dllexport) UINT16 DeviceInfo_getUsbProductId(DeviceInfo* di)
	{
		 return di->getUsbProductId();
	}
	__declspec(dllexport) UINT16 DeviceInfo_getUsbVendorId(DeviceInfo* di)
	{
		 return di->getUsbVendorId();
	}
	__declspec(dllexport) const char* DeviceInfo_getVendor(DeviceInfo* di)
	{
		 return di->getVendor();
	}
};