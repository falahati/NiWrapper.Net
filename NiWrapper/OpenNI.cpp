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
#include "OpenNI_Listener.cpp"
#include "Array.cpp"
using namespace openni;

extern "C" {
ONI_WRAPPER_API Version OpenNI_getVersion()
{
	return OpenNI::getVersion();
}

ONI_WRAPPER_API const char* OpenNI_getExtendedError()
{
	return OpenNI::getExtendedError();
}

ONI_WRAPPER_API Status OpenNI_initialize()
{
	return OpenNI::initialize();
}

ONI_WRAPPER_API void OpenNI_shutdown()
{
	OpenNI::shutdown();
}

ONI_WRAPPER_API Status OpenNI_waitForAnyStream(VideoStream** vsArray, int vsArraySize, int* selectedStream, int timeOut)
{
	return OpenNI::waitForAnyStream(vsArray, vsArraySize, selectedStream, timeOut);
}

ONI_WRAPPER_API OpenNI_Listener* OpenNI_RegisterListener(
	void (*connect)(DeviceInfo*),
	void (*disconnect)(DeviceInfo*),
	void (*statechanged)(DeviceInfo*, DeviceState))
{
	OpenNI_Listener* lis = new OpenNI_Listener();
	lis->SetConnectCallback(connect);
	lis->SetDisconnectCallback(disconnect);
	lis->SetStateChangedCallback(statechanged);
	Status ret = OpenNI::addDeviceConnectedListener(lis);
	if (ret != STATUS_OK)
		return nullptr;
	ret = OpenNI::addDeviceDisconnectedListener(lis);
	if (ret != STATUS_OK)
		return nullptr;
	ret = OpenNI::addDeviceStateChangedListener(lis);
	if (ret != STATUS_OK)
		return nullptr;
	return lis;
}

ONI_WRAPPER_API WrapperArray OpenNI_enumerateDevices()
{
	WrapperArray csarray;
	Array<DeviceInfo>* dIArray = new Array<DeviceInfo>();
	OpenNI::enumerateDevices(dIArray);
	csarray.Handle = dIArray;
	csarray.Size = dIArray->getSize();
	DeviceInfo** dP = new DeviceInfo*[255];
	for (int i = 0; i < dIArray->getSize(); i++)
		dP[i] = const_cast<DeviceInfo*>(&((*dIArray)[i]));
	csarray.Data = dP;
	return csarray;
}

ONI_WRAPPER_API void OpenNI_destroyDevicesArray(WrapperArray p)
{
	DeviceInfo** array = reinterpret_cast<DeviceInfo**>(p.Data);
	delete [] array;
	//Array<DeviceInfo>* handle = reinterpret_cast<Array<DeviceInfo>*>(p.Handle);
	//delete handle;
}
};
