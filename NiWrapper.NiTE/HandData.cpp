#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void HandData_getPosition(HandData* hd,
		float *X, float *Y, float *Z)
	{
		 Point3f p = hd->getPosition();
		 *X = p.x; *Y = p.y; *Z = p.z;
	}

	__declspec(dllexport) HandId HandData_getId(HandData* hd)
	{
		 return hd->getId();
	}
	
	__declspec(dllexport) bool HandData_isLost(HandData* hd)
	{
		 return hd->isLost();
	}

	__declspec(dllexport) bool HandData_isNew(HandData* hd)
	{
		 return hd->isNew();
	}

	__declspec(dllexport) bool HandData_isTracking(HandData* hd)
	{
		 return hd->isTracking();
	}

	__declspec(dllexport) bool HandData_isTouchingFov(HandData* hd)
	{
		 return hd->isTouchingFov();
	}

};