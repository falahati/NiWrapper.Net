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
#include "VideoStream_Listener.cpp"
using namespace openni;

extern "C"
{
	__declspec(dllexport) void VideoStream_destroy(VideoStream* vs)
	{
		 vs->destroy();
	}

	__declspec(dllexport) Status VideoStream_create(VideoStream*& vs, Device* device, SensorType sensorType)
	{
		 VideoStream* vsl = new VideoStream();
		 Status status = vsl->create(*device, sensorType);
		 if (status == STATUS_OK)
			 vs = vsl;
		 else
			 VideoStream_destroy(vsl);
		 return status;
	}

	__declspec(dllexport) VideoMode* VideoStream_getVideoMode(VideoStream* vs)
	{
		 VideoMode* p = new VideoMode();
		 *p = (vs->getVideoMode());
		 return p;
	}
	__declspec(dllexport) Status VideoStream_setVideoMode(VideoStream* vs, VideoMode* vmod)
	{
		 return vs->setVideoMode(*vmod);
	}

	__declspec(dllexport) CameraSettings* VideoStream_getCameraSettings(VideoStream* vs)
	{
		 return vs->getCameraSettings();
	}

	__declspec(dllexport) bool VideoStream_isValid(VideoStream* vs)
	{
		 return vs->isValid();
	}

	__declspec(dllexport) Status VideoStream_start(VideoStream* vs)
	{
		 return vs->start();
	}
	__declspec(dllexport) void VideoStream_stop(VideoStream* vs)
	{
		 vs->stop();
	}

	__declspec(dllexport) Status VideoStream_getProperty(VideoStream* vs, int propertyId, void* data, int* dataSize)
	{
		 return vs->getProperty(propertyId, data, dataSize);
	}
	__declspec(dllexport) bool VideoStream_isPropertySupported(VideoStream* vs, int propertyId)
	{
		 return vs->isPropertySupported(propertyId);
	}
	__declspec(dllexport) Status VideoStream_setProperty(VideoStream* vs, int propertyId, void* data, int dataSize)
	{
		 return vs->setProperty(propertyId, data, dataSize);
	}

	__declspec(dllexport) Status VideoStream_invoke(VideoStream* vs, int propertyId, void* data, int dataSize)
	{
		 return vs->invoke(propertyId, data, dataSize);
	}
	__declspec(dllexport) bool VideoStream_isCommandSupported(VideoStream* vs, int commandId)
	{
		 return vs->isCommandSupported(commandId);
	}

	__declspec(dllexport) const SensorInfo* VideoStream_getSensorInfo(VideoStream* vs)
	{
		 return &(vs->getSensorInfo());
	}

	__declspec(dllexport) bool VideoStream_isCroppingSupported(VideoStream* vs)
	{
		 return vs->isCroppingSupported();
	}
	__declspec(dllexport) Status VideoStream_resetCropping(VideoStream* vs)
	{
		 return vs->resetCropping();
	}
	__declspec(dllexport) bool VideoStream_getCropping(VideoStream* vs, int* pOriginX, int* pOriginY, int* pWidth, int* pHeight)
	{
		 return vs->getCropping(pOriginX, pOriginY, pWidth, pHeight);
	}
	__declspec(dllexport) Status VideoStream_setCropping(VideoStream* vs, int pOriginX, int pOriginY, int pWidth, int pHeight)
	{
		 return vs->setCropping(pOriginX, pOriginY, pWidth, pHeight);
	}

	__declspec(dllexport) bool VideoStream_getMirroringEnabled(VideoStream* vs)
	{
		 return vs->getMirroringEnabled();
	}
	__declspec(dllexport) Status VideoStream_setMirroringEnabled(VideoStream* vs, bool isEnabled)
	{
		 return vs->setMirroringEnabled(isEnabled);
	}

	__declspec(dllexport) float VideoStream_getHorizontalFieldOfView(VideoStream* vs)
	{
		 return vs->getHorizontalFieldOfView();
	}
	__declspec(dllexport) float VideoStream_getVerticalFieldOfView(VideoStream* vs)
	{
		 return vs->getVerticalFieldOfView();
	}

	__declspec(dllexport) int VideoStream_getMaxPixelValue(VideoStream* vs)
	{
		 return vs->getMaxPixelValue();
	}
	__declspec(dllexport) int VideoStream_getMinPixelValue(VideoStream* vs)
	{
		 return vs->getMinPixelValue();
	}

	__declspec(dllexport) Status VideoStream_readFrame(VideoStream* vs, VideoFrameRef*& pFrame)
	{
		pFrame = new VideoFrameRef();
		return vs->readFrame(pFrame);
	}

	__declspec(dllexport) VideoStream_Listener* VideoStream_RegisterListener(
		VideoStream* vs, void (*newFrame)(VideoStream*)){
		VideoStream_Listener* lis = new VideoStream_Listener();
		lis->SetNewFrameCallback(newFrame);
		Status ret = vs->addNewFrameListener(lis);
		if (ret != STATUS_OK)
			return NULL;
		return lis;
	}
};