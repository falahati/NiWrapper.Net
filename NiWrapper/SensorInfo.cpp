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
#include "Array.cpp"

using namespace openni;

extern "C" {

ONI_WRAPPER_API SensorType SensorInfo_getSensorType(SensorInfo* si)
{
	return si->getSensorType();
}

ONI_WRAPPER_API WrapperArray SensorInfo_getSupportedVideoModes(SensorInfo* si)
{
	WrapperArray csarray;
	const Array<VideoMode>& modes = si->getSupportedVideoModes();
	Array<VideoMode>* dIArray = new Array<VideoMode>(&modes[0], modes.getSize());
	csarray.Handle = dIArray;
	csarray.Size = dIArray->getSize();
	VideoMode** dP = new VideoMode*[255];
	for (int i = 0; i < dIArray->getSize(); i++)
		dP[i] = const_cast<VideoMode*>(&((*dIArray)[i]));
	csarray.Data = dP;
	return csarray;
}

ONI_WRAPPER_API void SensorInfo_destroyVideoModesArray(WrapperArray p)
{
	VideoMode** array = reinterpret_cast<VideoMode**>(p.Data);
	delete [] array;
	Array<VideoMode>* handle = reinterpret_cast<Array<VideoMode>*>(p.Handle);
	delete handle;
}
};
