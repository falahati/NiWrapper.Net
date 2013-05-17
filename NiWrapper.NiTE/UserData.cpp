#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void UserData_getBoundingBox(UserData* ud,
		float *minX, float *minY, float *minZ, float *maxX, float *maxY, float *maxZ)
	{
		 BoundingBox bb = ud->getBoundingBox();
		 *minX = bb.min.x; *minY = bb.min.y; *minZ = bb.min.z;
		 *maxX = bb.max.x; *maxY = bb.max.y; *maxZ = bb.max.z;
	}

	__declspec(dllexport) void UserData_getCenterOfMass(UserData* ud,
		float *X, float *Y, float *Z)
	{
		 Point3f p = ud->getCenterOfMass();
		 *X = p.x; *Y = p.y; *Z = p.z;
	}

	__declspec(dllexport) UserId UserData_getId(UserData* ud)
	{
		 return ud->getId();
	}

	__declspec(dllexport) PoseData* UserData_getPose(UserData* ud, PoseType type)
	{
		 return const_cast<PoseData*>(&ud->getPose(type));
	}

	__declspec(dllexport) Skeleton* UserData_getSkeleton(UserData* ud)
	{
		 return const_cast<Skeleton*>(&ud->getSkeleton());
	}

	__declspec(dllexport) bool UserData_isLost(UserData* ud)
	{
		 return ud->isLost();
	}

	__declspec(dllexport) bool UserData_isNew(UserData* ud)
	{
		 return ud->isNew();
	}

	__declspec(dllexport) bool UserData_isVisible(UserData* ud)
	{
		 return ud->isVisible();
	}

};