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
#include "Array.cpp"
using namespace openni;

extern "C"
{

	__declspec(dllexport) SensorType SensorInfo_getSensorType(SensorInfo* si)
	{
		 return si->getSensorType();
	}

	__declspec(dllexport) WrapperArray SensorInfo_getSupportedVideoModes(SensorInfo* si){
		WrapperArray* csarray = new WrapperArray();
		const Array<VideoMode>& dIArray = si->getSupportedVideoModes();
		csarray->Size = dIArray.getSize();
		VideoMode** dP = new VideoMode*[255];
		for (int i = 0; i < dIArray.getSize(); i++)
			dP[i] = const_cast<VideoMode*>(&(dIArray[i]));
		csarray->Data = dP;
		return *csarray;
	}

	__declspec(dllexport) void SensorInfo_destroyVideoModesArray(WrapperArray p){
		delete[] p.Data;
	}
};