#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) GestureType GestureData_getType(GestureData* gd)
	{
		 return gd->getType();
	}

	__declspec(dllexport) bool GestureData_isComplete(GestureData* gd)
	{
		 return gd->isComplete();
	}

	__declspec(dllexport) bool GestureData_isInProgress(GestureData* gd)
	{
		 return gd->isInProgress();
	}

	__declspec(dllexport) void GestureData_getCurrentPosition(GestureData* gd,
		float *X, float *Y, float *Z)
	{
		 Point3f p = gd->getCurrentPosition();
		 *X = p.x; *Y = p.y; *Z = p.z;
	}
};