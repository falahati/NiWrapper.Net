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
#include "NiTE.h"
#include "HandTracker_Listener.cpp"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void HandTracker_destroy(HandTracker* ut)
{
	ut->destroy();
}

NITE_WRAPPER_API Status HandTracker_create(HandTracker*& ut, openni::Device* device)
{
	HandTracker* urt = new HandTracker();
	Status status = urt->create(device);
	if (status == STATUS_OK)
		ut = urt;
	else
		HandTracker_destroy(urt);
	return status;
}

NITE_WRAPPER_API bool HandTracker_isValid(HandTracker* ut)
{
	return ut->isValid();
}

NITE_WRAPPER_API Status HandTracker_convertDepthCoordinatesToHand(
	HandTracker* ut, int x, int y, int z, float* pOutX, float* pOutY)
{
	return ut->convertDepthCoordinatesToHand(x, y, z, pOutX, pOutY);
}

NITE_WRAPPER_API Status HandTracker_convertHandCoordinatesToDepth(
	HandTracker* ut, float x, float y, float z, float* pOutX, float* pOutY)
{
	return ut->convertHandCoordinatesToDepth(x, y, z, pOutX, pOutY);
}

NITE_WRAPPER_API float HandTracker_getSmoothingFactor(HandTracker* ut)
{
	return ut->getSmoothingFactor();
}

NITE_WRAPPER_API Status HandTracker_setSmoothingFactor(HandTracker* ut, float value)
{
	return ut->setSmoothingFactor(value);
}

NITE_WRAPPER_API Status HandTracker_readFrame(HandTracker* ut, HandTrackerFrameRef*& pFrame)
{
	pFrame = new HandTrackerFrameRef();
	return ut->readFrame(pFrame);
}

NITE_WRAPPER_API HandTracker_Listener* HandTracker_RegisterListener(
	HandTracker* ut, void (*newFrame)(HandTracker*))
{
	HandTracker_Listener* lis = new HandTracker_Listener();
	lis->SetNewFrameCallback(newFrame);
	ut->addNewFrameListener(lis);
	return lis;
}

NITE_WRAPPER_API Status HandTracker_startGestureDetection(HandTracker* ut, GestureType type)
{
	return ut->startGestureDetection(type);
}

NITE_WRAPPER_API void HandTracker_stopGestureDetection(HandTracker* ut, GestureType type)
{
	ut->stopGestureDetection(type);
}

NITE_WRAPPER_API Status HandTracker_startHandTracking(HandTracker* ut, float x, float y, float z, HandId* hand)
{
	Point3f* p = new Point3f(x, y, z);
	return ut->startHandTracking(*p, hand);
}

NITE_WRAPPER_API void HandTracker_stopHandTracking(HandTracker* ut, HandId hand)
{
	ut->stopHandTracking(hand);
}
};
