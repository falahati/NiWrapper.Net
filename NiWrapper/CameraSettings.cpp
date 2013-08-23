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

	__declspec(dllexport) int CameraSettings_getExposure(CameraSettings* cs)
	{
		 return cs->getExposure();
	}

	__declspec(dllexport) int CameraSettings_getGain(CameraSettings* cs)
	{
		 return cs->getGain();
	}

	__declspec(dllexport) Status CameraSettings_setExposure(CameraSettings* cs, int value)
	{
		 return cs->setExposure(value);
	}

	__declspec(dllexport) Status CameraSettings_setGain(CameraSettings* cs, int value)
	{
		 return cs->setGain(value);
	}
};