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
ONI_WRAPPER_API bool CameraSettings_isValid(CameraSettings* cs)
{
	return cs->isValid();
}

ONI_WRAPPER_API bool CameraSettings_getAutoExposureEnabled(CameraSettings* cs)
{
	return cs->getAutoExposureEnabled();
}

ONI_WRAPPER_API bool CameraSettings_getAutoWhiteBalanceEnabled(CameraSettings* cs)
{
	return cs->getAutoWhiteBalanceEnabled();
}

ONI_WRAPPER_API Status CameraSettings_setAutoExposureEnabled(CameraSettings* cs, bool isEnable)
{
	return cs->setAutoExposureEnabled(isEnable);
}

ONI_WRAPPER_API Status CameraSettings_setAutoWhiteBalanceEnabled(CameraSettings* cs, bool isEnable)
{
	return cs->setAutoWhiteBalanceEnabled(isEnable);
}

ONI_WRAPPER_API int CameraSettings_getExposure(CameraSettings* cs)
{
	return cs->getExposure();
}

ONI_WRAPPER_API int CameraSettings_getGain(CameraSettings* cs)
{
	return cs->getGain();
}

ONI_WRAPPER_API Status CameraSettings_setExposure(CameraSettings* cs, int value)
{
	return cs->setExposure(value);
}

ONI_WRAPPER_API Status CameraSettings_setGain(CameraSettings* cs, int value)
{
	return cs->setGain(value);
}
};
