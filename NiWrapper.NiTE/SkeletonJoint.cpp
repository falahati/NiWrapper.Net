#include <stdio.h>
#include "Array.cpp"
#include "OpenNI.h"
#include "NiTE.h"
using namespace nite;

extern "C"
{
	__declspec(dllexport) void SkeletonJoint_getOrientation(SkeletonJoint* sj,
		float *x, float *y, float *z, float *w)
	{
		Quaternion q = sj->getOrientation();
		*x = q.x; *y = q.y; *z = q.z; *w = q.w;
	}
	__declspec(dllexport) void SkeletonJoint_getPosition(SkeletonJoint* sj,
		float *x, float *y, float *z)
	{
		Point3f q = sj->getPosition();
		*x = q.x; *y = q.y; *z = q.z;
	}
	__declspec(dllexport) float SkeletonJoint_getOrientationConfidence(SkeletonJoint* sj)
	{
		return sj->getOrientationConfidence();
	}
	__declspec(dllexport) float SkeletonJoint_getPositionConfidence(SkeletonJoint* sj)
	{
		return sj->getPositionConfidence();
	}
	__declspec(dllexport) JointType SkeletonJoint_getType(SkeletonJoint* sj)
	{
		return sj->getType();
	}
};