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
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void HandTrackerFrameRef_release(HandTrackerFrameRef* vf)
{
	vf->release();
	delete vf;
}

NITE_WRAPPER_API openni::VideoFrameRef* HandTrackerFrameRef_getDepthFrame(HandTrackerFrameRef* vf)
{
	openni::VideoFrameRef* vp = new openni::VideoFrameRef(vf->getDepthFrame());
	return vp;
}

NITE_WRAPPER_API int HandTrackerFrameRef_getFrameIndex(HandTrackerFrameRef* vf)
{
	return vf->getFrameIndex();
}

NITE_WRAPPER_API uint64_t HandTrackerFrameRef_getTimestamp(HandTrackerFrameRef* vf)
{
	return vf->getTimestamp();
}

NITE_WRAPPER_API bool HandTrackerFrameRef_isValid(HandTrackerFrameRef* vf)
{
	return vf->isValid();
}

NITE_WRAPPER_API WrapperArray HandTrackerFrameRef_getGestures(HandTrackerFrameRef* vf)
{
	WrapperArray csarray;
	Array<GestureData>* dIArray = const_cast<Array<GestureData>*>(&(vf->getGestures()));
	csarray.Handle = &dIArray;
	csarray.Size = dIArray->getSize();
	GestureData** dP = new GestureData*[255];
	for (int i = 0; i < dIArray->getSize(); i++)
		dP[i] = const_cast<GestureData*>(&((*dIArray)[i]));
	csarray.Data = dP;
	return csarray;
}

NITE_WRAPPER_API void HandTrackerFrameRef_destroyGesturesArray(WrapperArray p)
{
	GestureData** array = reinterpret_cast<GestureData**>(p.Data);
	delete [] array;
}

NITE_WRAPPER_API WrapperArray HandTrackerFrameRef_getHands(HandTrackerFrameRef* vf)
{
	WrapperArray csarray;
	Array<HandData>* dIArray = const_cast<Array<HandData>*>(&(vf->getHands()));
	csarray.Handle = &dIArray;
	csarray.Size = dIArray->getSize();
	HandData** dP = new HandData*[255];
	for (int i = 0; i < dIArray->getSize(); i++)
		dP[i] = const_cast<HandData*>(&((*dIArray)[i]));
	csarray.Data = dP;
	return csarray;
}

NITE_WRAPPER_API void HandTrackerFrameRef_destroyHandsArray(WrapperArray p)
{
	HandData** array = reinterpret_cast<HandData**>(p.Data);
	delete [] array;
}

};
