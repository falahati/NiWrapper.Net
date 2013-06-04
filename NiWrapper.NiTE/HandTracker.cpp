#include <stdio.h>
#include "OpenNI.h"
#include "NiTE.h"
#include "HandTracker_Listener.cpp"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void HandTracker_destroy(HandTracker* ut)
	{
		 ut->destroy();
	}

	__declspec(dllexport) Status HandTracker_create(HandTracker*& ut, openni::Device* device)
	{
		 HandTracker* urt = new HandTracker();
		 Status status = urt->create(device);
		 if (status == STATUS_OK)
			 ut = urt;
		 else
			 HandTracker_destroy(urt);
		 return status;
	}

	__declspec(dllexport) bool HandTracker_isValid(HandTracker* ut)
	{
		 return ut->isValid();
	}

	__declspec(dllexport) Status HandTracker_convertDepthCoordinatesToHand (
		HandTracker* ut, int x, int y, int z, float *pOutX, float *pOutY)
	{
		 return ut->convertDepthCoordinatesToHand(x, y, z, pOutX, pOutY);
	}

	__declspec(dllexport) Status HandTracker_convertHandCoordinatesToDepth (
		HandTracker* ut, float x, float y, float z, float *pOutX, float *pOutY)
	{
		return ut->convertHandCoordinatesToDepth(x, y, z, pOutX, pOutY);
	}

	__declspec(dllexport) float HandTracker_getSmoothingFactor(HandTracker* ut)
	{
		 return ut->getSmoothingFactor();
	}

	__declspec(dllexport) Status HandTracker_setSmoothingFactor(HandTracker* ut, float value)
	{
		 return ut->setSmoothingFactor(value);
	}

	__declspec(dllexport) Status HandTracker_readFrame(HandTracker* ut, HandTrackerFrameRef*& pFrame)
	{
		pFrame = new HandTrackerFrameRef();
		return ut->readFrame(pFrame);
	}

	__declspec(dllexport) HandTracker_Listener* HandTracker_RegisterListener(
		HandTracker* ut, void (*newFrame)(HandTracker*)){
		HandTracker_Listener* lis = new HandTracker_Listener();
		lis->SetNewFrameCallback(newFrame);
		ut->addNewFrameListener(lis);
		return lis;
	}

	__declspec(dllexport) Status HandTracker_startGestureDetection(HandTracker* ut, GestureType type)
	{
		 return ut->startGestureDetection(type);
	}
	__declspec(dllexport) void HandTracker_stopGestureDetection(HandTracker* ut, GestureType type)
	{
		 ut->stopGestureDetection(type);
	}

	__declspec(dllexport) Status HandTracker_startHandTracking(HandTracker* ut, float x, float y, float z, HandId* hand)
	{
		 Point3f* p = new Point3f(x, y, z);
		 return ut->startHandTracking(*p, hand);
	}
	__declspec(dllexport) void HandTracker_stopHandTracking(HandTracker* ut, HandId hand)
	{
		 ut->stopHandTracking(hand);
	}
};