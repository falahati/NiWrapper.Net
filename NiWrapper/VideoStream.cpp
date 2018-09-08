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
#include "VideoStream_Listener.cpp"
using namespace openni;

extern "C" {
ONI_WRAPPER_API void VideoStream_destroy(VideoStream* vs)
{
	vs->destroy();
}

ONI_WRAPPER_API Status VideoStream_create(VideoStream*& vs, Device* device, SensorType sensorType)
{
	VideoStream* vsl = new VideoStream();
	Status status = vsl->create(*device, sensorType);
	if (status == STATUS_OK)
		vs = vsl;
	else
		VideoStream_destroy(vsl);
	return status;
}

ONI_WRAPPER_API VideoMode* VideoStream_getVideoMode(VideoStream* vs)
{
	VideoMode* p = new VideoMode();
	*p = (vs->getVideoMode());
	return p;
}

ONI_WRAPPER_API Status VideoStream_setVideoMode(VideoStream* vs, VideoMode* vmod)
{
	return vs->setVideoMode(*vmod);
}

ONI_WRAPPER_API CameraSettings* VideoStream_getCameraSettings(VideoStream* vs)
{
	return vs->getCameraSettings();
}

ONI_WRAPPER_API bool VideoStream_isValid(VideoStream* vs)
{
	return vs->isValid();
}

ONI_WRAPPER_API Status VideoStream_start(VideoStream* vs)
{
	return vs->start();
}

ONI_WRAPPER_API void VideoStream_stop(VideoStream* vs)
{
	vs->stop();
}

ONI_WRAPPER_API Status VideoStream_getProperty(VideoStream* vs, int propertyId, void* data, int* dataSize)
{
	return vs->getProperty(propertyId, data, dataSize);
}

ONI_WRAPPER_API bool VideoStream_isPropertySupported(VideoStream* vs, int propertyId)
{
	return vs->isPropertySupported(propertyId);
}

ONI_WRAPPER_API Status VideoStream_setProperty(VideoStream* vs, int propertyId, void* data, int dataSize)
{
	return vs->setProperty(propertyId, data, dataSize);
}

ONI_WRAPPER_API Status VideoStream_invoke(VideoStream* vs, int propertyId, void* data, int dataSize)
{
	return vs->invoke(propertyId, data, dataSize);
}

ONI_WRAPPER_API bool VideoStream_isCommandSupported(VideoStream* vs, int commandId)
{
	return vs->isCommandSupported(commandId);
}

ONI_WRAPPER_API const SensorInfo* VideoStream_getSensorInfo(VideoStream* vs)
{
	return &(vs->getSensorInfo());
}

ONI_WRAPPER_API bool VideoStream_isCroppingSupported(VideoStream* vs)
{
	return vs->isCroppingSupported();
}

ONI_WRAPPER_API Status VideoStream_resetCropping(VideoStream* vs)
{
	return vs->resetCropping();
}

ONI_WRAPPER_API bool VideoStream_getCropping(VideoStream* vs, int* pOriginX, int* pOriginY, int* pWidth, int* pHeight)
{
	return vs->getCropping(pOriginX, pOriginY, pWidth, pHeight);
}

ONI_WRAPPER_API Status VideoStream_setCropping(VideoStream* vs, int pOriginX, int pOriginY, int pWidth, int pHeight)
{
	return vs->setCropping(pOriginX, pOriginY, pWidth, pHeight);
}

ONI_WRAPPER_API bool VideoStream_getMirroringEnabled(VideoStream* vs)
{
	return vs->getMirroringEnabled();
}

ONI_WRAPPER_API Status VideoStream_setMirroringEnabled(VideoStream* vs, bool isEnabled)
{
	return vs->setMirroringEnabled(isEnabled);
}

ONI_WRAPPER_API float VideoStream_getHorizontalFieldOfView(VideoStream* vs)
{
	return vs->getHorizontalFieldOfView();
}

ONI_WRAPPER_API float VideoStream_getVerticalFieldOfView(VideoStream* vs)
{
	return vs->getVerticalFieldOfView();
}

ONI_WRAPPER_API int VideoStream_getMaxPixelValue(VideoStream* vs)
{
	return vs->getMaxPixelValue();
}

ONI_WRAPPER_API int VideoStream_getMinPixelValue(VideoStream* vs)
{
	return vs->getMinPixelValue();
}

ONI_WRAPPER_API Status VideoStream_readFrame(VideoStream* vs, VideoFrameRef*& pFrame)
{
	pFrame = new VideoFrameRef();
	return vs->readFrame(pFrame);
}

ONI_WRAPPER_API VideoStream_Listener* VideoStream_RegisterListener(
	VideoStream* vs, void (*newFrame)(VideoStream*))
{
	VideoStream_Listener* lis = new VideoStream_Listener();
	lis->SetNewFrameCallback(newFrame);
	Status ret = vs->addNewFrameListener(lis);
	if (ret != STATUS_OK)
		return nullptr;
	return lis;
}
};
