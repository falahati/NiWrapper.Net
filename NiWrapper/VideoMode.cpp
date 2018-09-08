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
ONI_WRAPPER_API VideoMode* VideoMode_new()
{
	VideoMode* vml = new VideoMode();
	return vml;
}

ONI_WRAPPER_API PixelFormat VideoMode_getPixelFormat(VideoMode* vm)
{
	return vm->getPixelFormat();
}

ONI_WRAPPER_API void VideoMode_setPixelFormat(VideoMode* vm, PixelFormat pf)
{
	vm->setPixelFormat(pf);
}

ONI_WRAPPER_API int VideoMode_getFps(VideoMode* vm)
{
	return vm->getFps();
}

ONI_WRAPPER_API void VideoMode_setFps(VideoMode* vm, int fps)
{
	vm->setFps(fps);
}

ONI_WRAPPER_API int VideoMode_getResolutionX(VideoMode* vm)
{
	return vm->getResolutionX();
}

ONI_WRAPPER_API int VideoMode_getResolutionY(VideoMode* vm)
{
	return vm->getResolutionY();
}

ONI_WRAPPER_API void VideoMode_setResolution(VideoMode* vm, int resolutionX, int resolutionY)
{
	vm->setResolution(resolutionX, resolutionY);
}
};
