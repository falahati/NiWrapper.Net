#include <stdio.h>
#include "OpenNI.h"
using namespace openni;

extern "C"
{

	__declspec(dllexport) bool CameraSettings_isValid(CameraSettings* cs)
	{
		 return cs->isValid();
	}

	__declspec(dllexport) bool CameraSettings_getAutoExposureEnabled(CameraSettings* cs)
	{
		 return cs->getAutoExposureEnabled();
	}

	__declspec(dllexport) bool CameraSettings_getAutoWhiteBalanceEnabled(CameraSettings* cs)
	{
		 return cs->getAutoWhiteBalanceEnabled();
	}

	__declspec(dllexport) Status CameraSettings_setAutoExposureEnabled(CameraSettings* cs, bool isEnable)
	{
		 return cs->setAutoExposureEnabled(isEnable);
	}

	__declspec(dllexport) Status CameraSettings_setAutoWhiteBalanceEnabled(CameraSettings* cs, bool isEnable)
	{
		 return cs->setAutoWhiteBalanceEnabled(isEnable);
	}

};