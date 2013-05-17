#include <stdio.h>
#include "OpenNI.h"
#include "NiTE.h"
#include "UserTracker_Listener.cpp"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void UserTracker_destroy(UserTracker* ut)
	{
		 ut->destroy();
	}

	__declspec(dllexport) Status UserTracker_create(UserTracker*& ut, openni::Device* device)
	{
		 UserTracker* urt = new UserTracker();
		 Status status = urt->create(device);
		 if (status == STATUS_OK)
			 ut = urt;
		 else
			 UserTracker_destroy(urt);
		 return status;
	}

	__declspec(dllexport) bool UserTracker_isValid(UserTracker* ut)
	{
		 return ut->isValid();
	}

	__declspec(dllexport) Status UserTracker_convertDepthCoordinatesToJoint (
		UserTracker* ut, int x, int y, int z, float *pOutX, float *pOutY)
	{
		 return ut->convertDepthCoordinatesToJoint(x, y, z, pOutX, pOutY);
	}

	__declspec(dllexport) Status UserTracker_convertJointCoordinatesToDepth (
		UserTracker* ut, float x, float y, float z, float *pOutX, float *pOutY)
	{
		return ut->convertJointCoordinatesToDepth(x, y, z, pOutX, pOutY);
	}

	__declspec(dllexport) float UserTracker_getSkeletonSmoothingFactor(UserTracker* ut)
	{
		 return ut->getSkeletonSmoothingFactor();
	}

	__declspec(dllexport) Status UserTracker_setSkeletonSmoothingFactor(UserTracker* ut, float value)
	{
		 return ut->setSkeletonSmoothingFactor(value);
	}

	__declspec(dllexport) Status UserTracker_readFrame(UserTracker* ut, UserTrackerFrameRef*& pFrame)
	{
		pFrame = new UserTrackerFrameRef();
		return ut->readFrame(pFrame);
	}

	__declspec(dllexport) UserTracker_Listener* UserTracker_RegisterListener(
		UserTracker* ut, void (*newFrame)(UserTracker*)){
		UserTracker_Listener* lis = new UserTracker_Listener();
		lis->SetNewFrameCallback(newFrame);
		ut->addListener(lis);
		return lis;
	}

	__declspec(dllexport) Status UserTracker_startPoseDetection(UserTracker* ut, UserId user, PoseType type)
	{
		 return ut->startPoseDetection(user, type);
	}
	__declspec(dllexport) void UserTracker_stopPoseDetection(UserTracker* ut, UserId user, PoseType type)
	{
		 ut->stopPoseDetection(user, type);
	}

	__declspec(dllexport) Status UserTracker_startSkeletonTracking(UserTracker* ut, UserId user)
	{
		 return ut->startSkeletonTracking(user);
	}
	__declspec(dllexport) void UserTracker_stopSkeletonTracking(UserTracker* ut, UserId user)
	{
		 ut->stopSkeletonTracking(user);
	}
};