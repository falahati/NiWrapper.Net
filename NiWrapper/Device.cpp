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

#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

extern "C"
{
	__declspec(dllexport) void Device_close(Device* di)
	{
		 di->close();
	}
	__declspec(dllexport) bool Device_getDepthColorSyncEnabled(Device* di)
	{
		 return di->getDepthColorSyncEnabled();
	}
	__declspec(dllexport) DeviceInfo* Device_getDeviceInfo(Device* di)
	{
		 return const_cast<DeviceInfo*>(&(di->getDeviceInfo()));
	}
	__declspec(dllexport) ImageRegistrationMode Device_getImageRegistrationMode(Device* di)
	{
		 return di->getImageRegistrationMode();
	}
	__declspec(dllexport) PlaybackControl* Device_getPlaybackControl(Device* di)
	{
		 return di->getPlaybackControl();
	}
	__declspec(dllexport) Status Device_getProperty(Device* di, int propertyId, void* data, int* dataSize)
	{
		 return di->getProperty(propertyId, data, dataSize);
	}
	__declspec(dllexport) const SensorInfo* Device_getSensorInfo(Device* di, SensorType sensorType)
	{
		 return di->getSensorInfo(sensorType);
	}
	__declspec(dllexport) bool Device_hasSensor(Device* di, SensorType sensorType)
	{
		 return di->hasSensor(sensorType);
	}
	__declspec(dllexport) Status Device_invoke(Device* di, int propertyId, void* data, int dataSize)
	{
		 return di->invoke(propertyId, data, dataSize);
	}
	__declspec(dllexport) bool Device_isCommandSupported(Device* di, int commandId)
	{
		 return di->isCommandSupported(commandId);
	}
	__declspec(dllexport) bool Device_isFile(Device* di)
	{
		 return di->isFile();
	}
	__declspec(dllexport) bool Device_isImageRegistrationModeSupported(Device* di, ImageRegistrationMode mode)
	{
		 return di->isImageRegistrationModeSupported(mode);
	}
	__declspec(dllexport) bool Device_isPropertySupported(Device* di, int propertyId)
	{
		 return di->isPropertySupported(propertyId);
	}
	__declspec(dllexport) bool Device_isValid(Device* di)
	{
		 return di->isValid();
	}

	__declspec(dllexport) Status Device_open(Device*& di, const char* uri)
	{
		 Device* dl = new Device();
		 Status status = dl->open(uri);
		 if (status == STATUS_OK)
			 di = dl;
		 else
			 Device_close(dl);
		 return status;
	}

    __declspec(dllexport) Status Device__openEx(Device*& di, const char* uri, const char* mode)
    {
        Device* dl = new Device();
        Status status = dl->_openEx(uri, mode);
        if (status == STATUS_OK)
            di = dl;
        else
            Device_close(dl);
        return status;
    }

	__declspec(dllexport) Status Device_setDepthColorSyncEnabled(Device* di, bool enable)
	{
		 return di->setDepthColorSyncEnabled(enable);
	}
	__declspec(dllexport) Status Device_setImageRegistrationMode(Device* di, ImageRegistrationMode mode)
	{
		 return di->setImageRegistrationMode(mode);
	}
	__declspec(dllexport) Status Device_setProperty(Device* di, int propertyId, void* data, int dataSize)
	{
		 return di->setProperty(propertyId, data, dataSize);
	}
};