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
	__declspec(dllexport) VideoMode* VideoMode_new()
	{
		 VideoMode* vml = new VideoMode();
		 return vml;
	}

	__declspec(dllexport) PixelFormat VideoMode_getPixelFormat(VideoMode* vm)
	{
		 return vm->getPixelFormat();
	}
	__declspec(dllexport) void VideoMode_setPixelFormat(VideoMode* vm, PixelFormat pf)
	{
		 vm->setPixelFormat(pf);
	}

	__declspec(dllexport) int VideoMode_getFps(VideoMode* vm)
	{
		 return vm->getFps();
	}
	__declspec(dllexport) void VideoMode_setFps(VideoMode* vm, int fps)
	{
		 vm->setFps(fps);
	}

	__declspec(dllexport) int VideoMode_getResolutionX(VideoMode* vm)
	{
		 return vm->getResolutionX();
	}
	__declspec(dllexport) int VideoMode_getResolutionY(VideoMode* vm)
	{
		 return vm->getResolutionY();
	}
	__declspec(dllexport) void VideoMode_setResolution(VideoMode* vm, int resolutionX, int resolutionY)
	{
		 vm->setResolution(resolutionX, resolutionY);
	}
};