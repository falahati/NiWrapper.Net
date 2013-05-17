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