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

extern "C" {
ONI_WRAPPER_API void Device_close(Device* di)
{
	di->close();
}

ONI_WRAPPER_API bool Device_getDepthColorSyncEnabled(Device* di)
{
	return di->getDepthColorSyncEnabled();
}

ONI_WRAPPER_API DeviceInfo* Device_getDeviceInfo(Device* di)
{
	return const_cast<DeviceInfo*>(&(di->getDeviceInfo()));
}

ONI_WRAPPER_API ImageRegistrationMode Device_getImageRegistrationMode(Device* di)
{
	return di->getImageRegistrationMode();
}

ONI_WRAPPER_API PlaybackControl* Device_getPlaybackControl(Device* di)
{
	return di->getPlaybackControl();
}

ONI_WRAPPER_API Status Device_getProperty(Device* di, int propertyId, void* data, int* dataSize)
{
	return di->getProperty(propertyId, data, dataSize);
}

ONI_WRAPPER_API const SensorInfo* Device_getSensorInfo(Device* di, SensorType sensorType)
{
	return di->getSensorInfo(sensorType);
}

ONI_WRAPPER_API bool Device_hasSensor(Device* di, SensorType sensorType)
{
	return di->hasSensor(sensorType);
}

ONI_WRAPPER_API Status Device_invoke(Device* di, int propertyId, void* data, int dataSize)
{
	return di->invoke(propertyId, data, dataSize);
}

ONI_WRAPPER_API bool Device_isCommandSupported(Device* di, int commandId)
{
	return di->isCommandSupported(commandId);
}

ONI_WRAPPER_API bool Device_isFile(Device* di)
{
	return di->isFile();
}

ONI_WRAPPER_API bool Device_isImageRegistrationModeSupported(Device* di, ImageRegistrationMode mode)
{
	return di->isImageRegistrationModeSupported(mode);
}

ONI_WRAPPER_API bool Device_isPropertySupported(Device* di, int propertyId)
{
	return di->isPropertySupported(propertyId);
}

ONI_WRAPPER_API bool Device_isValid(Device* di)
{
	return di->isValid();
}

ONI_WRAPPER_API Status Device_open(Device*& di, const char* uri)
{
	Device* dl = new Device();
	Status status = dl->open(uri);
	if (status == STATUS_OK)
		di = dl;
	else
		Device_close(dl);
	return status;
}

ONI_WRAPPER_API Status Device__openEx(Device*& di, const char* uri, const char* mode)
{
	Device* dl = new Device();
	Status status = dl->_openEx(uri, mode);
	if (status == STATUS_OK)
		di = dl;
	else
		Device_close(dl);
	return status;
}

ONI_WRAPPER_API Status Device_setDepthColorSyncEnabled(Device* di, bool enable)
{
	return di->setDepthColorSyncEnabled(enable);
}

ONI_WRAPPER_API Status Device_setImageRegistrationMode(Device* di, ImageRegistrationMode mode)
{
	return di->setImageRegistrationMode(mode);
}

ONI_WRAPPER_API Status Device_setProperty(Device* di, int propertyId, void* data, int dataSize)
{
	return di->setProperty(propertyId, data, dataSize);
}
};
