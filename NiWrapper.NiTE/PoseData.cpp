#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) PoseType PoseData_getType(PoseData* pd)
	{
		 return pd->getType();
	}

	__declspec(dllexport) bool PoseData_isEntered(PoseData* pd)
	{
		 return pd->isEntered();
	}

	__declspec(dllexport) bool PoseData_isExited(PoseData* pd)
	{
		 return pd->isExited();
	}

	__declspec(dllexport) bool PoseData_isHeld(PoseData* pd)
	{
		 return pd->isHeld();
	}
};