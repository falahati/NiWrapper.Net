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
#include "UserTracker_Listener.cpp"
using namespace nite;

extern "C" {
NITE_WRAPPER_API void UserTracker_destroy(UserTracker* ut)
{
	ut->destroy();
}

NITE_WRAPPER_API Status UserTracker_create(UserTracker*& ut, openni::Device* device)
{
	UserTracker* urt = new UserTracker();
	Status status = urt->create(device);
	if (status == STATUS_OK)
		ut = urt;
	else
		UserTracker_destroy(urt);
	return status;
}

NITE_WRAPPER_API bool UserTracker_isValid(UserTracker* ut)
{
	return ut->isValid();
}

NITE_WRAPPER_API Status UserTracker_convertDepthCoordinatesToJoint(
	UserTracker* ut, int x, int y, int z, float* pOutX, float* pOutY)
{
	return ut->convertDepthCoordinatesToJoint(x, y, z, pOutX, pOutY);
}

NITE_WRAPPER_API Status UserTracker_convertJointCoordinatesToDepth(
	UserTracker* ut, float x, float y, float z, float* pOutX, float* pOutY)
{
	return ut->convertJointCoordinatesToDepth(x, y, z, pOutX, pOutY);
}

NITE_WRAPPER_API float UserTracker_getSkeletonSmoothingFactor(UserTracker* ut)
{
	return ut->getSkeletonSmoothingFactor();
}

NITE_WRAPPER_API Status UserTracker_setSkeletonSmoothingFactor(UserTracker* ut, float value)
{
	return ut->setSkeletonSmoothingFactor(value);
}

NITE_WRAPPER_API Status UserTracker_readFrame(UserTracker* ut, UserTrackerFrameRef*& pFrame)
{
	pFrame = new UserTrackerFrameRef();
	return ut->readFrame(pFrame);
}

NITE_WRAPPER_API UserTracker_Listener* UserTracker_RegisterListener(
	UserTracker* ut, void (*newFrame)(UserTracker*))
{
	UserTracker_Listener* lis = new UserTracker_Listener();
	lis->SetNewFrameCallback(newFrame);
	ut->addNewFrameListener(lis);
	return lis;
}

NITE_WRAPPER_API Status UserTracker_startPoseDetection(UserTracker* ut, UserId user, PoseType type)
{
	return ut->startPoseDetection(user, type);
}

NITE_WRAPPER_API void UserTracker_stopPoseDetection(UserTracker* ut, UserId user, PoseType type)
{
	ut->stopPoseDetection(user, type);
}

NITE_WRAPPER_API Status UserTracker_startSkeletonTracking(UserTracker* ut, UserId user)
{
	return ut->startSkeletonTracking(user);
}

NITE_WRAPPER_API void UserTracker_stopSkeletonTracking(UserTracker* ut, UserId user)
{
	ut->stopSkeletonTracking(user);
}
};
